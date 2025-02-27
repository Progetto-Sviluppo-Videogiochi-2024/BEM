# Vincoli

## Caratteristiche Gioco

- **Aree esplorabili**: Scelta di aree tra quelle accessibili.
- **Scambio di oggetti**: Non è consentito tra personaggi.
- **Ricompense**: Ottenibili tramite:
  - Quest, rompicapi, cassaforti (codice sparso nel gioco).
  - Porte chiuse (chiavi sparse per il gioco).
  - Oggetti nascosti.
- **Nemici**: Mutanti e soldati presenti nelle aree esplorabili.
- **Interazione**: Modalità Single Player.
- **Oggetti**: Consumabili, non consumabili e collezionabili.
- **NPC**: Presenti con dialoghi e quest secondarie.
- **Crafting**: Possibile solo tra oggetti craftabili, con ricette sparse nel gioco.

## Giocatore

- **Salute**:
  - Non visibile, ma intuibile dalla GUI.
  - Se la HP è 0, il personaggio muore e riparte dall'ultimo checkpoint.
  - Possibilità di utilizzare oggetti curativi.

- **Sanità mentale**: 

  - **Effetti**:
    - SE (sanitaMentale <= 40) menteSana = false IMPLICA "SFX (affanno), VFX (allucinazioni horror)"
    - (DA IMPLEMENTARE) Possibilità di poter essere controllato dal Boss (blocco wasd per qualche secondo) se la sua sanità mentale = 60
    
  - **Cosa la decrementa**.
    - Se la HP <= 20 ALLORA -15 sanità
    - Morte di un Amico = -30 sanità
    - Danno subito da un nemico = -10 sanità

  - **Cosa la incremente**
    - Si recupera con cibo e oggetti medici.

  - **Visibilità nella GUI Player**
    - Valore pari a player.maxHealth all'inizio.
      - Esempio: sanita = 100 => "stefano sta bene mentalmente".
    - Al decrementare del suo valore la visibilità dell'icona sarà maggiore



- **Armi**:
  - **Melee (Corpo a corpo)**:
    - Armi bianche come coltello, mazza, bastone, ascia.
    - Combo di due attacchi ripetuti.
    - Consuma stamina per ogni attacco.
    - Il coltello in modalità stealth permette di eliminare nemici senza essere scoperti.
  - **Ranged (Distanza)**:
    - Armi da fuoco, con tempo di ricarica e munizioni limitate.
    - Munizioni da raccogliere, armi automatiche e non automatiche.
  - **Armi da lancio**:
    - Oggetti come sassi e bottiglie.
    - Utilizzabili per attirare, attaccare o distrarre i nemici.
- **Oggetti**:
  - Un solo oggetto equipaggiato in un solo slot (GUI e Inventario)

## Inventario

- **Oggetti**:
  - **Quantità limitata**: Dipende dallo spazio disponibile nell'inventario.
  - **Consumabili e non consumabili**.
  - **Equipaggiabili/non equipaggiabili**: Armi e oggetti di uso.
  - **Utilizzabili**: Medikit, bende, soluzioni curative, cibo, bevande, sigarette.
  - **Ispezionabili**: Visualizzazione di un pop-up con informazioni sull'oggetto.
  - **Scartabili**: Rimuove una quantità dell'oggetto dall'inventario.
  - **Rimuovibili**: Eliminati definitivamente dall'inventario (non recuperabili).
- **Collezionabili**:
  - **Craftabili**: Solo con la ricetta giusta.
  - **Non equipaggiabili**.

## Mutanti

- Se ti avvistano possono attaccarti anche fino a zona radio a meno che tu li semini/uccidi
  - Non puoi salvare in zona radio finché non li semini/uccidi
- Se morti non respawnano

## UI

### 1. Durante il gioco (senza UI aperte)

- **Cinematica e movimenti attivi**.
- **Cursore invisibile**.

### 2. Durante il gioco (con UI aperte)

#### a. **GPM e Radio**

- **Cursore visibile**.
- Il gioco viene **bloccato** (nessuna cinematica né movimenti).

#### b. **Dialoghi**

- **Cursore visibile**.
- Solo i **movimenti** del giocatore vengono **bloccati** (se non è tutorial, solo con NPC), la cinematica continua.

#### c. **Inventario e Cellulare**

- **Cursore visibile**.
- Se il cursore **è sul Canvas**:
  - La **cinematica viene disabilitata**.
- Quando il cursore **esce dal Canvas**:
  - La **cinematica riparte**.
