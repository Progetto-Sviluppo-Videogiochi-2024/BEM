using UnityEngine;

public class GestoreScena0 : MonoBehaviour
{
    [Header("References")]
    #region References
    public GameObject backPackPlayer;
    private GameObject player;
    private Animator animator;
    public TutorialScript tutorialScript; // Riferimento diretto allo script del tutorial
    #endregion

    [Header("Tutorial")]
    #region Tutorial
    public int contatoriEndClick = 0;
    #endregion

    void Start()
    {
        // cambiare la camera a runtime

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
        if (!animator.GetBool("sit")) tutorialScript.StartTutorial(); // Invoca WASD e Mouse
    }

    void Update()
    {
        // Gestione del personaggio
        ManageCharacter();
        HideBackPack();
    }

    private void HideBackPack() => backPackPlayer.SetActive(PlayerPrefs.GetInt("hasBackpack") == 1);

    private void ManageCharacter()
    {
        // Appena parte il DE di Stefano
        // - ricordarsi di switchare la camera (da "dietro il pc" a "dietro il personaggio")
        // - trigger standUp:
        // - settare sit=false
        // -- abilitare il layer di movimento
        // -- lo script di movimento (MovementStateManager)
        // -- disabilitare il layer di animazione
        // -- ?
    }
}
