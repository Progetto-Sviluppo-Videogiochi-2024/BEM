# **BUG / DIFETTI**

## Scena 0 

## Scena2

- **Lupo:**
  - Non collide con gli alberi (solo lago fixato)

- **Tests**:
  - **Lupo**:
    - ~~Potrebbe arrivare a destinazione ma continua a _camminare_ e non va in idle (penso di averlo fixato @ccorvino3)~~ 
    - ~~Potrebbe decidere di _non attaccare_ a volte (penso di averlo fixato @ccorvino3)~~
    - _Non collide_ con alberi, ~~lago~~, ecc...

**Implementazioni facoltative**
  - Togliere l'erba che va dal sentiero di destra verso l'uomo baita, soprattutto quella sul terreno
  - Inserire punti di riferimento in ogni sentiero, possibilmente diversi.
  - Nel primo dialogo potremmo aggiungere qualcosa che dica al giocatore che ora può premere ALT per lo switch camera e che se non si vuole perdere 
    può seguire i sentieri ma niente gli vieta di non entrare nelle boscaglie

## Scena 3

- Da sviluppare.

## Generali

- **Salvataggio/Caricamento**
  - Da implementare
- **Sezioni MainMenu e GPM**
  - Da implementare (esempio Opzioni ecc) inoltre bisogna cambiare il nome di alcuni e scriverli in italiano, eccetto "Play Game" o altri comunemente usati
- **Cursore**
  - Sembra funzionare bene per ora.
- **AI di NPC e villain**
  - Da completare, controllare la sezione "Vincoli".
- **Gestione della camera:**
  - Stefano: movimento, crouch, aim, **ALT**.
    **Offset animazioni**
  - Da verificare con @ccorvino3
- **Rompicapi**
  - da implementare.
- **Gestione dell'audio**
  - Nelle varie scene

# **DA DECIDERE INSIEME:**

- Estetica di:
  - **Menu main**, **menu pausa**, **gameplay menu**.: Migliorare l'estetica se necessario una volta finito il game
- Definire e implementare le **meccaniche core**. (In scena2 sono presenti quasi tutte, tranne il crouch in modo esplicito)
- Creare il **documento di game design**.
- Realizzare una presentazione in **PowerPoint**.
- Ricontrollare a fine gioco se i file progettazione presentano tutti gli aspetti del gioco (se vanno consegnati al prof)

---
