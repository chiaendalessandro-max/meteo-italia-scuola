using MeteoItalia.Api.DTOs;

namespace MeteoItalia.Api.Services;

public interface IGeocodingService
{
    /// <summary>Cerca località in Italia tramite Open-Meteo Geocoding.</summary>
    Task<IReadOnlyList<CityMatchDto>> SearchItalianCitiesAsync(string name, CancellationToken cancellationToken = default);
}
