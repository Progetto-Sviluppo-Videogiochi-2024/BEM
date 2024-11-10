using System.Collections;
using UnityEngine;
using DialogueEditor;

public class GestoreScena0 : MonoBehaviour
{
    [Header("Tutorial")]
    #region Tutorial
    private bool backPackTaken = false;
    private bool torchTaken = false;
    #endregion

    [Header("Camere")]
    #region Camere
    public Camera display1Camera; // Camera principale
    public Camera display2Camera; // Camera secondaria
    private bool isDisplay1Active = false; // Indica se la camera principale è attiva
    #endregion

    [Header("References")]
    #region References
    public GameObject backPackPlayer; // Riferimento al GameObject dello zaino del giocatore
    private GameObject player; // Riferimento al GameObject del giocatore
    private Animator animator; // Riferimento all'Animator del giocatore
    public TutorialScript tutorialScript; // Riferimento diretto allo script del tutorial
    public Diario diario; // Riferimento diretto allo script del diario
    public Door door; // Riferimento alla porta
    #endregion

    void Start()
    {
        // cambiare la camera a runtime
        SwitchCamera(2);

        // Gestione del personaggio
        player = FindObjectOfType<Player>().gameObject;
        animator = player.GetComponent<Animator>();
        animator.SetBool("sit", true);
        player.GetComponent<MovementStateManager>().enabled = false;
        animator.SetLayerWeight(animator.GetLayerIndex("Movement"), 0);
        backPackPlayer.SetActive(false);

        // Salvare i dati
        PlayerPrefs.SetInt("hasBackpack", 0);
        PlayerPrefs.SetInt("hasTorch", 0);

        // Tutorial
        diario.AggiungiMissione("Lista degli oggetti per Pasquetta: Zaino e Torcia");
        tutorialScript.StartTutorial(); // Invoca AvanzaDialogo
    }

    void Update()
    {
        // Gestione del tutorial
        StandUpAndWASD();
        PostAction("quest");
        PostAction("zaino");

        // Gestione dello zaino
        HideBackPack();

        // Gestione degli oggetti raccolti
        if (PlayerPrefs.GetInt("hasBackpack") == 1) backPackTaken = true;
        if (PlayerPrefs.GetInt("hasTorch") == 1) torchTaken = true;

        // Gestione della porta e se le quest sono state completate
        if (backPackTaken && torchTaken)
        {
            diario.CompletaMissione("Lista degli oggetti per Pasquetta: Zaino e Torcia");
            door.canOpen = true;
        }
    }

    private void SwitchCamera(int displayNumber)
    {
        if (displayNumber == 1) // Display 1 (principale)
        {
            display1Camera.targetDisplay = 0;
            display2Camera.targetDisplay = 1;
        }
        else // Display 2 (secondario)
        {
            display1Camera.targetDisplay = 1;
            display2Camera.targetDisplay = 0;
        }
    }

    private void HideBackPack() => backPackPlayer.SetActive(PlayerPrefs.GetInt("hasBackpack") == 1);

    private void StandUpAndWASD()
    {
        if (!BooleanAccessor.istance.GetBoolFromThis("wasd") && animator.GetBool("sit") && ConversationManager.Instance.hasClickedEnd)
        {
            ConversationManager.Instance.hasClickedEnd = false;
            isDisplay1Active = !isDisplay1Active;
            SwitchCamera(isDisplay1Active ? 1 : 2);

            animator.SetBool("sit", false);
            animator.SetTrigger("standUp");
            animator.SetLayerWeight(animator.GetLayerIndex("Movement"), 1);
            player.GetComponent<MovementStateManager>().enabled = true;
            animator.SetLayerWeight(animator.GetLayerIndex("Clip"), 0);

            tutorialScript.StartTutorial(); // Invoca WASD e Mouse
        }
    }

    private void PostAction(string dialogue)
    {
        if (!BooleanAccessor.istance.GetBoolFromThis(dialogue) && ConversationManager.Instance.hasClickedEnd)
        {
            ConversationManager.Instance.hasClickedEnd = false;
            StartCoroutine(StartAfterTimer()); // Invoca l'azione data dal parametro dialogue
        }
    }

    private IEnumerator StartAfterTimer()
    {
        // Sospendi l'esecuzione per tot secondi
        yield return new WaitForSeconds(10f);
        tutorialScript.StartTutorial();
    }
}
