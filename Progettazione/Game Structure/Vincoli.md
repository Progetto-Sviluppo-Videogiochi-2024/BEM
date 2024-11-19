# Vincoli

## Vincoli sui nemici e utilizzo delle componenti UI 
1. non può salvare quando i mutanti lo hanno avvistato e quindi quando è in stato di allerta, else può salvare
- possono attaccarti se ti inseguono fino alla zona della radio
- mutanti che non respawnano se vengono uccisi

2.
    1.durante il gioco senza UI aperte: cinematica e movimenti, cursore invisibile
    2. durante il gioco con UI: cursore visibile e:
- GPM e radio: blocco il gioco
- dialoghi: blocco solo i movimenti
- inventario e cellulare: se cursore qui sopra, la cinematica viene disabilitata e quando esce dal canvas riparte la cinematica

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
  - Se la salute è 0, il personaggio muore e riparte dall'ultimo checkpoint.
  - Possibilità di utilizzare oggetti curativi.
- **Stamina**:
  - Non visibile al giocatore.
  - Se la stamina è 0, il personaggio cammina lentamente o con affanno.
- **Sanità mentale**:
  - Effetti uditivi (audio), non visibile al giocatore.
  - Dipende dagli HP del personaggio.
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
