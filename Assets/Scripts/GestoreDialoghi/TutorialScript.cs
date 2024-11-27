using UnityEngine;
using DialogueEditor;
using System.Collections.Generic;

public class TutorialScript : MonoBehaviour
{
    [Header("Tutorial Dialogues")]
    #region Tutorial Dialogues
    public NPCConversation[] conversations;
    #endregion

    [Header("Data Storage")]
    #region Data Storage
    private Dictionary<string, NPCConversation> tutorialConversations;
    #endregion

    void Awake()
    {
        // Inizializza il dizionario con i nomi dei flag e i dialoghi associati
        tutorialConversations = new()
        {
            { "premereE", conversations[0] },
            { "wasd", conversations[1] },
            { "quest", conversations[2] },
            { "zaino", conversations[3] },
        };
    }

    public void StartTutorial(string tutorial)
    {
        if (BooleanAccessor.istance != null)
        {
            var convManager = ConversationManager.Instance;

            // Trova il primo dialogo non ancora visualizzato
            foreach (var entry in tutorialConversations)
            {
                if (entry.Key == tutorial)
                {
                    // Avvia il dialogo e aggiorna il flag associato
                    convManager.StartConversation(entry.Value);
                    convManager.SetBool(entry.Key, BooleanAccessor.istance.GetBoolFromThis(entry.Key));
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("BooleanAccessor.instance don't exist!");
        }
    }
}
