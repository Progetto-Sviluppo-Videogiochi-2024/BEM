using UnityEngine;
using DialogueEditor;

public class ManagerScena2 : MonoBehaviour
{
    [Header("Dati Scena2")]
    #region Dati Scena2
    public int dialoghiTotali = 5; // Numero di dialoghi totali
    public int dialoghiEseguiti; // Numero di dialoghi eseguiti 
    public int documentiTotali = 2; // Numero di documenti totali
    public int documentiRaccolti = 0; // Numero di documenti raccolti
    public int violeTotali = 15; // Numero di viole totali
    public int violeRaccolte = 0; // Numero di viole raccolte
    #endregion

    [Header("Settings")]
    #region Settings
    private bool clickEndHandled = false; // Flag per evitare che esegua più volte il codice nel metodo Update quando si clicca su "End"
    private bool hasFlowers = false; // Flag per i fiori
    #endregion

    [Header("References")]
    #region References
    public AudioClip forestSound; // Suono della foresta
    private AudioSource audioSource; // Riferimento all'audio source
    public NPCConversation intro;
    public Diario diario;
    public Animator lupo;
    public BooleanAccessor booleanAccessor;
    #endregion

    // Dati Inventario scena2
    // TODO: Inserire i dati dell'inventario @ccorvino3 @marcoWarrior

    void Start()
    {
        // Inizializza l'audio source
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = forestSound;
        audioSource.loop = true;
        audioSource.Play();

        // Assegna il numero di dialoghi eseguiti in base alle missioni completate nel diario
        dialoghiEseguiti = diario.missioniCompletate.Count;
        diario.AggiungiMissione("Esplora la foresta (" + dialoghiEseguiti + " / " + dialoghiTotali + ")");

        // Avvia la conversazione iniziale
        GestoreScena.ChangeCursorActiveStatus(true, "ManagerScena2.start"); // false quando hasClickedEnd è true
        ConversationManager.Instance.StartConversation(intro);
    }

    void Update()
    {
        if (!clickEndHandled && ConversationManager.Instance.hasClickedEnd)
        {
            clickEndHandled = true;
            ConversationManager.Instance.hasClickedEnd = false;
            GestoreScena.ChangeCursorActiveStatus(false, "ManagerScena2.update");
        }
        
        var boolAccessor = BooleanAccessor.istance;
        if (!hasFlowers && boolAccessor.GetBoolFromThis("fiori") && InventoryManager.instance.GetQtaItem("Viola") >= 3)
        { hasFlowers = true; boolAccessor.SetBoolOnDialogueE("fioriRaccolti"); }
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
