using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Diario : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    private bool diarioVisibile = false;
    #endregion

    [Header("Structure Data")]
    #region Structure Data
    public List<string> missioniAttive = new();
    public List<string> missioniCompletate = new();
    #endregion

    [Header("UI Data")]
    #region UI Data
    public GameObject missionPrefab;
    public Transform content;
    public GameObject scrollView;
    #endregion

    // Eventi delegati
    #region Events
    public delegate void MissionCompletedDelegate(string missione);
    public event MissionCompletedDelegate OnMissionCompleted;
    #endregion

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
        if (diarioVisibile) { ToggleCinematic(); }
        if (Input.GetKeyDown(KeyCode.Q)) ToggleDiario(!diarioVisibile);
    }

    private void ToggleDiario(bool isOpen)
    {
        diarioVisibile = isOpen;
        scrollView.SetActive(diarioVisibile);
        GestoreScena.ChangeCursorActiveStatus(diarioVisibile, "Diario");
        FindAnyObjectByType<Player>().GetComponent<AimStateManager>().enabled = true;
    }

    private void ToggleCinematic()
    {
        // Se il mouse è sopra un UI, disabilita la visuale
        bool isCursorOverUI = EventSystem.current.IsPointerOverGameObject();
        FindAnyObjectByType<Player>().GetComponent<AimStateManager>().enabled = !isCursorOverUI;
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
                print("La missione '" + missioneBase + "' è già stata completata.");
                return; // Non aggiunge la missione se è già completata
            }
        }

        // Se la missione non è presente né nelle attive né nelle completate, la aggiunge
        missioniAttive.Add(missione);
        // print("Nuova missione aggiunta: " + missione);
        AggiornaDiarioUI();
    }

    public void CompletaMissione(string missione)
    {
        if (missioniAttive.Contains(missione))
        {
            missioniAttive.Remove(missione);
            missioniCompletate.Add(missione);
            // print("Missione completata: " + missione);
            AggiornaDiarioUI();

            // Invoca l'evento per notificare il completamento della missione
            OnMissionCompleted?.Invoke(missione);
        }
        else
        {
            // print("La missione non è presente nel diario.");
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
            SetMissione("<s>- " + missione + "</s>"); // Applicazione dello strikethrough per le completate
        }

        // Aggiunge le missioni attive
        foreach (string missione in missioniAttive)
        {
            SetMissione("- " + missione);
        }

        // Se non ci sono missioni attive o completate, mostra "Nessuna attività"
        if (missioniAttive.Count == 0 && missioniCompletate.Count == 0)
        {
            SetMissione("Nessuna attività");
        }
    }

    private void SetMissione(string missione)
    {
        GameObject missionObj = Instantiate(missionPrefab, content);
        TMP_Text missionText = missionObj.GetComponent<TMP_Text>();
        if (missionText != null)
        {
            missionText.text = missione;
        }
        else
        {
            Debug.LogError("Prefab di missione non ha un componente TMP_Text!");
        }
    }
}
