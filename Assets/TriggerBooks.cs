using DialogueEditor;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class TriggerBooks : MonoBehaviour
{
    public List<NPCConversation> conversations;
    private ConversationManager conversationManager;
    private BooleanAccessor booleanAccessor;

    public CinemachineVirtualCamera behindPlayerCam; // Camera dietro il personaggio
    public CinemachineVirtualCamera behindHoleWallCam; // Camera dietro il buco nel muro

    private bool isPlayerInRange = false; // Stato per indicare la vicinanza del player al trigger
    private bool isViewingHole = false; // Stato per il blocco della visuale sul buco

    void Start()
    {
        conversationManager = ConversationManager.Instance;
        booleanAccessor = BooleanAccessor.istance;

        booleanAccessor.SetIntOnDialogueE("nInteractionBookShelf", 0);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (isViewingHole)
            {
                SwitchCamera(10, 5);
                isViewingHole = false;
                return;
            }

            switch (booleanAccessor.GetIntFromThis("nInteractionBookShelf"))
            {
                case 0: // Se il player rimuove i libri e ci interagisce
                    print("Scenario 1: Rimuove i libri e vede il buco");
                    conversationManager.StartConversation(conversations[0]);
                    booleanAccessor.SetIntOnDialogueE("nInteractionBookShelf", 1);
                    break;

                case 1: // Se il player decide di sbirciare nel buco del muro
                    SwitchCamera(5, 10);
                    print("Scenario 2: Sbircia nel buco del muro. VA IN LOOP -> TO_CONTINUE");
                    isViewingHole = true;
                    break;

                case 2:
                    conversationManager.StartConversation(conversations[1]);
                    booleanAccessor.SetIntOnDialogueE("nInteractionBookShelf", 2);
                    SwitchCamera(5, 10);
                    break;

                case 3:
                    conversationManager.StartConversation(conversations[2]);
                    booleanAccessor.SetIntOnDialogueE("nInteractionBookShelf", 3);
                    SwitchCamera(5, 10);
                    enabled = false;
                    break;
            }
        }
    }

    private void SwitchCamera(int priority_vcam1, int priority_vcam2)
    {
        behindPlayerCam.Priority = priority_vcam1;
        behindHoleWallCam.Priority = priority_vcam2;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
