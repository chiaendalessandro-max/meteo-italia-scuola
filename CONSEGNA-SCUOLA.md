# Meteo Semplice — consegna scuola (spiegazione + pubblicazione)

Questo progetto è pensato per essere **facile da capire**:
- **Frontend**: pagina web (HTML/CSS/JS)
- **Backend**: programma **C#** che risponde su Internet e va a prendere il meteo

## 1) Spiegazione semplice (come a scuola)

### Cos'è il frontend
Il frontend è la **parte che vedi nel browser**: testi, colori, pulsanti, caselle di ricerca.

### Cos'è il backend
Il backend è la **parte che gira su un computer del cloud** (un server online). Non la vedi, ma fa i lavori "pesanti" e restituisce **dati** al sito.

### Cosa fa il C#
In questo progetto il C# (ASP.NET) fa queste cose:
- riceve una richiesta tipo: "dammi il meteo per questa città"
- trova le coordinate della città (servizio di ricerca luoghi)
- chiede il meteo usando quelle coordinate (servizio meteo)
- risponde al sito con un testo strutturato (**JSON**): numeri e descrizioni

### Cosa fanno HTML, CSS e JavaScript
- **HTML**: struttura della pagina (titolo, campo ricerca, area risultati)
- **CSS**: aspetto (colori, spazi, leggibilità)
- **JavaScript**: comportamento (quando premi Cerca, manda la richiesta al backend e aggiorna la pagina)

### Come comunicano tra loro
Quando premi **Cerca**, il JavaScript nel browser fa una richiesta via Internet al backend (indirizzo HTTPS).
Il backend risponde con **JSON** e il JavaScript scrive i valori nella pagina.

## 2) Struttura del progetto (schema semplice)

Cartella principale: `MeteoItalia/`

- `backend/MeteoItalia.Api/`
  - `Program.cs`: avvio del server C# e impostazioni base (API + file statici)
  - `Program.cs`: endpoint **Minimal API** (es. `GET /api/meteo/citta?city=...`)
  - `Services/`: codice che parla con i servizi esterni (meteo e ricerca città)
  - `DTOs/`: modelli di dati semplici (come “schede” con campi)
  - `wwwroot/`: copia del sito (HTML/CSS/JS) servita dallo stesso backend (utile per avere **un solo link**)
- `frontend/`
  - `index.html`, `css/style.css`, `js/app.js`: sito semplice
  - `js/config.js`: qui metti l'URL pubblico del backend se pubblichi il frontend su Netlify

File guida:
- `GUIDA-SCUOLA.md` e `ISTRUZIONI-PUBBLICAZIONE.md`: passi per andare online

## 3) Funzionamento del sito (passo passo)

### Quando apri il sito
Il browser scarica `index.html`, carica `style.css` e `app.js`.
La pagina mostra il modulo di ricerca.

### Quando cerchi una città
1) Il JavaScript legge il nome scritto nella casella.
2) Chiama il backend: `/api/meteo/citta?city=...`
3) Il backend:
   - cerca la città in Italia e ottiene **latitudine e longitudine**
   - chiede il meteo per quelle coordinate
   - prepara una risposta semplice (temperatura, umidità, vento, descrizione)
4) Il JavaScript riceve la risposta e aggiorna la pagina.

### Come arrivano i dati meteo
Il backend non “inventa” il meteo: lo chiede a un servizio pubblico di meteo (**Open-Meteo**), usando coordinate geografiche.

## 4) Discorso da 1–2 minuti (da dire al professore)

Ho fatto un sito molto semplice che mostra il meteo di una città italiana.
La parte che si vede nel browser e fatta con HTML per la struttura, CSS per lo stile e JavaScript per i comandi quando premo Cerca.

Il meteo però non lo calcola il sito da solo: ho un backend scritto in C# che gira online.
Quando cerco una città, il JavaScript manda una richiesta al mio backend.
Il backend trova dove si trova la città e poi chiede i dati del meteo a un servizio esterno, e mi risponde con i numeri in formato JSON.

In pratica: il frontend serve per usare il sito, il backend serve per ottenere dati aggiornati in modo sicuro e ordinato.

## 5) Versione pubblicabile (link per tutti, senza localhost)

Hai due modi semplici. Scegline uno.

### Opzione consigliata: UN SOLO LINK (più facile da spiegare)
Pubblichi solo `backend/MeteoItalia.Api` (include `wwwroot`).
Risultato: apri un URL tipo `https://....onrender.com` e hai **sito + API insieme**.

Cose da controllare:
- in `wwwroot/js/config.js` deve esserci `window.METEO_API_BASE = ""` (stringa vuota)

### Opzione: DUE LINK (Netlify + backend)
1) Pubblichi il backend e copi il suo URL pubblico HTTPS.
2) Apri `frontend/js/config.js` e sostituisci `https://SOSTITUISCI-IL-TUO-BACKEND` con quell'URL (senza slash finale).
3) Pubblichi la cartella `frontend` su Netlify.

Verifica veloce prima della consegna:
- apri `https://TUO-BACKEND/api/health` e controlli che risponda “ok”
- apri il sito da telefono in 4G per essere sicuro che sia pubblico
