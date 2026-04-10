const form = document.getElementById("search-form");
const cityInput = document.getElementById("city-input");
const messageBox = document.getElementById("message-box");
const weatherBox = document.getElementById("weather-box");

function showMessage(text, isError = false) {
  messageBox.textContent = text;
  messageBox.className = isError ? "message error" : "message";
}

function clearMessage() {
  messageBox.textContent = "";
  messageBox.className = "message";
}

function setWeather(data) {
  document.getElementById("city-name").textContent = data.locationLabel || "Citta trovata";
  document.getElementById("description").textContent = data.description;
  document.getElementById("temperature").textContent = `${data.temperatureC.toFixed(1)} °C`;
  document.getElementById("humidity").textContent = `${data.relativeHumidityPercent}%`;
  document.getElementById("wind").textContent = `${data.windSpeedKmh.toFixed(0)} km/h`;
  weatherBox.classList.remove("hidden");
}

async function searchWeatherByCity(cityName) {
  const apiBase = (window.METEO_API_BASE || "").replace(/\/$/, "");
  const url = `${apiBase}/api/weather/city?name=${encodeURIComponent(cityName)}`;
  const response = await fetch(url);
  const data = await response.json();

  if (!response.ok) {
    throw new Error(data?.detail || "Errore durante la ricerca.");
  }

  if (!data?.primaryWeather) {
    throw new Error("Nessun meteo trovato per questa citta.");
  }

  return data.primaryWeather;
}

form.addEventListener("submit", async (event) => {
  event.preventDefault();
  const cityName = cityInput.value.trim();
  if (cityName.length < 2) {
    showMessage("Inserisci almeno 2 lettere.", true);
    return;
  }

  clearMessage();
  showMessage("Caricamento...");

  try {
    const weatherData = await searchWeatherByCity(cityName);
    setWeather(weatherData);
    showMessage("Meteo caricato.");
  } catch (error) {
    weatherBox.classList.add("hidden");
    showMessage(error.message, true);
  }
});
