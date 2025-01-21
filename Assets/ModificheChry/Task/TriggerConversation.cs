using DialogueEditor;
using UnityEngine;

public class TriggerConversation : MonoBehaviour
{
    private bool oneTimeDialogue = false; // Flag per riprodurre la conversazione una sola volta
    public NPCConversation[] conversations; // Array di dialoghi
    public Player player; // Riferimento al giocatore
    public string nomeBoolBA; // Nome del boolean accessor da settare

    void OnDialogueEnded()
    {
        oneTimeDialogue = true;

        SaveLoadSystem.Instance.SaveCheckpoint();
        GestoreScena.ChangeCursorActiveStatus(false, "NPCDialogueBase.update: " + gameObject.transform.parent.name);
        player.GetComponent<MovementStateManager>().enabled = true;
        BooleanAccessor.istance.SetBoolOnDialogueE(nomeBoolBA);
        ConversationManager.OnConversationEnded -= OnDialogueEnded;
        enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!player.hasEnemyDetectedPlayer && !BooleanAccessor.istance.GetBoolFromThis(nomeBoolBA) && !oneTimeDialogue && other.CompareTag("Player"))
        {
            GestoreScena.ChangeCursorActiveStatus(true, "TriggerConversation.OnTriggerEnter: " + gameObject.name);
            ConversationManager.Instance.StartConversation(conversations[0]);
            ConversationManager.OnConversationEnded += OnDialogueEnded;
        }
    }
}
