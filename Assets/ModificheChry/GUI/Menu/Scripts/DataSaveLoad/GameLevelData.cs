using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

        var characters = GameObject.FindGameObjectsWithTag("NPCDynamic");
        foreach (var character in characters)
        {
            var characterData = data.characters.Find(c => c.nameCharacter == $"{character.name}_{SceneManager.GetActiveScene().name}");
            if (characterData != null)
            {
                character.transform.SetPositionAndRotation(characterData.position, characterData.rotation);
                character.GetComponent<Animator>().Play(characterData.currentAnimation);
            }
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

        // Personaggi dinamici
        var characters = GameObject.FindGameObjectsWithTag("NPCDynamic");
        data.characters.Clear(); // Svuoto la lista per evitare duplicati
        foreach (var character in characters)
        {
            data.characters.Add(
                new CharacterData(
                    $"{character.name}_{SceneManager.GetActiveScene().name}",
                    "Movement", // Richiede che speed = 0, per la sit del lupo
                    character.transform.position,
                    character.transform.rotation)
            );
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
    public List<CharacterData> characters = new(); // Lista per memorizzare i dati dei personaggi dinamici

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

[Serializable]
public class CharacterData
{
    public string nameCharacter; // Nome del personaggio dinamico
    public string currentAnimation; // Animazione corrente del personaggio
    public Vector3 position; // Posizione del personaggio
    public Quaternion rotation; // Rotazione del personaggio

    public CharacterData(string nameCharacter, string currentAnimation, Vector3 position, Quaternion rotation)
    {
        this.nameCharacter = nameCharacter;
        this.currentAnimation = currentAnimation;
        this.position = position;
        this.rotation = rotation;
    }
}
