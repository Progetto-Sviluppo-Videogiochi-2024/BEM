using UnityEngine;

public class TriggerPlayer : MonoBehaviour
{
    public bool isPlayerPassed = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerPassed = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerPassed = false;
        }
    }
}
