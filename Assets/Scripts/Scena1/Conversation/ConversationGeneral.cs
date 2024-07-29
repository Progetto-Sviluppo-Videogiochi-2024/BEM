using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationGeneral : MonoBehaviour
{
    [SerializeField] private NPCConversation dialogue;
    private VanWheelRotation vanWheelRotation;

    // Start is called before the first frame update
    void Start()
    {
        vanWheelRotation = FindObjectOfType<VanWheelRotation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (vanWheelRotation != null && vanWheelRotation.isRotating)
        {
            StartDialogue();
        }
    }

    // Avvia la conversazione con il giocatore
    private void StartDialogue()
    {
        ConversationManager.Instance.StartConversation(dialogue);
        // Disabilitiamo l'update per evitare di ripetere la conversazione
        this.enabled = false;
    }

    private void StopConversation()
    {
        ConversationManager.Instance.EndConversation();
    }
}