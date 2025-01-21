using System.Collections;
using DialogueEditor;
using UnityEngine;
using UnityEngine.AI;

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
    private Animator jacobAnimator; // Riferimento all'animator di Jacob
    private NavMeshAgent agentJacob; // Riferimento al NavMeshAgent di Jacob
    private Transform angelica; // Riferimento ad Angelica (AI)
    private AngelicaHitBall agentAngelica; // Riferimento al componente AngelicaHitBall di Angelica
    public Transform player; // Riferimento al giocatore (Player)
    public Transform gaia; // Riferimento a Gaia (AI)
    public GameObject cameraIntro; // Riferimento alla camera dell'intro della scena 3
    public AudioListener audioCameraStefano; // Riferimento all'AudioListener della camera di Stefano
    public GameObject ballLauncher; // Riferimento alla palla lanciata da Angelica
    public GameObject ball; // Riferimento alla palla non ancora calciata da Angelica
    public Diario diario; // Riferimento diretto allo script del diario
    #endregion

    void Start()
    {
        booleanAccessor = BooleanAccessor.istance;
        conversationManager = ConversationManager.Instance;

        var characters = transform.GetChild(0);
        gaiaSitting = characters.GetChild(0);
        stefanoSitting = characters.GetChild(1);
        jacob = characters.GetChild(2);
        angelica = characters.GetChild(3);

        if (!booleanAccessor.GetBoolFromThis("videoMutant"))
        {
            jacob.GetComponent<Animator>().SetBool("Portiere", true);
            jacobAnimator = jacob.GetComponent<Animator>();
            agentJacob = jacob.GetComponent<NavMeshAgent>();
            agentJacob.enabled = false;
            agentAngelica = angelica.GetComponent<AngelicaHitBall>();
            ballLauncher.SetActive(false);
            InitCharacters(false);
            PlayerPrefs.SetInt("videoMutantCP", 0);
            PlayerPrefs.Save();
        }
        else // Se è già stato visto il video del mutante (controllo per il LG e il bug pre-video)
        {
            // Dopo il video del mutante allora Stefano e Gaia si alzano e camminano
            ball.SetActive(false);
            ballLauncher.SetActive(true);
            cameraIntro.SetActive(false);
            audioCameraStefano.enabled = true;
            InitCharacters(true); // AI di Gaia che cammina seguendo Stefano (Player)
            angelica.gameObject.SetActive(false);
            jacob.gameObject.SetActive(false);
            diario.AggiungiMissione("Trova i tuoi amici");
            if (PlayerPrefs.GetInt("videoMutantCP") == 0)
            {
                conversationManager.StartConversation(conversations[3]);
                GestoreScena.ChangeCursorActiveStatus(true, $"ZonaCamping.Start else: Dialogo_3");
                ConversationManager.OnConversationEnded += OnDialogueEnded;
                PlayerPrefs.SetInt("videoMutantCP", 1);
                PlayerPrefs.Save();
                SaveLoadSystem.Instance.SaveCheckpoint();
            }
            enabled = false;
            return;
        }

        if (!booleanAccessor.GetBoolFromThis("preHitBallJA"))
        {
            cameraIntro.SetActive(true);
            audioCameraStefano.enabled = false;
            GestoreScena.ChangeCursorActiveStatus(true, $"ZonaCamping.Start if_2: Dialogo_1");
            conversationManager.StartConversation(conversations[0]); // Pre-HitBall di Angelica e Jacob
            ConversationManager.OnConversationEnded += OnDialogueEnded;
        }
    }

    void Update()
    {

        // Se è finito il primo dialogo e Angelica ha calciato la palla allora Post-HitBall di Angelica e Jacob
        if (!isPreHitBallFinished && booleanAccessor.GetBoolFromThis("preHitBallJA") && HasBallBeenKicked())
        {
            isPreHitBallFinished = true; // Si ripete una sola volta almeno
            conversationManager.hasClickedEnd = false;
            GestoreScena.ChangeCursorActiveStatus(true, $"ZonaCamping.Update: Dialogo_2");
            conversationManager.StartConversation(conversations[1]); // Post-HitBall di Angelica e Jacob
            ConversationManager.OnConversationEnded += OnDialogueEnded;
            jacobAnimator.SetBool("Portiere", true);
        }

        // Se Post-HitBall è TRUE allora Angelica e Jacob si allontanano e scompaiono
        if (!isPostHitBallJA && booleanAccessor.GetBoolFromThis("postHitBallJA"))
        {
            // Fare una classe astratta per le due AI così seguono una logica comune per poi allontanarsi e scomparire (usare un flag dello script)
            if (agentAngelica.isArrived) // Angelica sta dietro jacob => mi basta controllare solo lei
            {
                jacob.gameObject.SetActive(false);
                angelica.gameObject.SetActive(false);
                isPostHitBallJA = true; // Si ripete una sola volta almeno
                conversationManager.hasClickedEnd = false;
                GestoreScena.ChangeCursorActiveStatus(true, $"ZonaCamping.Update: Dialogo_3");
                conversationManager.StartConversation(conversations[2]); // Post-HitBall di Stefano e Gaia
                ConversationManager.OnConversationEnded += OnDialogueEnded;
            }
        }

        // Dopo che termina il dialogo di Stefano e Gaia allora parte il video del mutante che attacca i militari
        if (booleanAccessor.GetBoolFromThis("postHitBallSG")) videoTransitionManager.StartVideo();
    }

    private void InitCharacters(bool enable)
    {
        player.gameObject.SetActive(enable);
        gaia.gameObject.SetActive(enable);
        gaiaSitting.gameObject.SetActive(!enable);
        stefanoSitting.gameObject.SetActive(!enable);
    }

    private bool HasBallBeenKicked() => agentAngelica.hitBall;

    public void StartCoroutinePreVideo() => StartCoroutine(TimerPreVideo()); // Invocata nell'ultimo nodo dell'ultimo dialogo di Stefano e Gaia

    private IEnumerator TimerPreVideo()
    {
        while (!conversationManager.hasClickedEnd) yield return null; // Aspetta il frame successivo e poi ricontrolla
        booleanAccessor.SetBoolOnDialogueE("postHitBallSG");
    }

    public void CharacterMovement()
    {
        agentJacob.enabled = true;
        agentAngelica.MoveTowardsStopTarget(agentAngelica.stopTarget);
    }

    private void OnDialogueEnded()
    {
        ConversationManager.OnConversationEnded -= OnDialogueEnded;
        GestoreScena.ChangeCursorActiveStatus(false, "ZonaCamping.OnDialogueEnded");
    }
}
