using UnityEngine;

public class TriggerTargetSheep : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Se l'oggetto che attiva il trigger ha un componente AISheep
        if (other.gameObject.TryGetComponent<AISheep>(out var sheep))
        {
            sheep.SheepInArea();
        }
    }
    
    // OnCollisionExit non dovrebbe servire, in quanto se la pecora entra, andrà direttamente verso "targetEndTask"
}
