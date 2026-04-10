using System.Globalization;
using System.Text.Json;
using MeteoItalia.Api.DTOs;
using MeteoItalia.Api.Services.OpenMeteo;

namespace MeteoItalia.Api.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _http;
    private readonly ILogger<WeatherService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public WeatherService(HttpClient http, ILogger<WeatherService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<WeatherCurrentResponse> GetCurrentAsync(
        double latitude,
        double longitude,
        string? locationLabel,
        CancellationToken cancellationToken = default)
    {
        var query =
            $"v1/forecast?latitude={latitude.ToString(CultureInfo.InvariantCulture)}" +
            $"&longitude={longitude.ToString(CultureInfo.InvariantCulture)}" +
            "&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m,wind_direction_10m" +
            "&hourly=temperature_2m" +
            "&forecast_days=2" +
            "&timezone=auto";

        using var response = await _http.GetAsync(query, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Open-Meteo forecast fallita: {Status}", response.StatusCode);
            throw new HttpRequestException("Servizio meteo temporaneamente non disponibile.");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var dto = await JsonSerializer.DeserializeAsync<OpenMeteoForecastResponse>(stream, JsonOptions, cancellationToken)
                  ?? throw new InvalidOperationException("Risposta meteo non valida.");

        if (dto.Current is null)
        {
            throw new InvalidOperationException("Dati meteo correnti assenti.");
        }

        var c = dto.Current;
        var code = c.WeatherCode ?? 0;
        var (description, iconKey) = WeatherCodeMapper.Map(code);

        var shortForecast = BuildShortForecast(c.Time, dto.Hourly, c.Temperature2m ?? 0);

        return new WeatherCurrentResponse
        {
            LocationLabel = locationLabel,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            TemperatureC = c.Temperature2m ?? 0,
            ApparentTemperatureC = c.ApparentTemperature ?? c.Temperature2m ?? 0,
            RelativeHumidityPercent = c.RelativeHumidity2m ?? 0,
            WindSpeedKmh = c.WindSpeed10m ?? 0,
            WindDirectionDegrees = c.WindDirection10m ?? 0,
            WeatherCode = code,
            Description = description,
            IconKey = iconKey,
            ShortForecast = shortForecast
        };
    }

    /// <summary>Confronta le prossime ore con la temperatura attuale (testo semplice).</summary>
    private static string? BuildShortForecast(string? currentIsoTime, OpenMeteoHourly? hourly, double currentTempC)
    {
        if (hourly?.Time is null || hourly.Temperature2m is null || string.IsNullOrWhiteSpace(currentIsoTime))
        {
            return null;
        }

        if (!DateTime.TryParse(currentIsoTime, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var nowUtc))
        {
            return null;
        }

        var times = hourly.Time;
        var temps = hourly.Temperature2m;
        var startIndex = -1;
        for (var i = 0; i < times.Count; i++)
        {
            if (!DateTime.TryParse(times[i], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var t))
            {
                continue;
            }

            if (t >= nowUtc)
            {
                startIndex = i;
                break;
            }
        }

        if (startIndex < 0)
        {
            startIndex = 0;
        }

        var slice = new List<double>();
        for (var i = startIndex; i < Math.Min(startIndex + 4, temps.Count); i++)
        {
            slice.Add(temps[i]);
        }

        if (slice.Count == 0)
        {
            return null;
        }

        var min = slice.Min();
        var max = slice.Max();
        var delta = slice.Last() - currentTempC;

        var trend = delta switch
        {
            > 0.5 => "in leggero aumento",
            < -0.5 => "in leggero calo",
            _ => "stabile"
        };

        return $"Prossime ore: tra {min:F0}°C e {max:F0}°C, tendenza {trend}.";
    }
}
