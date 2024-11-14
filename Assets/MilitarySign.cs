using TMPro;
using UnityEngine;

public class MilitarySign : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    private bool isInRange = false; // Se il player è vicino al cartello
    private bool isInHand = false; // Se il cartello è in mano al player
    private Transform signPosition; // Posizione iniziale del cartello
    #endregion

    [Header("References")]
    #region References
    public Transform handPlayer; // Posizione in cui il cartello verrà tenuto (mano del player)
    public GameObject tooltip; // Tooltip per indicare che può lasciare il cartello
    #endregion

    void Start()
    {
        tooltip.SetActive(false);
        signPosition = gameObject.transform;
    }

    void Update()
    {
        var boolAccessor = BooleanAccessor.istance;
        if (boolAccessor.GetBoolFromThis("cartelloDone")) {this.enabled = false; return; } // Se la quest è completata disabilita lo script
        if (!boolAccessor.GetBoolFromThis("cartello") || !isInRange) return; // Se non ha parlato con Jacob o non è vicino al cartello
        
        print("secondo if passato");
        if (!isInHand && isInRange && Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.transform.SetParent(handPlayer);
            isInHand = true;
            print("Cartello in mano");
        }

        if (isInHand)
        {
            float distance = Vector3.Distance(signPosition.position, handPlayer.root.position);
            if (distance > 10f) // TODO: da testare se 10f è troppo distante
            {
                print("Posso lasciare il cartello");
                tooltip.SetActive(true);
                tooltip.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Qui posso lasciare il cartello";
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    print("Cartello lasciato");
                    gameObject.transform.SetParent(null);
                    isInHand = false;
                    tooltip.SetActive(false);
                    boolAccessor.SetBoolOnDialogueE("cartelloDone");
                }
            }
            else // Se la posizione in cui lasciarlo è troppo vicina a dov'era prima
            {
                tooltip.SetActive(false);
            }
        }
    }

    private void PlayerTrigger(Collider other, bool _isInRange)
    {
        if (other.CompareTag("Player") && BooleanAccessor.istance.GetBoolFromThis("cartello"))
        {
            isInRange = _isInRange;
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

    // appena il pg si avvicina e preme space il cartello scompare e riappare sulle mani del pg
    // a una certa distanza da dove era prima comparirà il tooltip con il testo "Premere E per lasciare il cartello"
    // aggiornare il DE
}
