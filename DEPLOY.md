# Deploy (sintesi)

Istruzioni dettagliate passo passo: **`ISTRUZIONI-PUBBLICAZIONE.md`** nella cartella principale del progetto.

- **Un solo URL (consigliato)**: pubblica solo `MeteoItalia.Api` (include `wwwroot`); niente localhost nel flusso utente.
- **Due URL**: cartella `frontend` su Netlify/Vercel + API su Azure/Render; modifica `frontend/js/config.js` con l’URL del backend.
