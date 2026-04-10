# Meteo Italia — pubblicazione online (procedura operativa)

## Risposte rapide ai tuoi punti

| Domanda | Risposta |
|--------|----------|
| Il frontend è statico? | **Sì**: HTML/CSS/JS sono file statici. |
| Il backend C# è obbligatorio? | **Sì**: le API `/api/...` e le chiamate a Open-Meteo girano solo lì. Senza backend non c’è meteo. |
| Cosa pubblichi | **Opzione A**: solo il progetto `MeteoItalia.Api` (include `wwwroot` = sito + API sullo stesso URL). **Opzione B**: cartella `frontend` su Netlify/Vercel + backend su Azure/Render. |
| Localhost | Rimosso da `appsettings.json` di base; gli URL di sviluppo sono solo in `appsettings.Development.json`. In produzione usa variabili sul cloud o `appsettings.Production.json`. |

---

## Soluzione più semplice (consigliata se vuoi un solo URL)

**Un solo deploy**: pubblichi **solo** il backend ASP.NET con la cartella `wwwroot` già nel progetto.

- URL pubblico unico, es. `https://meteo-api.azurewebsites.net`
- **Non** serve Netlify per il frontend: apri quel link da qualsiasi PC/smartphone.
- In `wwwroot/js/config.js` lascia `window.METEO_API_BASE = ""` (già così).

**Passi (Azure App Service, esempio):**

1. Apri **Visual Studio** o terminale nella cartella `backend\MeteoItalia.Api`.
2. Esegui pubblicazione verso **Azure** → **App Service** → runtime **.NET 9**.
3. Nel portale Azure → **Configuration** → **Application settings** aggiungi:
   - `ASPNETCORE_ENVIRONMENT` = `Production`
4. Ottieni l’URL tipo `https://<nome>.azurewebsites.net` e aprilo nel browser.

**Cosa sostituire:** nulla nel codice per l’URL se usi solo Azure all-in-one; il sito usa path relativi `/api/...`.

---

## Soluzione a due pezzi (frontend Netlify/Vercel + backend Render/Azure)

Usala se vuoi il sito su un dominio “statico” e l’API altrove.

### 1) File da aprire e modificare (frontend)

| File | Cosa fare |
|------|-----------|
| `frontend/js/config.js` | Sostituisci `https://SOSTITUISCI-URL-PUBBLICO-DEL-BACKEND` con l’URL **reale** del backend (es. `https://meteo-xxxx.onrender.com`), **senza** slash finale. |
| `frontend/index.html` | Di solito **nessuna** modifica (già carica `config.js` prima di `app.js`). |

### 2) File da configurare (backend)

| File / impostazione | Cosa fare |
|---------------------|-----------|
| `appsettings.Production.json` | `Cors:AllowedOrigins` vuoto → il codice usa `AllowAnyOrigin` (ok per MVP). Per restringere: imposta gli URL esatti del frontend (vedi sotto). |
| Portale hosting (Azure/Render) | `ASPNETCORE_ENVIRONMENT` = `Production`. |
| Variabili ambiente CORS (opzionale) | `Cors__AllowedOrigins__0` = `https://tuosito.netlify.app` |

### 3) Cosa pubblicare dove

| Dove | Cosa carichi |
|------|----------------|
| **Netlify** | Contenuto della cartella **`frontend`** (dopo aver salvato `config.js` con l’URL del backend). Root del sito = cartella dove c’è `index.html`. |
| **Vercel** | Stessa cartella **`frontend`**. |
| **Render / Azure (API)** | Progetto **`MeteoItalia.Api`**: da Visual Studio *Publish*, oppure **Docker** usando `backend/MeteoItalia.Api/Dockerfile`, oppure `dotnet publish` e deploy della cartella pubblicata. |

### 4) Sostituire “localhost” con URL reali

- **Non** inserire localhost nel codice pubblicato.
- Frontend pubblico: solo **`frontend/js/config.js`** → URL HTTPS del backend.
- Backend: nessun localhost in `appsettings.json` (base); sviluppo solo in `appsettings.Development.json`).

### 5) API in produzione

Le route principali sono **`/api/weather/...`** (ricerca città + meteo) e **`/api/health`** (controllo rapido).

- Stesso dominio del backend: richieste a `/api/...` (relative).
- Frontend su altro dominio: richieste a `https://tuo-backend/api/...` (assoluto, da `config.js`).

### 6) Dominio pubblico personalizzato

- **Netlify/Vercel**: imposti il dominio nel pannello del sito (DNS come da guida).
- **Azure**: *Custom domains* sull’App Service.
- **Backend**: stesso dominio (all-in-one) oppure sottodominio tipo `api.tuodominio.it` se separi frontend/backend.

---

## Database

In questa versione “semplice per la scuola” **non c’è database**: ogni ricerca chiede i dati al servizio meteo e li mostra subito.

---

## Confronto piattaforme (semplicità)

| Scelta | Difficoltà | Note |
|--------|------------|------|
| **Solo Azure App Service** (wwwroot + API) | Bassa | Un URL, niente Netlify, niente CORS tra sito e API. |
| **Netlify + Render** | Media | Due deploy; `config.js` + CORS; spesso free tier. |
| **Vercel + Azure** | Media | Simile a sopra. |

---

## Build Docker (backend) — esempio

Da `c:\Users\chiae\Desktop\MeteoItalia\backend\MeteoItalia.Api`:

```powershell
docker build -t meteoitalia-api .
docker run -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Production meteoitalia-api
```

Su **Render**: *New Web Service* → collega il repo → Dockerfile path `backend/MeteoItalia.Api/Dockerfile`, context `backend/MeteoItalia.Api`.

---

## Checklist prima di andare online

- [ ] `frontend/js/config.js` punta all’URL HTTPS del backend (deploy separato) **oppure** usi solo backend con `wwwroot` e `METEO_API_BASE` vuoto.
- [ ] Backend con `ASPNETCORE_ENVIRONMENT=Production`.
- [ ] Test `GET https://TUO-BACKEND/api/health` da browser o Postman.
- [ ] Apri il sito da **telefono in 4G** (non Wi‑Fi casa) per verificare accesso pubblico.
