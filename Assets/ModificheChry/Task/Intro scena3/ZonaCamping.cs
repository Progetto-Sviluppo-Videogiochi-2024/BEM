using System;
using DialogueEditor;
using UnityEngine;

public class ZonaCamping : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    private bool isPreHitBallFinished = false; // Flag per verificare se è finito il dialogo Pre-HitBall di Angelica e Jacob
    private bool isPostHitBallJA = false; // Flag per verificare se è finito il dialogo Post-HitBall di Angelica e Jacob
    #endregion

    [Header("References")]
    #region References
    public VideoTransitionManager videoTransitionManager; // Riferimento al VideoTransitionManager per il video del mutante che attacca i militari
    public NPCConversation[] conversations; // Array di dialoghi per la zona camping
    private ConversationManager conversationManager; // Riferimento al ConversationManager
    private BooleanAccessor booleanAccessor; // Riferimento al BooleanAccessor
    private Transform gaiaSitting; // Riferimento al transform di Gaia seduta
    private Transform stefanoSitting; // Riferimento al transform di Stefano seduto
    private Transform jacob; // Riferimento a Jacob (AI)
    private Transform angelica; // Riferimento ad Angelica (AI)
    public Transform player; // Riferimento al giocatore (Player)
    public Transform gaia; // Riferimento a Gaia (AI)
    #endregion

    void Start()
    {
        booleanAccessor = BooleanAccessor.istance;
        conversationManager = ConversationManager.Instance;

        var characters = transform.GetChild(0);
        gaiaSitting = characters.GetChild(0);
        stefanoSitting = characters.GetChild(1);
        // jacob = characters.GetChild(2);
        // angelica = characters.GetChild(3);

        // if (!booleanAccessor.GetBoolFromThis("videoMutant"))
        // {
        //     player.gameObject.SetActive(false);
        //     gaia.gameObject.SetActive(false);
        //     gaiaSitting.gameObject.SetActive(true);
        //     stefanoSitting.gameObject.SetActive(true);
        // }
        // else
        // {
        //     player.gameObject.SetActive(true);
        //     gaia.gameObject.SetActive(true);
        //     gaiaSitting.gameObject.SetActive(false);
        //     stefanoSitting.gameObject.SetActive(false);
        // }
        // if (angelica.isArrived && jacob.isArrived) // Usare i PP
        // {
        //     angelica.gameObject.SetActive(false);
        //     jacob.gameObject.SetActive(false);
        // }

        if (!booleanAccessor.GetBoolFromThis("preHitBallJA"))
            conversationManager.StartConversation(conversations[0]); // Pre-HitBall di Angelica e Jacob
    }

    void Update() // Invocarle come event del DialogueEditor nei relativi nodi e dialoghi -> fare delle sotto-funzioni per evitare l'uso di Update => efficienza++
    {
        if (booleanAccessor.GetBoolFromThis("videoMutant")) { enabled = false; return; }

        // Se è finito il primo dialogo e Angelica ha calciato la palla allora Post-HitBall di Angelica e Jacob
        if (!isPreHitBallFinished && booleanAccessor.GetBoolFromThis("preHitBallJA") && HasBallBeenKicked())
        {
            isPreHitBallFinished = true; // Si ripete una sola volta almeno
            conversationManager.StartConversation(conversations[1]); // Post-HitBall di Angelica e Jacob
        }

        // Se Post-HitBall è TRUE allora Angelica e Jacob si allontanano e scompaiono
        if (!isPostHitBallJA && booleanAccessor.GetBoolFromThis("postHitBallJA"))
        {
            // Fare una classe astratta per le due AI così seguono una logica comune per poi allontanarsi e scomparire (usare un flag dello script)
            if (Input.GetKeyDown(KeyCode.Z)/*angelica.isArrived && jacob.isArrived*/) // per testing // Usare i PP
            {
                // jacob.gameObject.SetActive(false);
                // angelica.gameObject.SetActive(false);
                isPostHitBallJA = true; // Si ripete una sola volta almeno
                conversationManager.StartConversation(conversations[2]); // Post-HitBall di Stefano e Gaia
            }
        }

        if (booleanAccessor.GetBoolFromThis("postHitBallSG"))
        {
            // Dopo che termina il dialogo di Stefano e Gaia allora parte il video del mutante che attacca i militari @marcoWarrior @ccorvino3
            videoTransitionManager.StartVideo();
            booleanAccessor.SetBoolOnDialogueE("videoMutant");
        }

        if (booleanAccessor.GetBoolFromThis("videoMutant"))
        {
            // Dopo il video del mutante allora Stefano e Gaia si alzano e camminano
            gaiaSitting.gameObject.SetActive(false);
            stefanoSitting.gameObject.SetActive(false);
            gaia.gameObject.SetActive(true);
            player.gameObject.SetActive(true);
            // AI di Gaia che cammina seguendo Stefano (Player)
        }
    }

    private bool HasBallBeenKicked()
    {
        // Per testing. Nico, questa parte è da cancellare
        if (Input.GetKeyDown(KeyCode.A)) return true;
        else return false;
        throw new NotImplementedException("Nicola implementa qui la funzione per verificare se la palla è stata colpita da Angelica.");
    }
}
