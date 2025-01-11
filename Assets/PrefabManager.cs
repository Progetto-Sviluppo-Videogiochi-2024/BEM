using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [Serializable]
    public struct GOWeaponEntry
    {
        public string objectName; // Nameitem dell'item
        public GameObject weapon; // GO per l'arma
    }

    [Header("GO Collection")]
    #region GO Collection
    public List<GOWeaponEntry> GOWeaponEntries; // Lista di GO armi: nameItemSO -> GO arma
    private Dictionary<string, GameObject> GODictionary; // Dizionario di coppie: nameItemSO -> (icona, immagine)
    #endregion

    [Header("Singleton")]
    #region Singleton
    private static PrefabManager _instance; // Singleton per il DontDestroyOnLoad
    public static PrefabManager Instance => _instance; // Proprietà per accedere all'istanza singleton
    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            // DontDestroyOnLoad(gameObject); // Rendi l'oggetto persistente
            InitGODictionary();
        }
        else
        {
            Destroy(gameObject); // Evita duplicati nelle scene
        }
    }

    private void InitGODictionary()
    {
        GODictionary = new();
        foreach (var entry in GOWeaponEntries)
        {
            if (!string.IsNullOrEmpty(entry.objectName) && !GODictionary.ContainsKey(entry.objectName))
            {
                GODictionary.Add(entry.objectName, entry.weapon);
            }
        }
    }

    public GameObject GetGO(string objectName)
    {
        if (GODictionary.TryGetValue(objectName, out var gameObject)) return gameObject;
        Debug.LogError($"GO '{objectName}' does not exist in the collection.");
        return null;
    }
}
