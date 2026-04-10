/**
 * Chiama il backend C# (Minimal API GET /api/meteo/citta?city=...)
 * METEO_API_BASE: "" se sito e API sono sullo stesso dominio (es. Render con wwwroot).
 */
function getApiBase() {
  const raw = (window.METEO_API_BASE || "").trim().replace(/\/$/, "");
  if (!raw || raw.includes("SOSTITUISCI")) {
    return "";
  }
  return raw;
}

function buildMeteoUrl(cityName) {
  const base = getApiBase();
  const path = `/api/meteo/citta?city=${encodeURIComponent(cityName)}`;
  return base ? `${base}${path}` : path;
}

const iconByKey = {
  clear: "☀️",
  "partly-cloudy": "⛅",
  cloudy: "☁️",
  fog: "🌫️",
  drizzle: "🌦️",
  rain: "🌧️",
  snow: "❄️",
  "rain-showers": "🌦️",
  "snow-showers": "🌨️",
  thunder: "⛈️",
  "thunder-hail": "⛈️",
  unknown: "🌤️",
};

const form = document.getElementById("search-form");
const cityInput = document.getElementById("city-input");
const messageBox = document.getElementById("message-box");
const weatherBox = document.getElementById("weather-box");

function showMessage(text, isError) {
  messageBox.textContent = text;
  messageBox.className = "feedback" + (isError ? " feedback--error" : "");
}

function clearMessage() {
  messageBox.textContent = "";
  messageBox.className = "feedback";
}

function setWeather(data) {
  document.getElementById("city-name").textContent =
    data.locationLabel || "Città trovata";
  document.getElementById("description").textContent = data.description || "";
  document.getElementById("temperature").textContent =
    typeof data.temperatureC === "number"
      ? `${data.temperatureC.toFixed(1)} °C`
      : "—";
  document.getElementById("feels").textContent =
    typeof data.apparentTemperatureC === "number"
      ? `${data.apparentTemperatureC.toFixed(1)} °C`
      : "—";
  document.getElementById("humidity").textContent =
    typeof data.relativeHumidityPercent === "number"
      ? `${data.relativeHumidityPercent} %`
      : "—";
  document.getElementById("wind").textContent =
    typeof data.windSpeedKmh === "number"
      ? `${data.windSpeedKmh.toFixed(0)} km/h`
      : "—";

  const emoji = iconByKey[data.iconKey] || iconByKey.unknown;
  document.getElementById("weather-icon").textContent = emoji;

  const sf = data.shortForecast || "";
  const el = document.getElementById("short-forecast");
  el.textContent = sf;
  el.classList.toggle("hidden", !sf);

  weatherBox.classList.remove("hidden");
}

function readServerMessage(data) {
  if (!data || typeof data !== "object") {
    return "Risposta del server non valida.";
  }
  if (typeof data.messaggio === "string" && data.messaggio) {
    return data.messaggio;
  }
  if (typeof data.detail === "string" && data.detail) {
    return data.detail;
  }
  if (typeof data.title === "string" && data.title) {
    return data.title;
  }
  return "Qualcosa è andato storto. Riprova.";
}

async function searchWeatherByCity(cityName) {
  const url = buildMeteoUrl(cityName);
  let response;
  try {
    response = await fetch(url, { headers: { Accept: "application/json" } });
  } catch (err) {
    throw new Error(
      "Non riesco a contattare il server. Se usi Netlify, apri js/config.js e inserisci l'URL pubblico del backend (HTTPS)."
    );
  }

  const text = await response.text();
  let data = null;
  if (text) {
    try {
      data = JSON.parse(text);
    } catch {
      throw new Error("Il server ha risposto in modo non previsto. Controlla che il backend sia online.");
    }
  }

  if (!response.ok || data.ok === false) {
    throw new Error(readServerMessage(data));
  }

  if (!data.meteo) {
    throw new Error("Nessun dato meteo nella risposta.");
  }

  return data.meteo;
}

form.addEventListener("submit", async (event) => {
  event.preventDefault();
  const cityName = cityInput.value.trim();
  if (cityName.length < 2) {
    showMessage("Inserisci almeno 2 lettere.", true);
    return;
  }

  clearMessage();
  showMessage("Caricamento…", false);

  try {
    const weatherData = await searchWeatherByCity(cityName);
    setWeather(weatherData);
    clearMessage();
  } catch (error) {
    weatherBox.classList.add("hidden");
    showMessage(error.message, true);
  }
});
