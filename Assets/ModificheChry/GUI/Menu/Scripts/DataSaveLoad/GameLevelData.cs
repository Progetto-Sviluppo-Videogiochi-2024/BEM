using UnityEngine;
using System;
using System.Collections.Generic;

public class GameLevelData : MonoBehaviour, IBind<LevelData>
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid(); // ID univoco del livello
    [SerializeField] public LevelData data; // Dati del livello

    public void Bind(LevelData data) // Carica da file
    {
        this.data = data;
        this.data.Id = Id;

        // PlayerPrefs
        foreach (var prefs in data.playerPrefs)
        {
            PlayerPrefs.SetInt(prefs.key, prefs.value);
        }
        PlayerPrefs.Save();

        // BA
        var booleanAccessor = BooleanAccessor.istance;
        booleanAccessor?.SetBoolValues(data.booleanAccessor);

        // Quest
        var diario = FindObjectOfType<Diario>();
        if (diario != null)
        {
            diario.missioniAttive = new(data.questAttive);
            diario.missioniCompletate = new(data.questCompletate);
        }
    }

    public void SaveLevelData() // Salva su file
    {
        // PlayerPrefs
        data.playerPrefs.Clear(); // Svuoto la lista per evitare duplicati
        data.playerPrefs.Add(new PlayerPrefsData("hasBackpack", PlayerPrefs.GetInt("hasBackpack")));
        data.playerPrefs.Add(new PlayerPrefsData("hasTorch", PlayerPrefs.GetInt("hasTorch")));
        data.playerPrefs.Add(new PlayerPrefsData("hasBait", PlayerPrefs.GetInt("hasBait")));
        data.playerPrefs.Add(new PlayerPrefsData("nTargetHit", PlayerPrefs.GetInt("nTargetHit")));
        data.playerPrefs.Add(new PlayerPrefsData("nInteractionBookShelf", PlayerPrefs.GetInt("nInteractionBookShelf")));

        // BA
        var booleanAccessor = BooleanAccessor.istance;
        if (booleanAccessor != null) data.booleanAccessor = booleanAccessor.GetBoolValues();

        // Quest
        var diario = FindObjectOfType<Diario>();
        if (diario != null)
        {
            data.questAttive = new(diario.missioniAttive);
            data.questCompletate = new(diario.missioniCompletate);
        }
    }
}

[Serializable]
public class LevelData : ISaveable
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid(); // ID univoco del livello

    [Header("Dati del PlayerPrefs")]
    public List<PlayerPrefsData> playerPrefs = new(); // Lista per memorizzare i dati di PlayerPrefs

    [Header("Dati livello")]
    // TODO: Dati importanti della scena attuale per le statistiche

    [Header("Dati del BA")]
    public List<BoolData> booleanAccessor = new(); // Dati booleani del BA

    [Header("Dati delle quest")]
    public List<string> questAttive = new(); // Lista per memorizzare le quest attive
    public List<string> questCompletate = new(); // Lista per memorizzare le quest completate
}

[Serializable]
public class PlayerPrefsData
{
    public string key; // Nome assegnato alla chiave di PlayerPrefs
    public int value; // Valore assegnato alla chiave di PlayerPrefs

    public PlayerPrefsData(string key, int value)
    {
        this.key = key;
        this.value = value;
    }
}
