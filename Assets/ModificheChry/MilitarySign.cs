using System.Collections;
using DialogueEditor;
using TMPro;
using UnityEngine;

public class MilitarySign : NPCDialogueBase
{
    [Header("Settings")]
    #region Settings
    private bool isInRange = false; // Se il player è vicino al cartello
    private bool isInHand = false; // Se il cartello è in mano al player
    [HideInInspector] public bool canLeft = false; // Se il cartello può essere lasciato (appena si allontana dal trigger)
    #endregion

    [Header("References")]
    #region References
    public Transform handPlayer; // Posizione in cui il cartello verrà tenuto (mano del player)
    public GameObject tooltip; // Tooltip per indicare che può lasciare il cartello
    private Rigidbody rb; // Rigidbody del cartello
    public Diario diario; // Riferimento al diario
    public NPCConversation monologo; // Dialogo da far partire quando cerca di prendere il cartello
    #endregion

    void Start()
    {
        GetComponent<ItemPickup>().enabled = false;
        tooltip.SetActive(false);
        rb = GetComponent<Rigidbody>();
        SetRigidbody(true);
    }

    void Update()
    {
        var boolAccessor = BooleanAccessor.istance;
        if (boolAccessor.GetBoolFromThis("cartelloDone")) { this.enabled = false; return; } // Se la quest è completata disabilita lo script
        if (!boolAccessor.GetBoolFromThis("cartello") || !isInRange) return; // Se non ha parlato con Jacob o non è vicino al cartello

        if (!isInHand && isInRange && Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<ItemPickup>().enabled = false;
            gameObject.transform.SetParent(handPlayer);
            gameObject.transform.SetLocalPositionAndRotation(new(-0.51f, 1.491f, -0.21f), Quaternion.Euler(-236.603f, 46.45799f, -96.40601f));
            isInHand = true;
            SetRigidbody(true);
        }

        if (isInHand)
        {
            if (canLeft) // Se la posizione in cui lasciarlo è distante da dov'era prima
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    gameObject.transform.SetParent(null);
                    gameObject.transform.position += new Vector3(0, 0.1f, 0);
                    isInHand = false;
                    SetRigidbody(false);
                    boolAccessor.SetBoolOnDialogueE("cartelloDone");
                    diario.CompletaMissione("Togli il cartello");
                }
            }
            else // Se la posizione in cui lasciarlo è troppo vicina a dov'era prima
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    tooltip.SetActive(true);
                    tooltip.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Qui è ancora troppo in vista";
                    StartCoroutine(DisableTooltip());
                }
            }
        }
    }

    protected override void StartDialogue()
    {
        StartConversation(monologo);
    }

    private IEnumerator DisableTooltip()
    {
        yield return new WaitForSeconds(2);
        tooltip.SetActive(false);
    }

    private void SetRigidbody(bool _isKinematic)
    {
        rb.isKinematic = _isKinematic;
        rb.useGravity = !_isKinematic;
    }

    private void PlayerTrigger(Collider other, bool _isInRange)
    {
        if (other.CompareTag("Player") && BooleanAccessor.istance.GetBoolFromThis("cartello"))
        {
            isInRange = _isInRange;
            GetComponent<ItemPickup>().enabled = _isInRange;
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
