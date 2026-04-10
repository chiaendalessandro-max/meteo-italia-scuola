namespace MeteoItalia.Api.Services.OpenMeteo;

/// <summary>
/// Mapping codici WMO Open-Meteo → descrizione italiana + chiave icona.
/// Riferimento: https://open-meteo.com/en/docs
/// </summary>
public static class WeatherCodeMapper
{
    public static (string Description, string IconKey) Map(int code)
    {
        return code switch
        {
            0 => ("Sereno", "clear"),
            1 => ("Prevalentemente sereno", "clear"),
            2 => ("Parzialmente nuvoloso", "partly-cloudy"),
            3 => ("Nuvoloso", "cloudy"),
            45 or 48 => ("Nebbia", "fog"),
            51 or 53 or 55 => ("Pioggerella", "drizzle"),
            56 or 57 => ("Pioggerella gelata", "drizzle"),
            61 or 63 or 65 => ("Pioggia", "rain"),
            66 or 67 => ("Pioggia gelata", "rain"),
            71 or 73 or 75 => ("Nevicate", "snow"),
            77 => ("Granelli di neve", "snow"),
            80 or 81 or 82 => ("Rovesci", "rain-showers"),
            85 or 86 => ("Rovesci di neve", "snow-showers"),
            95 => ("Temporale", "thunder"),
            96 or 99 => ("Temporale con grandine", "thunder-hail"),
            _ => ("Condizioni variabili", "unknown")
        };
    }
}
