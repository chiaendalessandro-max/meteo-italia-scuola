using System.Text.Json.Serialization;

namespace MeteoItalia.Api.Services.OpenMeteo;

/// <summary>Modelli minimi per deserializzare la risposta JSON di Open-Meteo.</summary>
public class OpenMeteoForecastResponse
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("current")]
    public OpenMeteoCurrent? Current { get; set; }

    [JsonPropertyName("hourly")]
    public OpenMeteoHourly? Hourly { get; set; }
}

public class OpenMeteoCurrent
{
    [JsonPropertyName("time")]
    public string? Time { get; set; }

    [JsonPropertyName("temperature_2m")]
    public double? Temperature2m { get; set; }

    [JsonPropertyName("relative_humidity_2m")]
    public int? RelativeHumidity2m { get; set; }

    [JsonPropertyName("apparent_temperature")]
    public double? ApparentTemperature { get; set; }

    [JsonPropertyName("weather_code")]
    public int? WeatherCode { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double? WindSpeed10m { get; set; }

    [JsonPropertyName("wind_direction_10m")]
    public int? WindDirection10m { get; set; }
}

public class OpenMeteoHourly
{
    [JsonPropertyName("time")]
    public List<string>? Time { get; set; }

    [JsonPropertyName("temperature_2m")]
    public List<double>? Temperature2m { get; set; }
}

public class OpenMeteoGeocodeResponse
{
    [JsonPropertyName("results")]
    public List<OpenMeteoGeocodeResult>? Results { get; set; }
}

public class OpenMeteoGeocodeResult
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("admin1")]
    public string? Admin1 { get; set; }

    /// <summary>Es. "IT" (campo JSON snake_case).</summary>
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }
}
