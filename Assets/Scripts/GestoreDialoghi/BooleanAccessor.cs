using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        { "premereE", false },
        { "wasd", false },
        { "quest", false },
        { "zaino", false },
        { "radio", false },
        { "cartello", false },
        { "tolto", false },
        { "wolf", false },
        { "wolfDone", false },
        { "fiore", false },
        { "soluzione", false }
    };
    #endregion

    void Awake()
    {
        if (istance == null)
        {
            istance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (istance != this)
        {
            Debug.Log("Un'altra istanza di BooleanAccessor è stata distrutta");
            Destroy(gameObject);
        }
    }

    public void SetBoolOnDialogueE(string nomeBool)
    {
        if (boolValues.ContainsKey(nomeBool)) boolValues[nomeBool] = true;
        else Debug.LogWarning("BooleanAccessor: nomeBool non riconosciuto - " + nomeBool);
    }

    public bool GetBoolFromThis(string nomeBool) => boolValues.ContainsKey(nomeBool) && boolValues[nomeBool];

    public void ResetBoolValues()
    {
        foreach (var key in boolValues.Keys.ToList())
        {
            boolValues[key] = false; // Resetta tutti i valori booleani
        }
    }

}
