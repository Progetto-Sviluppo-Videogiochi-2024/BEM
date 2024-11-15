using UnityEngine;

public class TriggerLeftSign : MonoBehaviour
{
    [Header("References")]
    #region References
    public MilitarySign militarySign;
    #endregion

    private void PlayerTrigger(Collider other, bool canLeft)
    {
        if (other.CompareTag("Player") && BooleanAccessor.istance.GetBoolFromThis("cartello"))
        {
            militarySign.canLeft = canLeft;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerTrigger(other, false);
    }

    void OnTriggerExit(Collider other)
    {
        PlayerTrigger(other, true);
    }
}
