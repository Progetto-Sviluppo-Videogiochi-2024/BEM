using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class BooleanAccessor : MonoBehaviour
{
    [Header("Singleton")]
    #region Singleton
    public static BooleanAccessor istance;
    #endregion

    [Header("Data Storage")]
    #region Data Storage
    private Dictionary<string, bool> boolValues = new()
    {
        { "premereE", false }, // Se il giocatore ha finito il primo dialogo di scena0
        { "wasd", false }, // Se il giocatore ha finito il secondo dialogo di scena0
        { "quest", false }, // Se il giocatore ha finito il terzo dialogo di scena0
        { "zaino", false }, // Se il giocatore ha finito il quarto dialogo di scena0
        { "radio", false }, // Se il giocatore ha spento la radio di scena0 (task)
        { "cartello", false }, // Se il giocatore ha parlato con Jacob in scena2
        { "cartelloDone", false }, // Se il giocatore ha tolto il cartello dalla recinzione in scena2
        { "wolf", false }, // Se il giocatore ha parlato con UomoBaita in scena2 (task)
        { "wolfDone", false }, // Se il giocatore ha dato il cibo al lupo in scena2
        { "cocaCola", false }, // Se il giocatore ha parlato con UomoBaita dopo aver riportato il lupo ed è pronto per sparare in scena2 (task)
        { "cocaColaDone", false }, // Se il giocatore ha sparato le lattine di cocacola in scena2
        { "fioriRaccolti", false }, // Se il giocatore ha raccolto 3 fiori in scena2
        { "fiori", false }, // Se il giocatore ha parlato con Gaia in scena2 (task)
        { "soluzione", false } // Se il giocatore è giunto nel dialogo di Gaia a craftare la soluzione in scena2
    };
    #endregion

    void Awake()
    {
        if (istance == null)
        {
            istance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += TakeBA;
        }
        else if (istance != this)
        {
            print("BooleanAccessor: Instance already exists, destroying object!");
            Destroy(gameObject);
        }
    }

    public void SetBoolOnDialogueE(string nomeBool)
    {
        if (boolValues.ContainsKey(nomeBool)) boolValues[nomeBool] = true;
        else Debug.LogWarning("BooleanAccessor: nomeBool don't found - " + nomeBool);
    }

    public void ResetBoolValue(string nomeBool)
    {
        if (boolValues.ContainsKey(nomeBool)) boolValues[nomeBool] = false;
        else Debug.LogWarning("BooleanAccessor: nomeBool don't found - " + nomeBool);
    }

    public bool GetBoolFromThis(string nomeBool) => boolValues.ContainsKey(nomeBool) && boolValues[nomeBool];

    public void ResetBoolValues()
    {
        foreach (var key in boolValues.Keys.ToList())
        {
            boolValues[key] = false; // Resetta tutti i valori booleani
        }
    }

    private void TakeBA(Scene scene, LoadSceneMode mode)
    {
        // TODO: far partire il BA da main menu considerando però anche la questione del save del gioco 

        string sceneName = scene.name; // Nome della scena appena caricata
        string sceneNumber = sceneName[^1..]; // Ultimo carattere

        if (string.IsNullOrEmpty(sceneNumber) && (sceneName != "MainMenu" || sceneName != "Transizione"))
        { Debug.LogWarning("Scene number not found in scene name: " + sceneName); return; }

        // Cerca il GO e in particolare il componente con lo stesso nome del GO
        string managerObjectName = "ManagerScena" + sceneNumber;
        GameObject manager = GameObject.Find(managerObjectName);

        if (manager == null && (sceneName != "MainMenu" || sceneName != "Transizione"))
        { Debug.LogWarning("Manager don't found in scene " + sceneName); return; }

        var components = manager.GetComponents<Component>();
        foreach (var component in components)
        {
            if (component.GetType().Name.StartsWith(managerObjectName)) // Se il componente ha lo stesso nome del GO
            {
                component.GetType().GetField("booleanAccessor").SetValue(component, this);
                return; // Uscire dal ciclo una volta trovato e settato BA
            }
        }
    }

    private void OnDestroy()
    {
        // Rimuovi il listener quando l'oggetto viene distrutto
        if (istance == this) SceneManager.sceneLoaded -= TakeBA;
    }
}
