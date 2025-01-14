using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    Rigidbody[] rigidbodies; // Lista di rigidbodies da attivare per il ragdoll
    #endregion

    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies) rb.isKinematic = true; // Imposta tutti i rigidbodies come kinematici (=> non influenzati dalla fisica)
    }

    public void TriggerRagdoll()
    {
        foreach (Rigidbody rb in rigidbodies) rb.isKinematic = false; // Imposta tutti i rigidbodies come non kinematici (=> influenzati dalla fisica)
    }
}
