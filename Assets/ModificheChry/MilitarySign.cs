using System.Collections;
using DialogueEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MilitarySign : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    private bool clickEndHandled = false; // Flag per evitare che esegua più volte il codice nel metodo Update quando si clicca su "End"
    private bool isConversationActive = false; // Stato della conversazione
    private bool isInRange = false; // Se il player è vicino al cartello per poterci interagire
    private bool isInHand = false; // Se il cartello è in mano al player
    [HideInInspector] public bool canLeft = false; // Se il cartello può essere lasciato (appena si allontana dal trigger)
    #endregion

    [Header("References")]
    #region References
    private Transform player; // Riferimento al player
    public Transform handPlayer; // Posizione in cui il cartello verrà tenuto (mano del player)
    public GameObject tooltip; // Tooltip per indicare che può lasciare il cartello
    private Rigidbody rb; // Rigidbody del cartello
    public Diario diario; // Riferimento al diario
    private NPCConversation monologo; // Dialogo da far partire quando cerca di prendere il cartello
    #endregion

    void Start()
    {
        monologo = GetComponent<NPCConversation>();
        player = FindAnyObjectByType<Player>().transform;
        GetComponent<ItemPickup>().enabled = false;
        tooltip.SetActive(false);
        rb = GetComponent<Rigidbody>();
        SetRigidbody(true);
    }

    void Update()
    {
        if (!clickEndHandled && ConversationManager.Instance.hasClickedEnd) // Se il dialogo è finito
        {
            clickEndHandled = true;
            isConversationActive = false;
            player.GetComponent<MovementStateManager>().enabled = true;
        }

        if (BooleanAccessor.istance.GetBoolFromThis("cartelloDone")) { this.enabled = false; return; } // Se la quest è completata disabilita lo script
        if (!isInRange) return; // Se non ha parlato con Jacob o non è vicino al cartello

        // Gestione del dialogo e del prendere/lasciare il cartello
        HandlePickUpSign();
        HandleDropSign();
    }

    void StartConversation(NPCConversation dialog)
    {
        isConversationActive = true;
        ConversationManager.Instance.StartConversation(dialog);
    }

    private void HandlePickUpSign()
    {
        if (!isInHand && isInRange && Input.GetKeyDown(KeyCode.Space)) // Se non ha il cartello in mano e il player è vicino al cartello e preme spacebar
        {
            ConversationManager.Instance.hasClickedEnd = false;
            var boolAccessor = BooleanAccessor.istance;
            if (boolAccessor.GetBoolFromThis("cartello")) // Se non ha il cartello in mano e ha parlato con Jacob
            {
                GetComponent<ItemPickup>().enabled = false;
                gameObject.transform.SetParent(handPlayer);
                ToggleCollider(false);
                gameObject.transform.SetLocalPositionAndRotation(new(-0.51f, 1.491f, -0.21f), Quaternion.Euler(-236.603f, 46.45799f, -96.40601f));
                isInHand = true;
                SetRigidbody(true);
            }
            if (!boolAccessor.GetBoolFromThis("cartello") && !isConversationActive) // Se non ha parlato con Jacob e il dialogo non è aperto
            {
                ConversationManager.Instance.hasClickedEnd = false;
                StartConversation(monologo);
                player.GetComponent<MovementStateManager>().enabled = false;
            }
        }
    }

    private void HandleDropSign()
    {
        if (isInHand) // Se ha il cartello in mano
        {
            if (canLeft) // Se la posizione in cui lasciarlo è distante da dov'era prima
            {
                if (Input.GetKeyDown(KeyCode.Space)) // Se spacebar
                {
                    var boolAccessor = BooleanAccessor.istance;
                    gameObject.transform.SetParent(null);
                    ToggleCollider(true);
                    gameObject.transform.position += new Vector3(0, 0.1f, 0);
                    isInHand = false;
                    SetRigidbody(false);
                    boolAccessor.SetBoolOnDialogueE("cartelloDone");
                    diario.CompletaMissione("Togli il cartello");
                    Destroy(GetComponent<ItemController>());
                    player.GetComponent<ItemDetector>().RemoveItemDetection(gameObject);
                }
            }
            else // Se la posizione in cui lasciarlo è troppo vicina a dov'era prima
            {
                if (Input.GetKeyDown(KeyCode.Space)) // Se spacebar
                {
                    tooltip.SetActive(true);
                    tooltip.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Qui è ancora troppo in vista";
                    StartCoroutine(DisableTooltip());
                }
            }
        }
    }

    private IEnumerator DisableTooltip()
    {
        yield return new WaitForSeconds(2);
        tooltip.SetActive(false);
    }

    private void ToggleCollider(bool enable)
    {
        Collider[] colliders = GetComponents<BoxCollider>();
        foreach (var collider in colliders)
        {
            if (!collider.isTrigger) collider.enabled = enable;
        }
    }

    private void SetRigidbody(bool _isKinematic)
    {
        rb.isKinematic = _isKinematic;
        rb.useGravity = !_isKinematic;
    }

    private void PlayerTrigger(Collider other, bool _isInRange)
    {
        if (other.CompareTag("Player"))
        {
            var boolAccessor = BooleanAccessor.istance;
            isInRange = _isInRange;
            if (boolAccessor.GetBoolFromThis("cartello") && !boolAccessor.GetBoolFromThis("cartelloDone")) GetComponent<ItemPickup>().enabled = _isInRange;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerTrigger(other, true);
    }

    void OnTriggerExit(Collider other)
    {
        PlayerTrigger(other, false);
    }
}
