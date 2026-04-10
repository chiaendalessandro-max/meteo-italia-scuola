using MeteoItalia.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    client.BaseAddress = new Uri("https://api.open-meteo.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<IGeocodingService, GeocodingService>(client =>
{
    client.BaseAddress = new Uri("https://geocoding-api.open-meteo.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        if (corsOrigins is { Length: > 0 })
        {
            policy.WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", utc = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

// Meteo per nome città (Italia). Query: ?city=Roma  oppure  ?name=Roma (compatibilità)
app.MapGet("/api/meteo/citta", async (
        string? city,
        string? name,
        IWeatherService weatherService,
        IGeocodingService geocodingService,
        ILogger<Program> logger,
        CancellationToken cancellationToken) =>
    {
        var searchText = !string.IsNullOrWhiteSpace(city) ? city.Trim() : name?.Trim() ?? "";
        if (searchText.Length < 2)
        {
            return Results.BadRequest(new
            {
                ok = false,
                messaggio = "Scrivi almeno 2 lettere del nome della città."
            });
        }

        try
        {
            var matches = await geocodingService.SearchItalianCitiesAsync(searchText, cancellationToken);
            if (matches.Count == 0)
            {
                return Results.NotFound(new
                {
                    ok = false,
                    messaggio = "Non ho trovato questa città in Italia. Prova un altro nome (es. \"Roma\", \"Milano\")."
                });
            }

            var first = matches[0];
            var label = string.IsNullOrWhiteSpace(first.Admin1)
                ? first.Name
                : $"{first.Name} ({first.Admin1})";

            var meteo = await weatherService.GetCurrentAsync(
                first.Latitude,
                first.Longitude,
                label,
                cancellationToken);

            return Results.Ok(new { ok = true, messaggio = (string?)null, meteo });
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Errore di rete verso il servizio meteo o geocoding");
            return Results.Json(
                new { ok = false, messaggio = "Il servizio meteo non è raggiungibile al momento. Riprova tra qualche minuto." },
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Errore imprevisto durante la ricerca meteo");
            return Results.Json(
                new { ok = false, messaggio = "Si è verificato un errore imprevisto. Riprova più tardi." },
                statusCode: StatusCodes.Status500InternalServerError);
        }
    })
    .WithTags("Meteo");

app.Run();
