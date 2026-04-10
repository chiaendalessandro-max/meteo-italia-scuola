# Guida veloce (compito scolastico — link pubblico per il professore)

## 1. È un sito statico o c’è anche il backend C#?

| Parte | Cosa è |
|--------|--------|
| **Frontend** | Sì: HTML, CSS, JavaScript (cartella `wwwroot` nel progetto API, oppure cartella `frontend`). |
| **Backend** | **Sì, obbligatorio**: progetto **C# ASP.NET Core** (`MeteoItalia.Api`). Senza di esso non ci sono meteo né ricerca città online. |

**Conclusione:** non è “solo un sito statico”. Per farlo vedere **funzionante** al professore devi pubblicare **online anche il backend**, non solo file HTML su GitHub Pages.

---

## 2. Modo più semplice (consigliato per la scuola)

**Un solo link** che apre tutto (pagina + API):

1. Pubblichi **un solo progetto**: `MeteoItalia.Api` (contiene già il sito in `wwwroot`).
2. Usi un servizio cloud che esegue **ASP.NET** in automatico.

**Due opzioni realistiche per studenti:**

| Servizio | Difficoltà | Nota |
|----------|------------|------|
| **Azure App Service** (piano gratuito / crediti studenti) | Media | Molto usato a scuola; da Visual Studio: *Pubblica* sul cloud. |
| **Render.com** (piano Free) | Media | Colleghi GitHub e usi il `Dockerfile` nella cartella del backend. |

**Non consigliato per questo compito:** mettere **solo** il frontend su GitHub Pages o Netlify **senza** backend online: il sito **non** mostrerà il meteo reale.

---

## 3. Quali file / cosa caricare?

### Opzione A — Tutto in uno (la più semplice)

Non carichi “a mano” file sparsi: pubblichi l’intero progetto backend così:

- Cartella del progetto: `MeteoItalia\backend\MeteoItalia.Api\`
- Contiene: `Program.cs`, `Controllers`, `wwwroot` (il sito), ecc.

**Da Visual Studio:** tasto destro sul progetto → **Pubblica** → scegli **Azure** (App Service) e segui la procedura.

**Oppure da terminale** (solo se ti chiedono di fare così):

```text
cd MeteoItalia\backend\MeteoItalia.Api
dotnet publish -c Release -o .\publish
```

La cartella `publish` è quella che alcuni servizi usano per il deploy.

### Opzione B — Due siti (più passi)

- **Frontend:** cartella `MeteoItalia\frontend\` → Netlify/Vercel (trascina la cartella).
- **Backend:** stesso progetto `MeteoItalia.Api` su Azure o Render.
- Prima di pubblicare il frontend, devi modificare **`frontend\js\config.js`**: metti l’URL **https** che ti dà il backend (vedi sezione 4).

---

## 4. Come “eliminare” localhost

- **Deploy tutto in uno (wwwroot):** nel file `wwwroot\js\config.js` c’è già `window.METEO_API_BASE = ""`.  
  Non devi scrivere localhost: le chiamate vanno a `/api/...` sul **stesso** dominio del sito pubblico.

- **Solo dopo** che hai deployato il backend, il link sarà tipo:
  - `https://qualcosa.azurewebsites.net` oppure  
  - `https://qualcosa.onrender.com`

- **Se** separi frontend e backend, apri **`frontend\js\config.js`** e sostituisci la riga con l’URL HTTPS del backend (quello che vedi nel browser quando il backend è online), **senza** `/` alla fine.

**Localhost** resta solo nei file di **sviluppo** sul tuo PC (`launchSettings.json`, `appsettings.Development.json`): non li carichi come “configurazione del sito pubblico”; il server usa `Production` in cloud.

---

## 5. Come ottenere il link pubblico da dare al professore

1. Completa il deploy su **Azure** o **Render** (o altro hosting che esegue .NET).
2. Nel pannello del servizio trovi l’URL pubblico (HTTPS).
3. **Copia quel link** e aprilo da un altro PC o dal telefono (anche in rete mobile): se vedi il meteo, va bene per la consegna.

**Prova rapida:** apri `https://TUO-URL/api/health` — se risponde con `ok`, il backend è raggiungibile da Internet.

---

## Riepilogo in 3 righe

1. Il progetto ha **frontend statico + backend C#**.  
2. La strada più semplice per il compito è **un solo deploy** del progetto `MeteoItalia.Api` con `wwwroot`.  
3. Il **link pubblico** è quello che ti dà Azure o Render; **non** serve aprire il terminale sul tuo PC quando il professore guarda il sito.

---

*Per dettagli tecnici in più, vedi `ISTRUZIONI-PUBBLICAZIONE.md` nella stessa cartella.*
