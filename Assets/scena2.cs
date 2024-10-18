using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scena2 : MonoBehaviour
{
    public Diario diario; // Oggetto che si occupa di gestire le missioni
    public int dialoghiTotali = 5; // Numero di dialoghi totali
    public int documentiTotali = 2; // Numero di documenti totali
    public int documentiRaccolti = 0; // Numero di documenti raccolti
    public int dialoghiEseguiti; // Numero di dialoghi eseguiti 

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

    // Start is called before the first frame update
    void Start()
    {
        if (diario == null)
        {
            diario = GetComponent<Diario>(); // Assicurati che Diario sia inizializzato correttamente
        }

        // Assegna il numero di dialoghi eseguiti in base alle missioni completate nel diario
        dialoghiEseguiti = diario.missioniCompletate.Count;

        diario.AggiungiMissione("Esplora la foresta (Conversazioni: " + dialoghiEseguiti + "/" + dialoghiTotali + ")");
    }

    // Metodo per aggiornare i dialoghi eseguiti
    private void AggiornaDialoghiEseguiti(string missione)
    {
        dialoghiEseguiti = diario.missioniCompletate.Count;
        diario.AggiungiMissione("Esplora la foresta (Conversazioni " + dialoghiEseguiti + "/" + dialoghiTotali + ")");
    }
}
