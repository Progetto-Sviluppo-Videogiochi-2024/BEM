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
    }

    public void SaveLevelData() // Salva su file
    {
        // PlayerPrefs
        data.playerPrefs.Add(new PlayerPrefsData("hasBackpack", PlayerPrefs.GetInt("hasBackpack")));
        data.playerPrefs.Add(new PlayerPrefsData("hasTorch", PlayerPrefs.GetInt("hasTorch")));
        data.playerPrefs.Add(new PlayerPrefsData("hasBait", PlayerPrefs.GetInt("hasBait")));
        data.playerPrefs.Add(new PlayerPrefsData("nTargetHit", PlayerPrefs.GetInt("nTargetHit")));
        data.playerPrefs.Add(new PlayerPrefsData("nInteractionBookShelf", PlayerPrefs.GetInt("nInteractionBookShelf")));

        // BA
        var booleanAccessor = BooleanAccessor.istance;
        if (booleanAccessor != null) data.booleanAccessor = booleanAccessor.GetBoolValues();
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
    public List<QuestData> questData = new(); // Dati delle quest
}

[Serializable]
public class PlayerPrefsData
{
    public string key;
    public int value;

    public PlayerPrefsData(string key, int value)
    {
        this.key = key;
        this.value = value;
    }
}

[Serializable]
public class QuestData
{
    public string title;
    public string description;
    public bool isCompleted;

    public QuestData(string title, string description, bool isCompleted)
    {
        this.title = title;
        this.description = description;
        this.isCompleted = isCompleted;
    }
}