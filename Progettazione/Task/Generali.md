# **BUG / DIFETTI**

## **Scena 0**

### **Problemi di movimento:**
- Dopo che il **pg** si alza, dovrebbe guardare verso il letto, **non** verso la finestra (evitare la retromarcia del pg).

### **JumpScare (opzionale, se c'è tempo):**
#### **Stanza adiacente:**
1. **Foro** dietro ai libri della libreria:
   - Se il player rimuove i libri e ci interagisce, vede:
     1. **Scenario 1:** "Persona" che dorme.
     2. **Scenario 2:** "Persona" che dorme, fissata da un mutante.
     3. **Scenario 3:** "Persona" morta (possibili graffi o segni) cosparsa di sangue, e il mutante è scomparso.
     4. **Scenario 4:**  
        - "Persona" scomparsa, sangue e mutante scomparsi.  
        - Oppure, "Persona" che dorme senza sangue e senza mutante.
2. **Foro** tra la libreria e il dipinto:
   - Stesse possibilità di **Scenario 1** sopra.

## Scena2

- **Task di sparare col fucile:**
  - Mancano **rig layer**, dialoghi e controlli.
- Manca l'**ammo del fucile** di Scena2 (potremmo usare anche la stessa per ogni arma).
- **Salute Mentale**
  - Implementare respiro affannoso per Stefano quando si attiva la sanità mentale
- Dialogo per passare a scena3 oppure una richiesta di conferma?
- **Tests** - Il lupo: Il lupo si è fermato nella posizione corretta, il problema degli alberi possiamo lasciarlo per il lupo secondo me XD
- Tooltip non compare dopo una aver utilizzato un consumabile che conferisce salute ma la salute è al max
- Discorso con uomo baita bloccato dopo aver portato il lupo (forse dovuto all'evento del nodo -> task fucile)

## Scena 3

- Da sviluppare.

## Generali

- **salvataggio/caricamento**
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
