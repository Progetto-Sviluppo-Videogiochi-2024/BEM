using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GestoreGioco : MonoBehaviour
{
    public static GestoreGioco Instance { get; private set; }

    [Header("Game Data")]
    public int livelloCorrente; // Livello attuale del gioco
    public int fioriRaccolti; // Numero totale di fiori raccolti (esempio)
    public Dictionary<string, int> progressoLivello = new(); // Dati di progresso specifici

    private string saveFilePath; // Percorso del file di salvataggio

    private void Awake()
    {
        // Implementa Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");
            CaricaDati(); // Carica i dati salvati all'avvio
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Metodo per aggiornare il progresso del livello
    public void AggiornaProgressoLivello(string nomeLivello, int oggettiRaccolti)
    {
        if (progressoLivello.ContainsKey(nomeLivello))
            progressoLivello[nomeLivello] = oggettiRaccolti;
        else
            progressoLivello.Add(nomeLivello, oggettiRaccolti);

        SalvaDati(); // Salva automaticamente quando aggiorniamo i progressi
    }

    // Metodo di salvataggio dei dati
    public void SalvaDati()
    {
        GameData data = new()
        {
            livelloCorrente = livelloCorrente,
            fioriRaccolti = fioriRaccolti,
            progressoLivello = progressoLivello
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Dati salvati con successo");
    }

    // Metodo di caricamento dei dati
    public void CaricaDati()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);

            livelloCorrente = data.livelloCorrente;
            fioriRaccolti = data.fioriRaccolti;
            progressoLivello = data.progressoLivello;

            Debug.Log("Dati caricati con successo");
        }
        else
        {
            Debug.LogWarning("Nessun file di salvataggio trovato, inizializzazione dei dati di default.");
        }
    }

    // Metodo per resettare i dati di gioco (utile per un nuovo gioco)
    public void ResetDati()
    {
        livelloCorrente = 0;
        fioriRaccolti = 0;
        progressoLivello.Clear();
        SalvaDati();
        Debug.Log("Dati di gioco resettati");
    }
}

// Classe che rappresenta i dati di gioco da salvare/caricare
[System.Serializable]
public class GameData
{
    public int livelloCorrente;
    public int fioriRaccolti;
    public Dictionary<string, int> progressoLivello; // Salva i progressi specifici dei livelli
}
