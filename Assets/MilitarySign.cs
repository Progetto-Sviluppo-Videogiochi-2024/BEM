using TMPro;
using UnityEngine;

public class MilitarySign : MonoBehaviour
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
    #endregion

    void Start()
    {
        GetComponent<ItemPickup>().enabled = false;
        tooltip.SetActive(false);
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
            isInHand = true;
        }

        if (isInHand)
        {
            if (canLeft)
            {
                tooltip.SetActive(true);
                tooltip.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Qui posso lasciare il cartello";
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    gameObject.transform.SetParent(null);
                    isInHand = false;
                    tooltip.SetActive(false);
                    boolAccessor.SetBoolOnDialogueE("cartelloDone");
                }
            }
            else tooltip.SetActive(false); // Se la posizione in cui lasciarlo è troppo vicina a dov'era prima
        }
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
