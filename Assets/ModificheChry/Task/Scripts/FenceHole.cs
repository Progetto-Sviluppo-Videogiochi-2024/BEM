using DialogueEditor;
using UnityEngine;
using UnityEngine.UI;

public class FenceHole : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    private bool isPlayerInRange = false; // Indica se il player è vicino al buco della recinzione
    private bool isConversationActive = false; // Indica se la conversazione è attiva
    private bool clickEndHandled = false; // Flag per evitare che esegua più volte il codice nel metodo Update quando si clicca su "End"
    #endregion

    [Header("References")]
    #region References
    public NPCConversation monologo; // Conversazione per il buco della recinzione quando non ha parlato con Jacob
    public GestoreScena gestoreScena; // Riferimento al gestore della scena
    private ManagerScena2 managerScena2; // Riferimento al manager della scena 2
    public GameObject confirmNextUI; // Riferimento al pannello di conferma per andare alla scena successiva
    private GameObject yesButton; // Riferimento al pulsante "Sì" del pannello di conferma
    private GameObject noButton; // Riferimento al pulsante "No" del pannello di conferma
    public Transform player; // Riferimento al player
    public Diario diario; // Riferimento al diario
    #endregion

    void OnEnable()
    {
        managerScena2 = gestoreScena.GetComponent<ManagerScena2>();

        var panel = confirmNextUI.transform.GetChild(0);
        yesButton = panel.Find("YesButton").gameObject;
        noButton = panel.Find("NoButton").gameObject;
    }

    void Update()
    {
        if (!clickEndHandled && ConversationManager.Instance.hasClickedEnd) // Se il dialogo è finito
        {
            clickEndHandled = true;
            isConversationActive = false;
            player.GetComponent<MovementStateManager>().enabled = true;
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (managerScena2.CanGoNextScene())
            {
                ToggleScripts(true);
                GestoreScena.ChangeCursorActiveStatus(true, "FenceHole.Update");
                confirmNextUI.SetActive(true);
                yesButton.GetComponent<Button>().onClick.AddListener(OnYesButtonClicked);
                noButton.GetComponent<Button>().onClick.AddListener(OnNoButtonClicked);
            }
            else if (!BooleanAccessor.istance.GetBoolFromThis("cartello") && !isConversationActive) StartConversation(monologo);
        }
    }

    private void StartConversation(NPCConversation dialog)
    {
        var animator = player.GetComponent<Animator>();
        animator.SetFloat("hInput", 0);
        animator.SetFloat("vInput", 0);

        player.GetComponent<MovementStateManager>().enabled = false;
        clickEndHandled = false;
        isConversationActive = true;
        ConversationManager.Instance.hasClickedEnd = false;
        ConversationManager.Instance.StartConversation(dialog);
    }

    private void OnYesButtonClicked()
    {
        diario.CompletaMissione("Oltrepassa la recinzione");
        GestoreScena.ChangeCursorActiveStatus(false, "FenceHole.yesButton");
        gestoreScena.GoToTransitionScene();
    }

    private void OnNoButtonClicked()
    {
        GestoreScena.ChangeCursorActiveStatus(false, "FenceHole.noButton");
        confirmNextUI.SetActive(false);
        ToggleScripts(false);
    }

    private void ToggleScripts(bool visible) // Simile a GamePlayMenuManager.ToggleScripts
    {
        if (player != null)
        {
            player.GetComponent<ActionStateManager>().enabled = !visible; // Per le azioni (ricarica e switch dell'arma, ecc.)
            player.GetComponent<WeaponClassManager>().enabled = !visible; // Per le armi
            player.GetComponent<OpenInventory>().enabled = !visible; // Per l'inventario
            player.GetComponent<MovementStateManager>().enabled = !visible; // Per il movimento
        }
        if (diario != null) diario.enabled = !visible; // Per il diario
        ConversationManager.Instance.enabled = !visible; // Per le conversazioni
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
