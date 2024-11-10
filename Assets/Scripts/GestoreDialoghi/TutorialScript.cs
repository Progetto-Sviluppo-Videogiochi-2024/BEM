using UnityEngine;
using DialogueEditor;
using System.Collections.Generic;

public class TutorialScript : MonoBehaviour
{
    [Header("Tutorial Dialogues")]
    #region Tutorial Dialogues
    public NPCConversation avanzaDialogo;
    public NPCConversation wasd;
    public NPCConversation quest;
    public NPCConversation zaino;
    #endregion

    [Header("Data Storage")]
    #region Data Storage
    private Dictionary<string, NPCConversation> tutorialConversations;
    #endregion

    void Awake()
    {
        // Inizializza il dizionario con i nomi dei flag e i dialoghi associati
        tutorialConversations = new Dictionary<string, NPCConversation>
        {
            { "premereE", avanzaDialogo },
            { "wasd", wasd },
            { "quest", quest },
            { "zaino", zaino }
        };
    }

    public void StartTutorial()
    {
        if (BooleanAccessor.istance != null)
        {
            var boolAccessor = BooleanAccessor.istance;
            var convManager = ConversationManager.Instance;

            // Trova il primo dialogo non ancora visualizzato
            foreach (var entry in tutorialConversations)
            {
                if (!boolAccessor.GetBoolFromThis(entry.Key))
                {
                    // Avvia il dialogo e aggiorna il flag associato
                    convManager.StartConversation(entry.Value);
                    convManager.SetBool(entry.Key, boolAccessor.GetBoolFromThis(entry.Key));
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
