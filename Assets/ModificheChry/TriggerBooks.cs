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
    }

    protected override void Update()
    {
        if (!clickEndHandled && ConversationManager.Instance.hasClickedEnd)
        {
            isConversationActive = false;
            clickEndHandled = true;

            print("NPCDialogueBase.Update 1.if: " + gameObject.name);
            GestoreScena.ChangeCursorActiveStatus(false, "NPCDialogueBase.update: " + gameObject.transform.parent.name);
            player.GetComponent<MovementStateManager>().enabled = true;
        }

        if (isInRange && !isConversationActive && !isTransitioning && Input.GetKeyDown(KeyCode.Space))
        {
            ConversationManager.Instance.hasClickedEnd = false;
            clickEndHandled = false;
            print("NPCDialogueBase.Update 2.if: " + gameObject.name);

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

        // Ottieni la durata del blend dal CinemachineBrain
        // float blendDuration = cinemachineBrain.ActiveBlend?.Duration ?? 0.5f; // Default a 0.5 secondi se non c'è un blend attivo

        float blendDuration = 0.5f; // Valore di default
        if (cinemachineBrain.ActiveBlend != null)
        {
            blendDuration = cinemachineBrain.ActiveBlend.Duration;
            print($"Blend attivo tra: {cinemachineBrain.ActiveBlend.CamA?.Name} e {cinemachineBrain.ActiveBlend.CamB?.Name}. " +
                      $"Durata: {blendDuration} secondi.");
        }

        yield return new WaitForSeconds(blendDuration); // Aspetta la fine del blend

        isTransitioning = false;
    }

    private void ManageJumpscare(int nInteractionBookShelf)
    {
        switch (nInteractionBookShelf)
        {
            case 3: // Appare il mutante
                mutant.gameObject.SetActive(true);
                break;
            case 5: // Appare la gotica morta col sangue e scompare la gotica seduta e il mutante
                gothicSitting.gameObject.SetActive(false);
                gothicDeathBlood.gameObject.SetActive(true);
                mutant.gameObject.SetActive(false);
                break;
            case 7: // Scompare la gotica morta col sangue e il mutante
                gothicDeathBlood.gameObject.SetActive(false);
                mutant.gameObject.SetActive(false);
                break;
        }
    }
}
