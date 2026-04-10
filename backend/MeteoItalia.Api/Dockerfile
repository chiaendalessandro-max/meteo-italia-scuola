# Build da questa cartella: docker build -t meteoitalia-api .
# Avvio su Render/Azure: la variabile PORT viene letta allo startup.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY MeteoItalia.Api.csproj .
RUN dotnet restore MeteoItalia.Api.csproj
COPY . .
RUN dotnet publish MeteoItalia.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
CMD ["/bin/sh", "-c", "exec dotnet MeteoItalia.Api.dll --urls \"http://0.0.0.0:${PORT:-8080}\""]
