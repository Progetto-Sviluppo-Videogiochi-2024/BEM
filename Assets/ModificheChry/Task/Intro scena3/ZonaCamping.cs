using System.Collections;
using System.Runtime.CompilerServices;
using DialogueEditor;
using UnityEngine;

public class ZonaCamping : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    private bool isPreHitBallFinished = false; // Flag per verificare se è finito il dialogo Pre-HitBall di Angelica e Jacob
    private bool isPostHitBallJA = false; // Flag per verificare se è finito il dialogo Post-HitBall di Angelica e Jacob
    private bool hasSeenVideoMutant = false; // Flag per verificare se è stato visto il video del mutante
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
    private Transform angelica; // Riferimento ad Angelica (AI)
    private HumanFollower angelicaFollower; // Riferimento al componente HumanFollower di Angelica
    public Transform player; // Riferimento al giocatore (Player)
    public Transform gaia; // Riferimento a Gaia (AI)
    public GameObject cameraIntro; // Riferimento alla camera dell'intro della scena 3
    public AudioListener audioCameraStefano; // Riferimento all'AudioListener della camera di Stefano
    public GameObject ballLauncher; // Riferimento alla palla lanciata da Angelica
    public GameObject ball; // Riferimento alla palla non ancora calciata da Angelica
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
        jacob.GetComponent<Animator>().SetBool("Portiere", true);
        jacobAnimator = jacob.GetComponent<Animator>();
        angelicaFollower = angelica.GetComponent<HumanFollower>();
        ballLauncher.SetActive(false);

        if (!booleanAccessor.GetBoolFromThis("videoMutant")) InitCharacters(false);
        else // Se è già stato visto il video del mutante (controllo per il LG e il bug pre-video)
        {
            // Dopo il video del mutante allora Stefano e Gaia si alzano e camminano
            // AI di Gaia che cammina seguendo Stefano (Player)
            ball.SetActive(false);
            ballLauncher.SetActive(true);
            hasSeenVideoMutant = true;
            cameraIntro.SetActive(false);
            audioCameraStefano.enabled = true;
            InitCharacters(true);
            angelica.gameObject.SetActive(false);
            jacob.gameObject.SetActive(false);
            enabled = false;
            return;
        }

        if (!booleanAccessor.GetBoolFromThis("preHitBallJA"))
        {
            hasSeenVideoMutant = PlayerPrefs.GetInt("videoMutant", 0) == 1;
            cameraIntro.SetActive(true);
            audioCameraStefano.enabled = false;
            conversationManager.StartConversation(conversations[0]); // Pre-HitBall di Angelica e Jacob
        }
    }

    void Update()
    {
        // Se è finito il primo dialogo e Angelica ha calciato la palla allora Post-HitBall di Angelica e Jacob
        if (!isPreHitBallFinished && booleanAccessor.GetBoolFromThis("preHitBallJA") && HasBallBeenKicked())
        {
            isPreHitBallFinished = true; // Si ripete una sola volta almeno
            conversationManager.StartConversation(conversations[1]); // Post-HitBall di Angelica e Jacob
            jacobAnimator.SetBool("Portiere", true);
        }

        // Se Post-HitBall è TRUE allora Angelica e Jacob si allontanano e scompaiono
        if (!isPostHitBallJA && booleanAccessor.GetBoolFromThis("postHitBallJA"))
        {
            // Fare una classe astratta per le due AI così seguono una logica comune per poi allontanarsi e scomparire (usare un flag dello script)
            if (Input.GetKeyDown(KeyCode.Z)/*angelica.isArrived && jacob.isArrived*/) // per testing // Usare i PP
            {
                jacob.gameObject.SetActive(false);
                angelica.gameObject.SetActive(false);
                isPostHitBallJA = true; // Si ripete una sola volta almeno
                conversationManager.hasClickedEnd = false;
                conversationManager.StartConversation(conversations[2]); // Post-HitBall di Stefano e Gaia
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

    private bool HasBallBeenKicked() => angelicaFollower.HitBall;

    public void StartCoroutinePreVideo() => StartCoroutine(TimerPreVideo()); // Invocata nell'ultimo nodo dell'ultimo dialogo di Stefano e Gaia

    private IEnumerator TimerPreVideo()
    {
        Debug.Log("Inizio Coroutine");
        print($"hasclickend è ancora false! {conversationManager.hasClickedEnd}");
        while (!conversationManager.hasClickedEnd) yield return null; // Aspetta il frame successivo e poi ricontrolla
        Debug.Log("hasclickend è diventato true!");
        booleanAccessor.SetBoolOnDialogueE("postHitBallSG");
    }
}
