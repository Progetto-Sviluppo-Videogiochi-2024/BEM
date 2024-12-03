using UnityEngine;
using Cinemachine;
using DialogueEditor;
using System.Collections;

public class TriggerBooks : NPCDialogueBase
{
    [Header("References")]
    #region References
    private BooleanAccessor booleanAccessor; // Riferimento al boolean accessor
    [SerializeField] CinemachineBrain cinemachineBrain; // Riferimento al CinemachineBrain
    [SerializeField] CinemachineVirtualCamera behindPlayerCam; // Camera dietro il personaggio
    [SerializeField] CinemachineVirtualCamera behindHoleWallCam; // Camera dietro il buco nel muro
    [SerializeField] Transform gothicSitting; // Riferimento alla gotica seduta
    [SerializeField] Transform gothicDeathBlood; // Riferimento alla gotica morta col sangue
    [SerializeField] Transform mutant; // Riferimento al mutante
    [SerializeField] Transform imageTv; // Riferimento all'immagine della TV
    private GameObject imageElefant; // Riferimento all'immagine dell'elefante
    private GameObject imageDistorsion; // Riferimento all'immagine della distorsione
    #endregion

    [Header("Settings")]
    #region Settings
    private bool isTransitioning = false; // Stato per il blocco della transizione tra le telecamere
    private bool isViewingHole = false; // Stato per il blocco della visuale sul buco
    private const int maxScenes = 8; // Numero massimo di scenari (pari -> dialogo stefano, dispari -> camera dietro il buco)
    #endregion

    void Start()
    {
        booleanAccessor = BooleanAccessor.istance;
        booleanAccessor.SetIntOnDialogueE("nInteractionBookShelf", 0);

        gothicDeathBlood.gameObject.SetActive(false);
        mutant.gameObject.SetActive(false);

        imageElefant = imageTv.GetChild(0).gameObject;
        imageDistorsion = imageTv.GetChild(1).gameObject;
        imageDistorsion.SetActive(false);
    }

    protected override void Update()
    {
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

    protected override void StartDialogue()
    {
        if (isViewingHole)
        {
            StartCoroutine(SwitchCameraWithDelay(behindHoleWallCam, behindPlayerCam)); // Camera su Stefano
            isViewingHole = false;
        }

        var nInteractionBookShelf = booleanAccessor.GetIntFromThis("nInteractionBookShelf");
        if (nInteractionBookShelf % 2 == 0) // Camera su Stefano
        {
            StartConversation(conversations[Mathf.CeilToInt(nInteractionBookShelf / 2f)]);
            if (nInteractionBookShelf == maxScenes) { enabled = false; return; }
        }
        else // Camera sulla Persona
        {
            ManageJumpscare(nInteractionBookShelf);
            StartCoroutine(SwitchCameraWithDelay(behindPlayerCam, behindHoleWallCam)); // Camera dietro il buco
            isViewingHole = true;
        }
        booleanAccessor.SetIntOnDialogueE("nInteractionBookShelf", 1);
    }

    private IEnumerator SwitchCameraWithDelay(CinemachineVirtualCamera fromCam, CinemachineVirtualCamera toCam)
    {
        isTransitioning = true;
        fromCam.Priority = 5;
        toCam.Priority = 10;

        yield return new WaitForSeconds(3f); // Aspetta la fine del blend

        isTransitioning = false;
    }

    private void ManageJumpscare(int nInteractionBookShelf)
    {
        switch (nInteractionBookShelf)
        {
            case 3: // Appare il mutante
                imageElefant.SetActive(false);
                imageDistorsion.SetActive(true);
                mutant.gameObject.SetActive(true);
                break;
            case 5: // Appare la gotica morta col sangue (scompare la gotica seduta) e il mutante
                gothicSitting.gameObject.SetActive(false);
                gothicDeathBlood.gameObject.SetActive(true);
                mutant.gameObject.SetActive(false);
                break;
            case 7: // Scompare la gotica morta col sangue e il mutante
                imageDistorsion.SetActive(false);
                gothicDeathBlood.gameObject.SetActive(false);
                mutant.gameObject.SetActive(false);
                break;
        }
    }
}
