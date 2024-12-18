using UnityEngine;

public class TriggerTargetWolf : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Se l'oggetto che attiva il trigger ha un componente AIWolf
        if (BooleanAccessor.istance.GetBoolFromThis("wolfDone")) return; // Se la quest è completata, non fare nulla
        if (other.gameObject.TryGetComponent<AIWolf>(out var wolf)) wolf.WolfInArea();
    }

    // OnCollisionExit non dovrebbe servire, in quanto se il lupo entra, andrà direttamente verso "targetEndTask"
}
