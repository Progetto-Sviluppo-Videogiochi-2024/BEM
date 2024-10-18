using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DialogueEditor;

public class Diario : MonoBehaviour
{
    public List<string> missioniAttive = new List<string>();
    public List<string> missioniCompletate = new List<string>(); // Lista per le missioni completate
    public GameObject missionPrefab;
    public Transform content;
    public GameObject scrollView;

    private bool diarioVisibile = false;

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

    public void AggiungiMissione(string missione)
    {
        if (!missioniAttive.Contains(missione))
        {
            missioniAttive.Add(missione);
            Debug.Log("Nuova missione aggiunta: " + missione);
            AggiornaDiarioUI();
        }
        else
        {
            Debug.Log("La missione è già nel diario: " + missione);
        }
    }

    public void CompletaMissione(string missione)
    {
        if (missioniAttive.Contains(missione))
        {
            missioniAttive.Remove(missione);
            missioniCompletate.Add(missione); // Aggiunge la missione alla lista delle completate
            Debug.Log("Missione completata: " + missione);
            AggiornaDiarioUI();
            //AggiornaParametriDialogo();
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

        // Se non ci sono missioni attive, mostra "Nessuna attività"
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

    // public void AggiornaParametriDialogo()
    // {
    //     foreach (string missione in missioniCompletate)
    //     {
    //         ConversationManager.Instance.SetBool(missione + "completata", true);
    //     }

    //     foreach (string missione in missioniAttive)
    //     {
    //         ConversationManager.Instance.SetBool(missione + "completata", false);
    //     }
    // }
}
