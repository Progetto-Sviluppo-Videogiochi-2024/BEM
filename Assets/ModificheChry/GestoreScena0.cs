using UnityEngine;

public class GestoreScena0 : MonoBehaviour
{
    [Header("Tutorial")]
    #region Tutorial
    public int contatoriEndClick = 0;
    #endregion

    [Header("Camere")]
    #region Camere
    public Camera display1Camera;
    public Camera display2Camera;
    private bool isDisplay1Active = false;
    #endregion

    [Header("References")]
    #region References
    public GameObject backPackPlayer;
    private GameObject player;
    private Animator animator;
    public TutorialScript tutorialScript; // Riferimento diretto allo script del tutorial
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

        // Tutoria del gioco
        tutorialScript.StartTutorial(); // Invoca AvanzaDialogo
    }

    void Update()
    {
        // Gestione del personaggio
        // ManageCharacter(); // scommentare se funziona
        HideBackPack();
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

    private void ManageCharacter()
    {
        // Appena parte il DE di Stefano
        if (!animator.GetBool("sit")) tutorialScript.StartTutorial(); // Invoca WASD e Mouse
        // ?= if (Tutorial DE è cominciato)
        // - ricordarsi di switchare la camera (da "dietro il pc" a "dietro il personaggio")
        isDisplay1Active = !isDisplay1Active;
        SwitchCamera(isDisplay1Active ? 1 : 2);
        // - settare sit=false
        animator.SetBool("sit", false);
        // - trigger standUp:
        animator.SetTrigger("standUp");
        // -- abilitare il layer di movimento
        animator.SetLayerWeight(animator.GetLayerIndex("Movement"), 1);
        // -- lo script di movimento (MovementStateManager)
        player.GetComponent<MovementStateManager>().enabled = true;
        // -- disabilitare il layer di animazione
        animator.SetLayerWeight(animator.GetLayerIndex("Scene"), 0);
        // -- altro ?
    }
}
