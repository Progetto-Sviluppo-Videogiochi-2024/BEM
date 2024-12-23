using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [Serializable]
    public struct SpritePairEntry
    {
        public string objectName; // Nome univoco dello ScriptableObject
        public Sprite icon; // Sprite per l'icona
        public Sprite image; // Sprite per l'immagine
    }

    [Header("Sprite Collection")]
    #region Sprite Collection
    public List<SpritePairEntry> spritePairEntries; // Lista delle coppie: nameItemSO -> (icona, immagine)
    private Dictionary<string, (Sprite icon, Sprite image)> spriteDictionary; // Dizionario di coppie: nameItemSO -> (icona, immagine)
    #endregion

    [Header("Singleton")]
    #region Singleton
    private static SpriteManager _instance; // Singleton per il DontDestroyOnLoad
    public static SpriteManager Instance => _instance; // Proprietà per accedere all'istanza singleton
    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Rendi l'oggetto persistente
            InitializeSpriteDictionary();
        }
        else
        {
            Destroy(gameObject); // Evita duplicati nelle scene
        }
    }

    private void InitializeSpriteDictionary()
    {
        spriteDictionary = new();
        foreach (var entry in spritePairEntries)
        {
            if (!string.IsNullOrEmpty(entry.objectName) && !spriteDictionary.ContainsKey(entry.objectName))
            {
                spriteDictionary.Add(entry.objectName, (entry.icon, entry.image));
            }
        }
    }

    public (Sprite icon, Sprite image) GetSpritesByObjectName(string objectName)
    {
        if (spriteDictionary.TryGetValue(objectName, out var sprites)) return sprites;
        Debug.LogError($"SpritePair '{objectName}' does not exist in the collection.");
        return (null, null);
    }
}
