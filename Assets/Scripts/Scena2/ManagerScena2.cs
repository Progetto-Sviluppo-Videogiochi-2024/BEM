using UnityEngine;
using DialogueEditor;


public class ManagerScena2 : MonoBehaviour
{
    public NPCConversation intro; // Tutorial smartphone
    public Diario diario; 
    

    // Dati missione scena2
    public int dialoghiTotali = 5; // Numero di dialoghi totali
    public int dialoghiEseguiti; // Numero di dialoghi eseguiti 
    public int documentiTotali = 2; // Numero di documenti totali
    public int documentiRaccolti = 0; // Numero di documenti raccolti

    public int violeTotali = 15; // Numero di viole totali
    public int violeRaccolte = 0; // Numero di viole raccolte
    public bool cartelloInScena = true; // Il cartello è presente sulla barricata

    // Dati Inventario scena2
    //To do: Inserire i dati dell'inventario

    // Start is called before the first frame update
    void Start()
    {
        // Se non è stato assegnato un diario, cerca di trovarlo
        if (diario == null)
        {
            diario = GetComponent<Diario>(); 
        }

        // Assegna il numero di dialoghi eseguiti in base alle missioni completate nel diario
        dialoghiEseguiti = diario.missioniCompletate.Count;
        diario.AggiungiMissione("Esplora la foresta (" + dialoghiEseguiti + "/" + dialoghiTotali + ")");

        // Avvia la conversazione iniziale
        //ConversationManager.Instance.StartConversation(intro);


    }


    private void OnEnable()
    {
        // Assicurati di registrarti all'evento quando lo script è abilitato
        if (diario != null)
        {
            diario.OnMissionCompleted += AggiornaDialoghiEseguiti;
        }
    }

    private void OnDisable()
    {
        // Assicurati di disregistrarti dall'evento quando lo script è disabilitato
        if (diario != null)
        {
            diario.OnMissionCompleted -= AggiornaDialoghiEseguiti;
        }
    }

    // Metodo per aggiornare i dialoghi eseguiti
    private void AggiornaDialoghiEseguiti(string missione)
    {
        dialoghiEseguiti = diario.missioniCompletate.Count;
        diario.AggiungiMissione("Esplora la foresta (" + dialoghiEseguiti + "/" + dialoghiTotali + ")");
    }
}
