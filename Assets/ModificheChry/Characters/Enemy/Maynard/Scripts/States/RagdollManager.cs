using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    [Header("Ragdoll Settings")]
    #region Ragdoll Settings
    Rigidbody[] rigidbodies;
    #endregion

    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies) rb.isKinematic = true;
    }

    public void TriggerRagdoll()
    {
        foreach (Rigidbody rb in rigidbodies) rb.isKinematic = false;
    }
}
