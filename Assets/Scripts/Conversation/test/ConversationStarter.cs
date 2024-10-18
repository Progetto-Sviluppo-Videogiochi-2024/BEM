using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using Unity.VisualScripting;

public class ConversationStarter : MonoBehaviour
{
    public NPCConversation dialogue;
    private bool isInRange;
    bool questAssegnataP;
    bool completataP;
    bool questAssegnataP2;
    bool completataP2;

    Diario diario;

    // Start is called before the first frame update
    void Start()
    {
        isInRange = false;

        // Assicurati che Diario sia inizializzato correttamente
        diario = FindObjectOfType<Diario>();
        if (diario == null)
        {
            Debug.LogError("Diario non trovato nella scena. Assicurati di avere un oggetto Diario nella scena.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Se il giocatore è nell'area di trigger e preme il tasto F, inizia la conversazione
        if (isInRange && Input.GetKeyDown(KeyCode.F))
        {
            StartDialogue(questAssegnataP, completataP);
        }
        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     Debug.Log("questAssegnata: " + ConversationManager.Instance.GetBool("questAssegnata") + "\n");
        //     Debug.Log("completata: " + ConversationManager.Instance.GetBool("completata") + "\n");

        // }
    }



    // Avvia la conversazione con il giocatore
    private void StartDialogue(bool questAssegnataP, bool completataP)
    {
        if (diario == null)
        {
            Debug.LogError("Diario non è stato inizializzato correttamente.");
            return;
        }

        // Inizia la conversazione
        ConversationManager.Instance.StartConversation(dialogue);


        // Imposta il parametro della quest assegnata
        ConversationManager.Instance.SetBool("questAssegnata", questAssegnataP);
        ConversationManager.Instance.SetBool("completata", completataP);
        ConversationManager.Instance.SetBool("questAssegnata2", questAssegnataP2);
        ConversationManager.Instance.SetBool("completata2", completataP2);


        Debug.Log("StartDialogue\n");
        Debug.Log("questAssegnata: " + ConversationManager.Instance.GetBool("questAssegnata") + "\n");
        Debug.Log("completata: " + ConversationManager.Instance.GetBool("completata") + "\n");
        Debug.Log("questAssegnata2: " + ConversationManager.Instance.GetBool("questAssegnata2") + "\n");
        Debug.Log("completata2: " + ConversationManager.Instance.GetBool("completata2") + "\n");

    }


    public void StopConversation()
    {
        ConversationManager.Instance.EndConversation();

        Debug.Log("EndConversation\n");
        Debug.Log("questAssegnata: " + ConversationManager.Instance.GetBool("questAssegnata"));
        Debug.Log("completata: " + ConversationManager.Instance.GetBool("completata"));
        Debug.Log("questAssegnata2: " + ConversationManager.Instance.GetBool("questAssegnata2"));
        Debug.Log("completata2: " + ConversationManager.Instance.GetBool("completata2"));
        questAssegnataP = ConversationManager.Instance.GetBool("questAssegnata");
        completataP = ConversationManager.Instance.GetBool("completata");
        questAssegnataP2 = ConversationManager.Instance.GetBool("questAssegnata2");
        completataP2 = ConversationManager.Instance.GetBool("completata2");

        if (questAssegnataP == true)
        {
            ConversationManager.Instance.SetBool("questAssegnataP2", true);
        }

        if (ConversationManager.Instance.GetBool("completata2") == true)
        {
            ConversationManager.Instance.SetBool("completataP2", true);
            
        }
        

    }

    // Inizia la conversazione con il giocatore se è nell'area di trigger 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    // Termina la conversazione con il giocatore se esce dall'area di trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            StopConversation();
        }
    }

    // Continua la conversazione con il giocatore se è nell'area di trigger
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }




}
