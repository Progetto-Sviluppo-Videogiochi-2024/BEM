using UnityEngine;

public class TriggerHideOutGaia : MonoBehaviour
{
    bool isGaiaHidden = false;

    void OnTriggerEnter(Collider other)
    {
        if (!isGaiaHidden && other.transform.name.Contains("Gaia"))
        {
            other.GetComponent<AIGaiaNPC>().enabled = false;
            other.GetComponent<Animator>().SetBool("crouching", true);
            isGaiaHidden = true;
        }
    }
}
