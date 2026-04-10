namespace MeteoItalia.Api.DTOs;

/// <summary>Risultato geocoding (Italia).</summary>
public class CityMatchDto
{
    public string Name { get; set; } = string.Empty;

    public string? Admin1 { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}

/// <summary>Ricerca città: elenco match + meteo per il primo risultato (se presente).</summary>
public class CitySearchWeatherResponse
{
    public IReadOnlyList<CityMatchDto> Matches { get; set; } = Array.Empty<CityMatchDto>();

    /// <summary>Meteo per il primo match; null se non ci sono risultati.</summary>
    public WeatherCurrentResponse? PrimaryWeather { get; set; }
}
