using MeteoItalia.Api.DTOs;
using MeteoItalia.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeteoItalia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weather;
    private readonly IGeocodingService _geocoding;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(IWeatherService weather, IGeocodingService geocoding, ILogger<WeatherController> logger)
    {
        _weather = weather;
        _geocoding = geocoding;
        _logger = logger;
    }

    /// <summary>Meteo corrente per coordinate (es. da geolocalizzazione browser).</summary>
    [HttpGet("current")]
    [ProducesResponseType(typeof(WeatherCurrentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WeatherCurrentResponse>> GetCurrent(
        [FromQuery] double lat,
        [FromQuery] double lon,
        [FromQuery] string? label = null,
        CancellationToken cancellationToken = default)
    {
        if (lat is < -90 or > 90 || lon is < -180 or > 180)
        {
            return Problem(
                title: "Coordinate non valide",
                detail: "Usa latitudine tra -90 e 90 e longitudine tra -180 e 180.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        try
        {
            var weather = await _weather.GetCurrentAsync(lat, lon, label, cancellationToken);
            return Ok(weather);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Errore chiamata meteo");
            return Problem(
                title: "Servizio meteo non disponibile",
                detail: ex.Message,
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore imprevisto meteo");
            return Problem(
                title: "Errore meteo",
                detail: "Impossibile elaborare la risposta del servizio meteo.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>Ricerca città italiana + meteo per il primo risultato.</summary>
    [HttpGet("city")]
    [ProducesResponseType(typeof(CitySearchWeatherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CitySearchWeatherResponse>> GetByCityName(
        [FromQuery] string name,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 2)
        {
            return Problem(
                title: "Nome non valido",
                detail: "Inserisci almeno 2 caratteri.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        try
        {
            var matches = await _geocoding.SearchItalianCitiesAsync(name, cancellationToken);
            if (matches.Count == 0)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Nessun risultato",
                    Detail = "Nessuna località trovata in Italia per la ricerca indicata.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            var first = matches[0];
            var label = string.IsNullOrWhiteSpace(first.Admin1)
                ? first.Name
                : $"{first.Name} ({first.Admin1})";

            var weather = await _weather.GetCurrentAsync(first.Latitude, first.Longitude, label, cancellationToken);

            return Ok(new CitySearchWeatherResponse
            {
                Matches = matches,
                PrimaryWeather = weather
            });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Errore geocoding");
            return Problem(
                title: "Ricerca non disponibile",
                detail: ex.Message,
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore ricerca città");
            return Problem(
                title: "Errore ricerca",
                detail: "Impossibile completare la ricerca.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
