using System.Collections;
using UnityEngine;
using DialogueEditor;
using Cinemachine;
using System;
using System.Linq;
using System.Collections.Generic;

public class ManagerScena0 : MonoBehaviour
{
    [Header("Tutorial")]
    #region Tutorial
    private Action onDialogueEnd; // Delegato per la fine del dialogo (per passare al prossimo)
    private bool backPackTaken = false; // Variabile di controllo per lo zaino
    private bool torchTaken = false; // Variabile di controllo per la torcia
    #endregion

    [Header("Camere")]
    #region Camere
    public CinemachineVirtualCamera startViewCam; // Riferimento alla camera iniziale
    public CinemachineVirtualCamera behindPlayerCam; // Riferimento alla camera dietro il giocatore
    #endregion

    [Header("References")]
    #region References
    [SerializeField] TriggerBooks triggerBooks; // Riferimento al pickup dei libri da togliere dalla libreria
    private ConversationManager conversationManager; // Riferimento al ConversationManager
    public BooleanAccessor booleanAccessor; // Riferimento al BooleanAccessor
    public GameObject backPackPlayer; // Riferimento al GameObject dello zaino del giocatore
    private GameObject player; // Riferimento al GameObject del giocatore
    private Animator animator; // Riferimento all'Animator del giocatore
    public TutorialScript tutorialScript; // Riferimento diretto allo script del tutorial
    public Diario diario; // Riferimento diretto allo script del diario
    public Door door; // Riferimento alla porta
    List<PlayerPrefsData> playerPrefsData; // Dati del PlayerPrefs
    #endregion

    void Start()
    {
        // Ottengo i riferimenti
        booleanAccessor = BooleanAccessor.istance;
        GameData gameData = SaveLoadSystem.Instance.gameData;
        playerPrefsData = gameData.levelData.playerPrefs;
        player = FindObjectOfType<Player>().gameObject;
        animator = player.GetComponent<Animator>();
        conversationManager = ConversationManager.Instance;

        // Init PlayerPrefs
        PlayerPrefs.SetInt("hasBackpack", playerPrefsData.Where(p => p.key == "hasBackpack").Select(p => p.value).FirstOrDefault());
        PlayerPrefs.SetInt("hasTorch", playerPrefsData.Where(p => p.key == "hasTorch").Select(p => p.value).FirstOrDefault());
        PlayerPrefs.Save();
        // backPackTaken = false;
        // torchTaken = false;

        // Per una nuova game session
        if (!booleanAccessor.GetBoolFromThis("premereE"))
        {
            booleanAccessor.ResetBoolValues();
            SwitchCamera(5, 10); // Camera davanti Stefano
            animator.SetBool("inactive", true);
            animator.SetInteger("nInactive", 1);
            animator.SetBool("sit", true);
            player.GetComponent<MovementStateManager>().enabled = false;
            animator.SetLayerWeight(animator.GetLayerIndex("Movement"), 0);
            backPackPlayer.SetActive(false);
            diario.AggiungiMissione("Lista degli oggetti per Pasquetta: Zaino e Torcia");
            triggerBooks.enabled = false;
            triggerBooks.GetComponentInChildren<ItemPickup>().enabled = false;
            conversationManager.hasClickedEnd = false;
        }
        else SwitchCamera(10, 5); // Camera dietro Stefano

        StartConversation(
            () =>
            {
                if (booleanAccessor.GetBoolFromThis("premereE")) { conversationManager.hasClickedEnd = true; return; }
                GestoreScena.ChangeCursorActiveStatus(true, "scena0.premereE");
                tutorialScript.StartTutorial("premereE");
            },
            () => Invoke(nameof(StartSecondDialogue), 0f)
        );
    }

    void Update()
    {
        // Gestione della porta e se le quest sono state completate
        if (!door.canOpen && backPackTaken && torchTaken)
        {
            diario.CompletaMissione("Lista degli oggetti per Pasquetta: Zaino e Torcia");
            door.canOpen = true;
        }

        // Gestione degli oggetti raccolti
        HideBackPack();
        if (PlayerPrefs.GetInt("hasBackpack") == 1) backPackTaken = true;
        if (PlayerPrefs.GetInt("hasTorch") == 1) torchTaken = true;
    }

    private void SwitchCamera(int priority_vcam1, int priority_vcam2)
    {
        behindPlayerCam.Priority = priority_vcam1;
        startViewCam.Priority = priority_vcam2;
    }

    private void HideBackPack() => backPackPlayer.SetActive(PlayerPrefs.GetInt("hasBackpack") == 1);

    private void StandUpAndWASD(string dialogue)
    {
        if (!booleanAccessor.GetBoolFromThis(dialogue) && animator.GetBool("sit") && conversationManager.hasClickedEnd)
        {
            conversationManager.hasClickedEnd = false;
            SwitchCamera(10, 5);

            animator.SetBool("sit", false);
            animator.SetTrigger("standUp");
            animator.SetLayerWeight(animator.GetLayerIndex("Movement"), 1);
            player.GetComponent<MovementStateManager>().enabled = true;
            animator.SetLayerWeight(animator.GetLayerIndex("Clip"), 0);

            GestoreScena.ChangeCursorActiveStatus(true, "scena0.wasd");
            tutorialScript.StartTutorial(dialogue); // Invoca WASD&Mouse
        }
        else conversationManager.hasClickedEnd = true;
    }

    private void PostAction(string dialogue)
    {
        if (!booleanAccessor.GetBoolFromThis(dialogue) && conversationManager.hasClickedEnd)
        {
            conversationManager.hasClickedEnd = false;
            GestoreScena.ChangeCursorActiveStatus(true, "scena0." + dialogue);
            tutorialScript.StartTutorial(dialogue); // Invoca "dialogue" (quest, zaino)
            if (dialogue == "zaino") { triggerBooks.enabled = true; triggerBooks.GetComponentInChildren<ItemPickup>().enabled = true; }
        }
        else conversationManager.hasClickedEnd = true;
    }

    void StartSecondDialogue() => StartConversation(() => StandUpAndWASD("wasd"), () => StartCoroutine(WaitAndStartThirdDialogue(10f)));

    IEnumerator WaitAndStartThirdDialogue(float waitTime)
    {
        // Attendi 10 secondi prima di avviare il terzo dialogo
        yield return new WaitForSeconds(waitTime);

        // Avvia il terzo dialogo e definisci il delegato per il quarto dialogo
        StartConversation(() => PostAction("quest"), () => StartCoroutine(WaitAndStartFourthDialogue(10f)));
    }

    IEnumerator WaitAndStartFourthDialogue(float waitTime)
    {
        // Attendi 10 secondi prima di avviare il quarto dialogo
        yield return new WaitForSeconds(waitTime);

        // Avvia il quarto dialogo
        StartConversation(() => PostAction("zaino"), () => print("All dialogues completed."));
    }

    void StartConversation(Action onStartCallback, Action onEndCallback)
    {
        onStartCallback?.Invoke(); // Invoca il delegato per avviare il dialogo
        onDialogueEnd = onEndCallback; // Imposta il delegato per chiamare il prossimo dialogo
        StartCoroutine(WaitForClickEnd());
    }

    IEnumerator WaitForClickEnd()
    {
        // Quando clicco su End, nella funzione del DE già sta hasClickedEnd a true e quindi esce dal ciclo
        while (!conversationManager.hasClickedEnd) yield return null;

        // Quando il dialogo finisce, invoca il delegato per avviare il prossimo dialogo
        GestoreScena.ChangeCursorActiveStatus(false, "scena0.waitClickEnd");
        onDialogueEnd?.Invoke();
    }

    public void SetDEBool(string nomeBool) // Da invocare nel DialogueEditor per settare i valori booleani del BooleanAccessor
    {
        booleanAccessor.SetBoolOnDialogueE(nomeBool);
    }
}
