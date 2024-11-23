using DialogueEditor;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class TaskFucile : MonoBehaviour
{
    private ConversationManager conversationManager; // Riferimento al ConversationManager
    public GameObject fucile; // Riferimento al fucile
    public NPCConversation alert; // Dialogo di alert se il player collide col "muro" invisibile
    private bool playerInRange = false; // Indica se è vicino al tronco
    private bool playerInCollision = false; // Indica se ha colliso con il "muro" invisibile
    [HideInInspector] public GameObject areaTask; // Riferimento a dove il player può raccogliere il fucile e iniziare il tutorial
    private const int maxTargetHit = 3; // Numero di bottiglie da colpire per completare il tutorial

    void Start()
    {
        // Lascio l'aura attiva per far capire che è interagibile solo dopo la quest del lupo
        fucile.GetComponent<ItemPickup>().enabled = false;
        areaTask = transform.GetChild(0).gameObject;
        areaTask.SetActive(false);
        PlayerPrefs.SetInt("nTargetHit", 0);
        PlayerPrefs.Save();
    }

    void Update()
    {
        // Controlli preliminari per poter attivare il tutorial del fucile
        if (conversationManager.GetBool("cocaColaDone")) { enabled = false; return; }
        if (!playerInRange || !conversationManager.GetBool("wolfDone")) return;
        fucile.GetComponent<ItemPickup>().enabled = true;
        if (!HasPlayerFucile()) return;

        // Comincia la task del fucile
        // Togliere il bordo
        fucile.SetActive(false);
        fucile.GetComponent<ItemPickup>().enabled = false; // Controllare se riguarda l'arma clonata o quella originale
        areaTask.GetComponent<BoxCollider>().isTrigger = false;

        if (PlayerPrefs.GetInt("nTargetHit") == maxTargetHit)
        {
            conversationManager.SetBool("cocacolaDone", true);
            fucile.SetActive(true);
            areaTask.GetComponent<BoxCollider>().isTrigger = true;
            // Togliere l'arma dall'inventario (considerare l'arma clonata)
            // Distruggere il playerprefs se non serve più nel gioco
            // OPPURE qualcosa per le statistiche di scena ? (es. "Stefano ha sparato 3 bottiglie con 3 colpi")
        }
    }

    public bool HasPlayerFucile() => InventoryManager.instance.items.Find(weapon => weapon.name == "Fucile da caccia");

    public void VisibleAreaPickup() => areaTask.SetActive(true); // Invocato dal DE di UomoBaita2

    private void PlayerTrigger(Collider other, bool _playerInRange)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = _playerInRange;
        }
    }

    private void PlayerCollision(Collision other, bool _playerInCollision)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInCollision = _playerInCollision;
            ConversationManager.Instance.StartConversation(alert);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerTrigger(other, true);
    }

    void OnTriggerExit(Collider other)
    {
        PlayerTrigger(other, false);
    }

    void OnCollisionEnter(Collision other)
    {
        PlayerCollision(other, true);
    }

    void OnCollisionExit(Collision other)
    {
        PlayerCollision(other, false);
    }
}

// Dopo aver concluso la missione del lupo:

// Vincoliamo il giocatore a seguire il tutorial:

// - L'uomo Baita dice a Stefano di raccogliere il fucile
// - Appena lo fa, compaiono 4 muri invisibili, tipo area trigger, se stefano Collide con essi, viene avviato un dialogo dell'uomo baita che dice "Non barare ragazzo, resta vicino al tronco".

// - Nel fucile inseriamo solo 10 proiettili, nessun limite di tempo, lui deve apprendere la meccanica del combattimento a distanza.

// - Il tutorial termina quando le 3 bottiglie sono state sparate OPPURE quando ha finito i proiettili del fucile.

// - Quando il tutorial termina, i muri invisibili vengono disattivati, l'arma si posiziona sul tronco, senza aura e senza pick-up, per far comprendere che non è interagibile, quindi il tutorial avviene solo una volta.
// Tanto se ha sparato 10 proiettili bene o male ha capito come si spara.

// - Ricompensa: Se spara 3 bottiglie e restano 7 proiettili significa che ha sparato con successo le 3 bottiglie con i primi 3 colpi, in tal caso gli conferiamo una ricompensa, altrimenti niente.

