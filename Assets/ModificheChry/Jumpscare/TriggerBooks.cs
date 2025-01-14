using UnityEngine;
using Cinemachine;
using DialogueEditor;
using System.Collections;
using System.Linq;

public class TriggerBooks : NPCDialogueBase
{
    [Header("References")]
    #region References
    [SerializeField] RadioManager radioManager; // Riferimento al RadioManager
    [SerializeField] AudioClip clipJumpscare; // Clip audio del jumpscare
    AudioSource audioSource; // Riferimento all'AudioSource
    [SerializeField] CinemachineVirtualCamera behindPlayerCam; // Camera dietro il personaggio
    [SerializeField] CinemachineVirtualCamera behindHoleWallCam; // Camera dietro il buco nel muro
    [SerializeField] Transform gothicSitting; // Riferimento alla gotica seduta
    [SerializeField] Transform gothicDeathBlood; // Riferimento alla gotica morta col sangue
    [SerializeField] Transform mutant; // Riferimento al mutante
    [SerializeField] Transform imageTv; // Riferimento all'immagine della TV
    private GameObject imageElefant; // Riferimento all'immagine dell'elefante
    private GameObject imageDistorsion; // Riferimento all'immagine della distorsione
    [SerializeField] Light TvLight; // Riferimento alla luce della TV
    #endregion

    [Header("Settings")]
    #region Settings
    private bool isTransitioning = false; // Stato per il blocco della transizione tra le telecamere
    private bool isViewingHole = false; // Stato per il blocco della visuale sul buco
    private const int maxScenes = 8; // Numero massimo di scenari (pari -> dialogo stefano, dispari -> camera dietro il buco)
    private const float mouseSensitivity = 3f; // Sensibilità del mouse
    private const float maxXRotation = 40f; // Limite massimo di rotazione sull'asse X
    private const float maxYRotation = 40f; // Limite massimo di rotazione sull'asse Y
    private bool allowSecondaryCameraControl = false; // Abilita il controllo del mouse per la camera dietro il muro
    private Vector2 currentSecondaryCameraRotation = Vector2.zero; // Rotazione attuale della camera secondaria
    private Quaternion initialRotation; // Rotazione iniziale della telecamera secondaria
    #endregion

    protected override void Start()
    {
        base.Start();

        initialRotation = behindHoleWallCam.transform.localRotation;

        GameData gameData = SaveLoadSystem.Instance.gameData;
        var playerPrefsData = gameData.levelData.playerPrefs;
        PlayerPrefs.SetInt("nInteractionBookShelf", playerPrefsData.Where(p => p.key == "nInteractionBookShelf").Select(p => p.value).FirstOrDefault());
        PlayerPrefs.Save();

        gothicDeathBlood.gameObject.SetActive(false);
        mutant.gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();

        imageElefant = imageTv.GetChild(0).gameObject;
        imageDistorsion = imageTv.GetChild(1).gameObject;
        imageDistorsion.SetActive(false);
    }

    protected override void Update()
    {
        if (PlayerPrefs.GetInt("nInteractionBookShelf") == maxScenes + 1) { enabled = false; return; }

        if (!clickEndHandled && ConversationManager.Instance.hasClickedEnd)
        {
            isConversationActive = false;
            clickEndHandled = true;

            GestoreScena.ChangeCursorActiveStatus(false, "NPCDialogueBase.update: " + gameObject.transform.parent.name);
            player.GetComponent<MovementStateManager>().enabled = true;
        }

        if (!isTransitioning && isInRange && !isConversationActive && Input.GetKeyDown(KeyCode.Space))
        {
            ConversationManager.Instance.hasClickedEnd = false;
            clickEndHandled = false;

            StartDialogue(); // Avvia il dialogo (metodo astratto che deve invocare StartConversation)
            player.GetComponent<MovementStateManager>().enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (allowSecondaryCameraControl) HandleSecondaryCameraControl();
    }

    protected override void StartDialogue()
    {
        print(PlayerPrefs.GetInt("nInteractionBookShelf"));
        if (isViewingHole)
        {
            StartCoroutine(SwitchCameraWithDelay(behindHoleWallCam, behindPlayerCam)); // Camera su Stefano
            isViewingHole = false;
        }

        var nInteractionBookShelf = PlayerPrefs.GetInt("nInteractionBookShelf");
        if (nInteractionBookShelf % 2 == 0) // Camera su Stefano
        {
            StopAudio();
            StartConversation(conversations[Mathf.CeilToInt(nInteractionBookShelf / 2f)]);
        }
        else // Camera sulla Persona
        {
            ManageJumpscare(nInteractionBookShelf);
            StartCoroutine(SwitchCameraWithDelay(behindPlayerCam, behindHoleWallCam)); // Camera dietro il buco
            isViewingHole = true;
        }
        PlayerPrefs.SetInt("nInteractionBookShelf", PlayerPrefs.GetInt("nInteractionBookShelf") + 1);
    }

    protected override void EndDialogue() { }

    private IEnumerator SwitchCameraWithDelay(CinemachineVirtualCamera fromCam, CinemachineVirtualCamera toCam)
    {
        isTransitioning = true;
        fromCam.Priority = 5;
        toCam.Priority = 10;

        yield return new WaitForSeconds(3f); // Aspetta la fine del blend

        isTransitioning = false;
        allowSecondaryCameraControl = toCam == behindHoleWallCam;
    }

    private void HandleSecondaryCameraControl()
    {
        // Ottieni il movimento del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Aggiorna la rotazione, rispettando i limiti
        currentSecondaryCameraRotation.x = Mathf.Clamp(currentSecondaryCameraRotation.x - mouseY, -maxXRotation, maxXRotation);
        currentSecondaryCameraRotation.y = Mathf.Clamp(currentSecondaryCameraRotation.y + mouseX, -maxYRotation, maxYRotation);

        // Applica la rotazione relativa alla rotazione iniziale
        Quaternion rotationX = Quaternion.AngleAxis(currentSecondaryCameraRotation.x, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(currentSecondaryCameraRotation.y, Vector3.up);

        // Combina la rotazione iniziale con le nuove rotazioni
        behindHoleWallCam.transform.localRotation = initialRotation * rotationY * rotationX;
    }

    private void ManageJumpscare(int nInteractionBookShelf)
    {
        switch (nInteractionBookShelf)
        {
            case 3: // Appare il mutante
                PlayAudio();
                imageElefant.SetActive(false);
                imageDistorsion.SetActive(true);
                mutant.gameObject.SetActive(true);
                StartCoroutine(FlickerLight(12f, 0.1f, 0.5f)); // 5 secondi di durata, intervallo tra 0.1 e 0.5 secondi
                break;
            case 5: // Appare la gotica morta col sangue (scompare la gotica seduta) e il mutante
                PlayAudio();
                gothicSitting.gameObject.SetActive(false);
                gothicDeathBlood.gameObject.SetActive(true);
                mutant.gameObject.SetActive(false);
                StartCoroutine(FlickerLight(12f, 0.1f, 0.5f)); // 5 secondi di durata, intervallo tra 0.1 e 0.5 secondi
                break;
            case 7: // Scompare la gotica morta col sangue e il mutante
                StopAudio();
                imageDistorsion.SetActive(false);
                gothicDeathBlood.gameObject.SetActive(false);
                mutant.gameObject.SetActive(false);
                TvLight.gameObject.SetActive(false);
                break;
        }
    }

    private void PlayAudio()
    {
        radioManager.MuteRadio(true);

        audioSource.clip = clipJumpscare;
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void StopAudio() { radioManager.MuteRadio(false); audioSource.Stop(); }

    private IEnumerator FlickerLight(float duration, float minInterval, float maxInterval)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Alterna lo stato della luce (accesa/spenta)
            TvLight.enabled = !TvLight.enabled;

            // Aspetta un intervallo casuale prima di cambiare stato
            float flickerInterval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(flickerInterval);

            elapsedTime += flickerInterval;
        }
        // Assicurati che la luce rimanga accesa alla fine
        if (TvLight != null) TvLight.enabled = true;
    }

}
