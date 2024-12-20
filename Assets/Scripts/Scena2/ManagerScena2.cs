using UnityEngine;
using DialogueEditor;

public class ManagerScena2 : MonoBehaviour
{
    [Header("Dati Scena2")]
    #region Dati Scena2
    private const int dialoghiTotali = 6; // Numero di dialoghi totali
    private int dialoghiEseguiti; // Numero di dialoghi eseguiti 
    // private const int documentiTotali = 7; // Numero di documenti totali
    // private int documentiRaccolti = 0; // Numero di documenti raccolti
    // private const int violeTotali = 15; // Numero di viole totali
    // private int violeRaccolte = 0; // Numero di viole raccolte
    // private const int consumablesTotali = 2; // Numero di oggetti consumabili totali
    // private int ammosTotali = 1; // Numero di munizioni totali
    #endregion

    [Header("Settings")]
    #region Settings
    private bool flagNextScene = false; // Flag per cambiare scena
    private bool hasFlowers = false; // Flag per i fiori
    #endregion

    [Header("References")]
    #region References
    public AudioClip forestSound; // Suono della foresta
    private AudioSource audioSource; // Riferimento all'audio source
    public NPCConversation intro; // Conversazione iniziale di scena2 (dove sono gli altri)
    public Diario diario; // Riferimento al diario
    public Animator lupo; // Riferimento all'animatore del lupo
    public BooleanAccessor booleanAccessor; // Riferimento al booleanAccessor
    public RadioManager radioManager; // Riferimento al radio manager
    #endregion

    // Dati Inventario scena2
    // TODO: Inserire i dati dell'inventario @ccorvino3 @marcoWarrior

    void Start()
    {
        booleanAccessor = BooleanAccessor.istance;

        //Setta la radio come spenta

        // Inizializza l'audio source
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = forestSound;
        audioSource.volume = 0.2f;
        audioSource.loop = true;
        audioSource.Play();

        // Assegna il numero di dialoghi eseguiti in base alle missioni completate nel diario
        dialoghiEseguiti = diario.missioniCompletate.Count;
        diario.AggiungiMissione("Esplora la foresta (" + dialoghiEseguiti + " / " + dialoghiTotali + ")");

        // Avvia la conversazione iniziale
        if (!booleanAccessor.GetBoolFromThis("intro2"))
        {
            GestoreScena.ChangeCursorActiveStatus(true, "ManagerScena2.start"); // false quando hasClickedEnd è true
            ConversationManager.Instance.StartConversation(intro);
        }
    }

    void Update()
    {
        if (!booleanAccessor.GetBoolFromThis("intro2") && ConversationManager.Instance.hasClickedEnd)
        {
            SetDEBool("intro2");
            ConversationManager.Instance.hasClickedEnd = false;
            GestoreScena.ChangeCursorActiveStatus(false, "ManagerScena2.update");
        }

        if (!hasFlowers && booleanAccessor.GetBoolFromThis("fiori") && InventoryManager.instance.GetQtaItem("Viola") >= 3)
        { hasFlowers = true; booleanAccessor.SetBoolOnDialogueE("fioriRaccolti"); }

        if (!flagNextScene && CanGoNextScene())
        {
            flagNextScene = true;
            diario.AggiungiMissione("Oltrepassa la recinzione");
        }

        if (dialoghiEseguiti == dialoghiTotali) diario.CompletaMissione("Esplora la foresta (" + dialoghiEseguiti + " / " + dialoghiTotali + ")");
    }

    private void OnEnable()
    {
        // Assicurati di registrarti all'evento quando lo script è abilitato
        if (diario != null) diario.OnMissionCompleted += AggiornaDialoghiEseguiti;
    }

    private void OnDisable()
    {
        // Assicurati di disregistrarti dall'evento quando lo script è disabilitato
        if (diario != null) diario.OnMissionCompleted -= AggiornaDialoghiEseguiti;
    }

    public bool CanGoNextScene() =>
        booleanAccessor.GetBoolFromThis("cocaColaDone") && // Se ha completato task fire
        booleanAccessor.GetBoolFromThis("cartelloDone") && // Se ha completato task cartello
        booleanAccessor.GetBoolFromThis("soluzione"); // Se ha completato task fiori (incluso anche il crafting)

    private void AggiornaDialoghiEseguiti(string missione)
    {
        dialoghiEseguiti = diario.missioniCompletate.Count;
        diario.AggiungiMissione("Esplora la foresta (" + dialoghiEseguiti + " / " + dialoghiTotali + ")");
    }

    public void SetDEBool(string nomeBool) // Da invocare nel DialogueEditor per settare i valori booleani del BooleanAccessor
    {
        booleanAccessor.SetBoolOnDialogueE(nomeBool);
    }
}
