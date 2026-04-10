namespace MeteoItalia.Api.DTOs;

/// <summary>Risposta unificata per il meteo corrente (coordinate o città).</summary>
public class WeatherCurrentResponse
{
    public string? LocationLabel { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double TemperatureC { get; set; }

    public double ApparentTemperatureC { get; set; }

    public int RelativeHumidityPercent { get; set; }

    /// <summary>Velocità vento in km/h (Open-Meteo usa m/s, convertiamo).</summary>
    public double WindSpeedKmh { get; set; }

    public int WindDirectionDegrees { get; set; }

    public int WeatherCode { get; set; }

    public string Description { get; set; } = string.Empty;

    /// <summary>Chiave semplice per icona lato client (es. partly-cloudy).</summary>
    public string IconKey { get; set; } = "unknown";

    /// <summary>Testo breve sulle prossime ore, se disponibile.</summary>
    public string? ShortForecast { get; set; }
}
