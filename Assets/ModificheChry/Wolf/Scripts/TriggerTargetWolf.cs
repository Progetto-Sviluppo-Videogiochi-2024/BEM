using UnityEngine;

public class TriggerTargetWolf : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Se l'oggetto che attiva il trigger ha un componente AIWolf
        if (other.gameObject.TryGetComponent<AIWolf>(out var wolf)) wolf.WolfInArea();
    }

    // OnCollisionExit non dovrebbe servire, in quanto se la pecora entra, andrà direttamente verso "targetEndTask"
}
