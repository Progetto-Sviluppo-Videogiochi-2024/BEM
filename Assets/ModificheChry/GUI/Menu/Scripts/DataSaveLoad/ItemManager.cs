using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Serializable]
    public struct ItemEntry
    {
        public string objectName; // Nome univoco dell'oggetto (get di item.nameItem)
        public Item item; // Riferimento all'oggetto Item
    }

    [Header("Item Collection")]
    #region Item Collection
    public List<ItemEntry> itemEntries; // Lista delle coppie: objectName -> Item
    private Dictionary<string, Item> itemDictionary; // Dizionario: objectName -> Item
    #endregion

    [Header("Singleton")]
    #region Singleton
    private static ItemManager _instance;
    public static ItemManager Instance => _instance;
    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Rendi l'oggetto persistente tra le scene
            InitializeItemDictionary();
        }
        else Destroy(gameObject); // Evita duplicati
    }

    private void InitializeItemDictionary()
    {
        itemDictionary = new();
        foreach (var entry in itemEntries)
        {
            if (!string.IsNullOrEmpty(entry.objectName) && entry.item != null && !itemDictionary.ContainsKey(entry.objectName))
            {
                itemDictionary.Add(entry.objectName, entry.item);
            }
        }
    }

    public Item GetItemByName(string objectName)
    {
        if (itemDictionary.TryGetValue(objectName, out var item)) return item;
        Debug.LogError($"Item '{objectName}' does not exist in the collection.");
        return null;
    }
}
