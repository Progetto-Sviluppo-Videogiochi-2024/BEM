# **BUG / DIFETTI:**  

## Scena0
- Dopo che il **pg** si alza, dovrebbe guardare verso il letto, non verso la finestra (evitare la retromarcia del pg).  
## Scena1
- **JumpoScare**: Inserirne uno alla stanza adiacente se abbiamo tempo
## Scena2 
- **Task di sparare col fucile:**  
  - Mancano **rig layer**, dialoghi e controlli.   
- Manca l'**ammo del fucile** di Scena2 (potremmo usare anche la stessa per ogni arma).
- **Salute Mentale**
  - Implementare respiro affanoso per Stefano quando si attiva la sanità mentale
- Dialogo per passare a scena3 oppure una richiesta di conferma?
- **Tests**
  - Test: 28.11.24 
      - **Grave**: Senza aver parlato con Jacob, mi sono recato al cartello correndo, ho premuto spacebar si è avviato il dialogo ma il pg non poteva più muoversi.
      Ovvero, continuava a correre, devo ritestare. 
      Dobbiamo verificare se accade anche in scena0
      - Il lupo: Il lupo si è fermato nella posizione corretta, il problema degli alberi possiamo lasciarlo per il lupo secondo me XD

## Scena 3 
- Da sviluppare. 

## Generali
**salvataggio/caricamento**
  - Da implementare 
**Sezioni MainMenu e GPM**
  - Da implementare (esempio Opzioni ecc) inoltre bisogna cambiare il nome di alcuni e scriverli in italiano, eccetto "Play Game" o altri comunemente usati 
- **Cursore** 
  - Sembra funzionare bene per ora.
- **AI di NPC e villain** 
  - Da completare, controllare la sezione "Vincoli".
- **Gestione della camera:**  
  - Stefano: movimento, crouch, aim, **ALT**. 
**Offset animazioni**
  - Da verificare con Christian 
- **Rompicapi** 
  - da implementare. 
- **Gestione dell'audio:**:

# **DA DECIDERE INSIEME:**  
- Estetica di:  
  - **Menu main**, **menu pausa**, **gameplay menu**.: Migliorare l'estetica se necessario una volta finito il game   
- Definire e implementare le **meccaniche core**.  (In scena2 sono presenti quasi tutte, tranne il crouch in modo esplicito)
- Creare il **documento di game design**.  
- Realizzare una presentazione in **PowerPoint**.  
- Ricontrollare a fine gioco se i file progettazione presentano tutti gli aspetti del gioco (se vanno consegnati al prof)
--- 
