# **BUG / DIFETTI**

## **Scena 0**

### **Problemi di movimento:**
- Dopo che il **pg** si alza, dovrebbe guardare verso il letto, **non** verso la finestra (evitare la retromarcia del pg).

### **JumpScare (opzionale, se c'è tempo):**
#### **Stanza adiacente:**
1. **Foro** dietro ai libri della libreria:
   - Se il player rimuove i libri e ci interagisce, vede: // C'è un buco lì?
     1. **Scenario 1:** "Persona" seduta. // L'ex conquilino doveva essere proprio un guardone.
     2. **Scenario 2:** "Persona" seduta, fissata da un mutante.
     3. **Scenario 3:** "Persona" morta (possibili graffi o segni) cosparsa di sangue, e il mutante è scomparso.
     4. **Scenario 4:** Stanza vuota quindi pura sua allucinazione.

## Scena2

- Discorso con UB bloccato dopo aver portato il lupo (forse dovuto all'evento del nodo -> taskfucile.cs)
- **Task di sparare col fucile:**
  - Mancano **rig layer**, dialoghi e controlli.

- **Salute Mentale**
  - Implementare respiro affannoso per Stefano quando si attiva la sanità mentale
- **Tests**:
  - **Lupo**:
    - ~~Potrebbe arrivare a destinazione ma continua a _camminare_ e non va in idle (penso di averlo fixato @ccorvino3)~~ 
    - ~~ Potrebbe decidere di _non attaccare_ a volte (penso di averlo fixato @ccorvino3)~~ 
      03.12.24: @marco Effettuato Test dopo la Build, si posiziona correttamente
    - _Non collide_ con alberi, ~~lago~~, ecc...
  - **Libreria**
    - A volte lo Spacebar non funzionara e non avviene l'interazione oppure non lo permette dopo la prima transizione
  - **Radio**
    - Appena arrivo in Scena2 la musica è già attiva nonostante ho impostato il blocco in ManagerScena2 allo start
  - **Urgete**
    - Bug Dialogo per l'event dell'uomo Baita pre-shoting

**Implementazioni facoltative**
  - Togliere l'erba che va dal sentiero di destra verso l'uomo baita, soprtutto quella sul terreno
  - Inserire punti di riferimento in ogni sentiero, possibilmente diversi.
  - Nel primo dialogo potremmo aggiungere qualcosa che dica al giocatore che ora può premere ALT per lo swithc camera e che se non si vuole perdere 
    può seguire i sentieri ma niente gli vieta di non entrare nelle boscaglie
  
- Decidere per le **ammo** _cosa fare_

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
