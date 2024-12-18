using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class GameData : ISerializationCallbackReceiver
{
    public string fileName; // Nome del file di salvataggio
    public string currentSceneName; // Nome della scena attuale
    [NonSerialized] public DateTime saveTime; // Data e ora del salvataggio in formato DateTime
    [SerializeField] private string saveTimeString; // Data e ora del salvataggio in formato stringa
    public int nSlotSave; // Numero dello slot di salvataggio
    public PlayerData playerData; // Dati del giocatore (stato, trasform, vcam)
    public InventoryData inventoryData; // Dati dell'inventario (oggetti raccolti) // Oggetti equipaggiati?
    public LevelData levelData; // Dati del livello (PlayerPrefs, BA) // Dati livello? // Dati importanti della scena attuale per le statistiche
    public List<string> collectedItemIds; // ID degli oggetti raccolti o interagiti

    public void OnBeforeSerialize() => saveTimeString = saveTime.ToString("yyyy-MM-dd HH:mm:ss"); // Formato: "yyyy-MM-dd HH:mm:ss" // Imposta saveTime da saveTimeString durante la deserializzazione

    public void OnAfterDeserialize() // Riconverte saveTimeString in DateTime durante la deserializzazione
    {
        if (!string.IsNullOrEmpty(saveTimeString)) saveTime = DateTime.Parse(saveTimeString);
    }
}

public interface ISaveable
{
    SerializableGuid Id { get; set; }
}

public interface IBind<TData> where TData : ISaveable
{
    SerializableGuid Id { get; set; }
    void Bind(TData data);
}

public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem>
{
    [SerializeField] public GameData gameData;
    [HideInInspector] public IDataService dataService;
    private bool isLoading = false;

    protected override void Awake()
    {
        base.Awake();
        dataService = new FileDataService(new JsonSerializer());
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu" || scene.name == "Transizione") return;
        if (!isLoading) return;
        Time.timeScale = 1;
        Bind<GameCharacter, PlayerData>(gameData.playerData);
        Bind<GameInventory, InventoryData>(gameData.inventoryData);
        Bind<GameLevelData, LevelData>(gameData.levelData);
    }

    void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
    {
        var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
        if (entity != null)
        {
            data ??= new TData { Id = entity.Id }; // Lo assegna sse data è NULL
            entity.Bind(data);
        }
    }

    void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new() // TODO: non so dove usarlo
    {
        var entities = FindObjectsByType<T>(FindObjectsSortMode.None);
        foreach (var entity in entities)
        {
            var data = datas.FirstOrDefault(d => d.Id == entity.Id);
            if (data == null)
            {
                data = new TData { Id = entity.Id };
                datas.Add(data);
            }
            entity.Bind(data);
        }
    }

    public void NewGame()
    {
        gameData = new GameData
        {
            fileName = $"Slot {dataService.ListSaves().Count() + 1}", // TODO: forse da cambiare in "Slot 1" o togliere proprio ?
            currentSceneName = "Scena0"
        };
        isLoading = false;
        SceneManager.LoadScene(gameData.currentSceneName);
    }

    public void SaveGame(int nSlotSave)
    {
        gameData.fileName = $"Slot {nSlotSave}";
        gameData.saveTime = DateTime.Now;
        gameData.currentSceneName = SceneManager.GetActiveScene().name;
        gameData.nSlotSave = nSlotSave;

        var player = FindObjectsByType<GameCharacter>(FindObjectsSortMode.None).FirstOrDefault();
        if (player != null)
        {
            player.SavePlayerData();
            gameData.playerData = player.data;
        }

        var inventory = FindObjectsByType<GameInventory>(FindObjectsSortMode.None).FirstOrDefault();
        if (inventory != null)
        {
            inventory.SaveInventoryData();
            gameData.inventoryData = inventory.data;
        }

        var level = FindObjectsByType<GameLevelData>(FindObjectsSortMode.None).FirstOrDefault();
        if (level != null)
        {
            level.SaveLevelData();
            gameData.levelData = level.data;
        }
        gameData.collectedItemIds = GestoreScena.collectedItemIds;

        dataService.Save(gameData);
    }

    public void LoadGame(string fileName)
    {
        gameData = dataService.Load(fileName);

        if (string.IsNullOrWhiteSpace(gameData.currentSceneName)) gameData.currentSceneName = "Scena0";
        isLoading = true;
        SceneManager.LoadScene(gameData.currentSceneName);
    }

    public void ReloadGame() => LoadGame(gameData.fileName); // TODO: non so dove usarlo

    public void DeleteGame(string fileName) => dataService.Delete(fileName);
}
