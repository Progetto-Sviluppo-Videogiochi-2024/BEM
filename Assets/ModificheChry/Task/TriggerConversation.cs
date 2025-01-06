using DialogueEditor;
using UnityEngine;

public class TriggerConversation : MonoBehaviour
{
    private bool clickEndHandled = true; // Flag per gestire la fine della conversazione una sola volta
    private bool isDialogueActive = false; // Flag per evitare chiamate multiple
    public NPCConversation[] conversations; // Array di dialoghi
    public Transform player; // Riferimento al giocatore
    public string nomeBoolBA; // Nome del boolean accessor da settare

    void Update()
    {
        if (!clickEndHandled && ConversationManager.Instance.hasClickedEnd)
        {
            isDialogueActive = false;
            clickEndHandled = true;

            // print("NPCDialogueBase.Update 1.if: " + gameObject.name);
            GestoreScena.ChangeCursorActiveStatus(false, "NPCDialogueBase.update: " + gameObject.transform.parent.name);
            player.GetComponent<MovementStateManager>().enabled = true;
            BooleanAccessor.istance.SetBoolOnDialogueE(nomeBoolBA);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isDialogueActive && other.CompareTag("Player"))
        {
            GestoreScena.ChangeCursorActiveStatus(true, "TriggerConversation.OnTriggerEnter: " + gameObject.name);
            isDialogueActive = true;
            ConversationManager.Instance.StartConversation(conversations[0]);
        }
    }
}
