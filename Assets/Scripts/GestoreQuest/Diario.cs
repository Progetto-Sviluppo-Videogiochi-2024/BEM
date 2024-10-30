using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DialogueEditor;

public class Diario : MonoBehaviour
{
    public List<string> missioniAttive = new List<string>();
    public List<string> missioniCompletate = new List<string>();
    public GameObject missionPrefab;
    public Transform content;
    public GameObject scrollView;

    private bool diarioVisibile = false;

    // Definizione del delegato
    public delegate void MissionCompletedDelegate(string missione);
    public event MissionCompletedDelegate OnMissionCompleted;

    void Start()
    {
        if (scrollView != null)
        {
            scrollView.SetActive(false);
        }
        else
        {
            Debug.LogError("ScrollView non assegnato!");
        }

        AggiornaDiarioUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            diarioVisibile = !diarioVisibile;
            if (scrollView != null)
            {
                scrollView.SetActive(diarioVisibile);
            }
        }
    }

    // Metodo modificato per gestire missioni duplicate
    public void AggiungiMissione(string missione)
    {
        // Cerca una missione esistente nella lista delle attive
        string missioneBase = missione.Split('(')[0].Trim(); // Ottieni solo la parte del titolo, escludendo i numeri
        
        // Controlla se la missione è già presente nelle missioni attive
        for (int i = 0; i < missioniAttive.Count; i++)
        {
            if (missioniAttive[i].StartsWith(missioneBase))
            {
                missioniAttive[i] = missione; // Aggiorna la missione esistente
                AggiornaDiarioUI(); // Aggiorna l'interfaccia utente
                return;
            }
        }

        // Controlla se la missione è già stata completata
        for (int i = 0; i < missioniCompletate.Count; i++)
        {
            if (missioniCompletate[i].StartsWith(missioneBase))
            {
                Debug.Log("La missione '" + missioneBase + "' è già stata completata.");
                return; // Non aggiunge la missione se è già completata
            }
        }

        // Se la missione non è presente né nelle attive né nelle completate, la aggiunge
        missioniAttive.Add(missione);
        // Debug.Log("Nuova missione aggiunta: " + missione);
        AggiornaDiarioUI();
    }


    public void CompletaMissione(string missione)
    {
        if (missioniAttive.Contains(missione))
        {
            missioniAttive.Remove(missione);
            missioniCompletate.Add(missione);
            Debug.Log("Missione completata: " + missione);
            AggiornaDiarioUI();

            // Invoca l'evento per notificare il completamento della missione
            OnMissionCompleted?.Invoke(missione);
        }
        else
        {
            Debug.Log("La missione non è presente nel diario.");
        }
    }

    private void AggiornaDiarioUI()
    {
        if (content == null || missionPrefab == null)
        {
            Debug.LogError("Content o MissionPrefab non assegnati!");
            return;
        }

        // Rimuove tutti gli elementi esistenti
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Aggiunge le missioni completate prima
        foreach (string missione in missioniCompletate)
        {
            GameObject missionObj = Instantiate(missionPrefab, content);
            TMP_Text missionText = missionObj.GetComponent<TMP_Text>();
            if (missionText != null)
            {
                missionText.text = "<s>- " + missione + "</s>"; // Applicazione dello strikethrough per le completate
            }
            else
            {
                Debug.LogError("Prefab di missione non ha un componente TMP_Text!");
            }
        }

        // Aggiunge le missioni attive
        foreach (string missione in missioniAttive)
        {
            GameObject missionObj = Instantiate(missionPrefab, content);
            TMP_Text missionText = missionObj.GetComponent<TMP_Text>();
            if (missionText != null)
            {
                missionText.text = "- " + missione;
            }
            else
            {
                Debug.LogError("Prefab di missione non ha un componente TMP_Text!");
            }
        }

        // Se non ci sono missioni attive o completate, mostra "Nessuna attività"
        if (missioniAttive.Count == 0 && missioniCompletate.Count == 0)
        {
            GameObject missionObj = Instantiate(missionPrefab, content);
            TMP_Text missionText = missionObj.GetComponent<TMP_Text>();
            if (missionText != null)
            {
                missionText.text = "Nessuna attività";
            }
            else
            {
                Debug.LogError("Prefab di missione non ha un componente TMP_Text!");
            }
        }
    }
}
