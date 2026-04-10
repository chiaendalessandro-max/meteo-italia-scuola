using System.Text.Json;
using MeteoItalia.Api.DTOs;
using MeteoItalia.Api.Services.OpenMeteo;

namespace MeteoItalia.Api.Services;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _http;
    private readonly ILogger<GeocodingService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GeocodingService(HttpClient http, ILogger<GeocodingService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CityMatchDto>> SearchItalianCitiesAsync(string name, CancellationToken cancellationToken = default)
    {
        name = name.Trim();
        if (name.Length < 2)
        {
            return Array.Empty<CityMatchDto>();
        }

        var q = Uri.EscapeDataString(name);
        var url = $"v1/search?name={q}&count=12&language=it&countryCode=IT";

        using var response = await _http.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Geocoding Open-Meteo fallito: {Status}", response.StatusCode);
            throw new HttpRequestException("Ricerca città temporaneamente non disponibile.");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var doc = await JsonSerializer.DeserializeAsync<OpenMeteoGeocodeResponse>(stream, JsonOptions, cancellationToken);
        var results = doc?.Results;
        if (results is null || results.Count == 0)
        {
            return Array.Empty<CityMatchDto>();
        }

        return results
            .Where(r => string.Equals(r.CountryCode, "IT", StringComparison.OrdinalIgnoreCase))
            .Select(r => new CityMatchDto
            {
                Name = r.Name ?? string.Empty,
                Admin1 = r.Admin1,
                Latitude = r.Latitude,
                Longitude = r.Longitude
            })
            .Where(r => !string.IsNullOrWhiteSpace(r.Name))
            .ToList();
    }
}
