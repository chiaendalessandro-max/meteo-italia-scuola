using MeteoItalia.Api.DTOs;

namespace MeteoItalia.Api.Services;

public interface IWeatherService
{
    /// <summary>Meteo corrente + breve trend dalle ore successive (Open-Meteo).</summary>
    Task<WeatherCurrentResponse> GetCurrentAsync(double latitude, double longitude, string? locationLabel, CancellationToken cancellationToken = default);
}
