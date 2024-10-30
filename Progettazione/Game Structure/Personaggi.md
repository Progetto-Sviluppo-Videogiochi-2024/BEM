## Player
### Attributi
- **Salute**: Comprensibili dal giocatore e dalla GUI.
- **Stamina**: Comprensibili dal giocatore e dalla GUI.
- **Inventario**: Lista di oggetti che il personaggio ha nello zaino.
  - Gli oggetti possono essere utilizzabili o equipaggiamento.
  - Gli oggetti utilizzabili possono essere usati per curare il personaggio, aumentare la stamina e la sanità mentale.
- **Sanità mentale**: Audio e dipende dagli HP del personaggio.
- **Equipaggiamento**: Lista di oggetti che il personaggio ha equipaggiato.
  - Esempi di oggetti utilizzabili: Cibo, bevande, medicinali, sigarette, armi da fuoco, ecc.
- **Azioni**: Lista di azioni che il personaggio può compiere.
  - Esempi: Attacco, Corri, Cammina, Ispeziona, Zaino (Raccogli e usa oggetto, Equipaggia oggetto...), ecc.

### Attacchi

## Mostri
### Attributi
- **Salute**: Indica la barra di vita del mostro (invisibile al personaggio).
- **Attacco**: Indica la forza dell'attacco con cui il mostro attacca il personaggio.
- **Difesa**: Indica la resistenza del mostro agli attacchi del personaggio.
- **Velocità**: Indica la velocità di movimento del mostro.
- **Abilità**: Lista di abilità speciali che il mostro può utilizzare durante gli scontri.
  - Esempi: Attacco a distanza, rigenerazione della salute, ecc.
- **Ricompensa**: Indica la ricompensa (es. munizioni) che il personaggio riceve dopo aver sconfitto il mostro.
- **Comportamento**: Indica il comportamento del mostro durante gli scontri.
  - Esempi: Attacco aggressivo, difesa, fuga, attacco a distanza, ecc.
- **Livello**: Indica il livello di difficoltà del mostro.
  - I mostri di livello più alto sono più forti e offrono ricompense migliori.
- **Immagine**: Indica l'immagine o l'icona che rappresenta il mostro nel gioco.
- **Suono**: Indica il suono associato al mostro (es. grugnito, urla, ecc.).
- **Animazione**: Indica l'animazione associata al mostro (es. movimenti, attacchi, ecc.).

### Attacchi
#### Maynard (Scarso)
- **Calcio o Spinta Violenta**: Quando il giocatore si avvicina troppo, il mostro lo respinge con una spinta o un calcio per mantenere la distanza.
- **Affondo Spettrale**: Un attacco rapido in cui il mostro scatta verso il giocatore, coprendo una breve distanza in un lampo, lasciando un effetto visivo spettrale per creare un senso di inquietudine.

#### Artiglio (Medio)
- **Artigliata Ravvicinata**: Il mostro attacca con una rapida artigliata, infliggendo danni diretti al giocatore quando è a distanza ravvicinata.
- **Morso Devastante**: Un attacco potente in cui il mostro cerca di mordere il giocatore, causando danni elevati. Può essere usato come attacco raro e più pericoloso.
- **Attacco Frenetico**: Entra in uno stato di furia, eseguendo una serie rapida di attacchi con gli artigli che durano per un paio di secondi. Questo potrebbe attivarsi quando è a bassa salute.

#### ThrowBall (BossDemo)
- **Ruggito Intimidatorio**: Un attacco non fisico che provoca paura nel giocatore, rallentandolo temporaneamente o riducendo la sua visibilità. Questo può avere un effetto di status.
  - *Nota*: Forse anche ad Artiglio???
- **Colpo di Codata** (se applicabile): Se il mostro ha una coda lunga o robusta, può usarla per colpire il giocatore lateralmente, causando danni e spingendolo indietro.
  - *Nota*: Questo attacco potrebbe attivarsi quando il mostro ha solo 1/3 o 1/4 degli HP.
- **Vomito Tossico**: Lancia una sostanza velenosa verso il giocatore, che causa danni continuativi (DoT - damage over time) o confonde la visuale del giocatore.
  - *Nota*: Questo implica aggiungere effetti di stato al gioco, quindi potrebbe non essere implementato.
- **Balzo**: Salta verso il giocatore da una distanza moderata, cercando di colpirlo con tutto il peso del corpo per fare danno e stordirlo.
  - *Nota*: Dopo il balzo, il nemico potrebbe fare un salto indietro di diversi metri per dare al giocatore il tempo di riprendersi.

### Attacchi Scartati ?
- **Attacco a Distanza con Lance d'Ossa**: Il mostro lancia schegge ossee o spine dalla sua pelle verso il giocatore, infliggendo danni a distanza.
