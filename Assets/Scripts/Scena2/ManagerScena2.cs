using UnityEngine;
using DialogueEditor;
using System.Linq;

public class ManagerScena2 : MonoBehaviour
{
    // Dati missione scena2
    [Header("Dati Missione")]
    #region Dati Missione
    public int dialoghiTotali = 5; // Numero di dialoghi totali
    public int dialoghiEseguiti; // Numero di dialoghi eseguiti 
    public int documentiTotali = 2; // Numero di documenti totali
    public int documentiRaccolti = 0; // Numero di documenti raccolti
    public int violeTotali = 15; // Numero di viole totali
    public int violeRaccolte = 0; // Numero di viole raccolte
    public bool cartelloInScena = true; // Il cartello è presente sulla barricata
    #endregion

    [Header("References")]
    #region References
    public AudioClip forestSound; // Suono della foresta
    private AudioSource audioSource; // Riferimento all'audio source
    public NPCConversation intro;
    public Diario diario;
    public Animator lupo;
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
        ConversationManager.Instance.StartConversation(intro);
    }

    void Update()
    {
        if (!lupo.GetBool("bait") && PlayerHasBait()) lupo.SetBool("bait", true);
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

    private bool PlayerHasBait()
    {
        var items = InventoryManager.instance?.items;
        return items != null && items.Any(item => item.inventorySectionType == Item.ItemType.Collectibles && item.name.Contains("Bait"));
    }

    private void AggiornaDialoghiEseguiti(string missione)
    {
        dialoghiEseguiti = diario.missioniCompletate.Count;
        diario.AggiungiMissione("Esplora la foresta (" + dialoghiEseguiti + "/" + dialoghiTotali + ")");
    }
}
