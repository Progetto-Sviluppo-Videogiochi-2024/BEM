using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class BooleanAccessor : MonoBehaviour
{
    // Singleton
    public static BooleanAccessor istance;

    // Bools di Cubo
    bool assegnata = false;
    bool detto = false;

    // Bool di Sfera
    bool cuboChat = false;

    //E' necessario che tutte le variabili booleane create nel dialogue editor siano settate a false.
    public void SetBoolOnDialogueE(string nomeBool)
    {
        if (nomeBool == "assegnata")
        {
            assegnata = true;
        }
        if (nomeBool == "detto")
        {
            detto = true;
        }
        if (nomeBool == "cuboChat")
        {
            cuboChat = true;
        }
    }

    //Permette agli scripts dei dialoghi di poter accedere alle variabili booleane globali
    public bool GetBoolFromThis(string nomeBool)
    {
        if (nomeBool == "assegnata")
        {
            return assegnata;
        }
        if (nomeBool == "detto")
        {
            return detto;
        }
        if (nomeBool == "cuboChat")
        {
            return cuboChat;
        }
        return false;
    }

    void Awake()
    {
        if (istance == null)
        {
            istance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("BooleanAccessor inizializzato");
        }
        else if (istance != this)
        {
            Debug.Log("Un'altra istanza di BooleanAccessor è stata distrutta");
            Destroy(gameObject);
        }
    }

    // For Debug

    // void Update()
    // {
    //     //Debug per vedere se i valori delle variabili booleane globali
    //     if (Input.GetKeyDown(KeyCode.S))
    //     {
    //         Debug.Log("assegnata " + assegnata);
    //         Debug.Log("detto " + detto);
    //         Debug.Log("cuboChat " + cuboChat);
    //     }
    // }

}
