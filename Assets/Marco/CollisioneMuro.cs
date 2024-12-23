using UnityEngine;
using DialogueEditor;

public class CollisioneMuro : MonoBehaviour
{
    [Header("CollisioneMuroDialogo")]
    #region CollisioneMuroDialogo
    public NPCConversation dialogoMuro; // Array di dialoghi
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Controlla se l'oggetto che collide ha il tag "Player"
        {
            // Avvia il dialogo
            ConversationManager.Instance.StartConversation(dialogoMuro);
        }
    }
}
