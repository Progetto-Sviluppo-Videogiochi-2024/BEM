using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestoreScena : MonoBehaviour
{
    [Header("Next Scene")]
    #region Next Scene
    public string nextChapter = "Chapter I:"; // Nome del capitolo della scena successiva
    public string nextScene = "ScenaI"; // Nome della scena successiva
    #endregion

    [Header("Static References")]
    #region Static References
    private static int nUIOpen = 0; // Numero di UI aperte nella scena corrente
    public static List<string> collectedItemIds = new(); // Lista degli ID degli oggetti raccolti o interagiti
    public static List<string> killedEnemyIds = new(); // Lista degli ID dei nemici uccisi
    public static List<string> detectingEnemies = new(); // Lista di nemici che rilevano il giocatore
    #endregion

    [Header("References")]
    #region References
    public Tooltip tooltip; // Riferimento al tooltip per gli oggetti nell'inventario
    [Tooltip("Riferimento alla testa del giocatore (per la visuale del nemico)")] public Transform playerHead; // Riferimento alla testa del giocatore (per la visuale del nemico)
    #endregion

    public List<Sprite> spritesScenes = new(); // Lista delle immagini delle scene

    void Awake()
    {
        detectingEnemies = new List<string>();
        nUIOpen = 0; // Resettata a ogni nuova scena per evitare problemi (la precedente viene distrutta, quindi, anche se alcune saranno aperte prima del cambio, io la azzero all'inizio della nuova scena)
        ToggleCursor(SceneManager.GetActiveScene().name == "MainMenu");
    }

    void Start()
    {
        if (tooltip != null) InventoryUIController.instance.tooltip = tooltip;
    }

    public string GetChapterBySceneName(string sceneName) => sceneName switch
    {
        "Scena0" => "Capitolo 1: Preparativi per l'Avventura",
        "Scena1" => "Capitolo 2: In Viaggio Verso la Foresta",
        "Scena2" => "Capitolo 3: Arrivo e Scoperte nella Foresta",
        "Scena3" => "Capitolo 4: Un'Inquietante Sorpresa",
        _ => "Capitolo: Sconosciuto" // Non dovrebbe mai accadere se le scene sono correttamente definite
    };

    public string GetChapterDescription(string sceneName) => sceneName switch
    {
        "Scena0" => "Una settimana dopo aver traslocato a Caccamone, Stefano si prepara per il lunedi di Pasquetta con i suoi amici.",
        "Scena1" => "In Viaggio Verso la Foresta",
        "Scena2" => "Il gruppo è arrivato nella Foresta di Mercadante dove iniziano ad ambientarsi e cercare un luogo adatto per allestire le tende e preparare il necessario per poter festeggiare.",
        "Scena3" => "Superata la barricata e trovato il punto perfetto, i ragazzi allestiscono il campeggio. Sistemano le tende, accendono un fuoco e si mettono comodi, raccontandosi storie, condividendo carne grigliata e bevendo. L'atmosfera è rilassata, e i ragazzi si godono la serata in un clima di spensieratezza, finché...",
        _ => "Descrizione: Sconosciuta" // Non dovrebbe mai accadere se le scene sono correttamente definite
    };

    public Sprite GetChapterSprite(string sceneName)
    {
        foreach (var sprite in spritesScenes)
        {
            if (sprite.name == sceneName) return sprite;
        }
        return null;
    }

    public (Sprite, string) GetSpriteAndDescriptionChapter(string sceneName)
    {
        var sprite = GetChapterSprite(sceneName);
        var description = GetChapterDescription(sceneName);
        return (sprite, description);
    }

    public void GoToTransitionScene()
    {
        PlayerPrefs.SetString("CurrentChapter", nextChapter);
        PlayerPrefs.SetString("NextScene", nextScene);
        PlayerPrefs.Save();
        if (nextScene == "Scena0")
        {
            SaveLoadSystem.Instance.DeleteGame("Checkpoint");
            collectedItemIds.Clear();
            Destroy(SaveLoadSystem.Instance.gameObject);
        }

        SceneManager.LoadScene("Transizione");
    }

    public void SetItemScene(Item item)
    {
        if (item.nameItem.Contains("Zaino")) PlayerPrefs.SetInt("hasBackpack", 1);
        else if (item.nameItem.Contains("Torcia")) PlayerPrefs.SetInt("hasTorch", 1);
        else if (item.nameItem.Contains("Salsiccia")) PlayerPrefs.SetInt("hasBait", 1);

        PlayerPrefs.Save();
    }

    public void SaveCheckpoint() => SaveLoadSystem.Instance.SaveCheckpoint(); // Invocata nel DE | cliccando su uno slot

    public static void ChangeCursorActiveStatus(bool isOpen, string debug)
    {
        // Gestione dell'apertura/chiusura delle UI
        // var nUIOpenDebug = nUIOpen;
        nUIOpen += isOpen ? 1 : -1;
        nUIOpen = Mathf.Max(0, nUIOpen); // Per evitare valori negativi

        // print($"UI aperte: ({nUIOpenDebug} {(isOpen ? "+ 1" : "- 1")}) {nUIOpen} (invocata in {debug})");
        ToggleCursor(nUIOpen > 0);  // Cambia visibilità del cursore se almeno una UI è aperta
    }

    public static void ToggleCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public static string GenerateId(GameObject gameObj, Transform transform)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string objectName = gameObj.name;
        Vector3 position = transform.position;
        return $"{sceneName}_{objectName}_{position.x}_{position.y}_{position.z}";
    }
}
