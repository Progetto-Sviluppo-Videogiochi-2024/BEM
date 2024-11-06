using UnityEngine;
using System.Collections.Generic;

public class ItemDetector : MonoBehaviour
{
    [Header("Settings")]
    public float detectionRadius = 5f; // Raggio massimo per il rilevamento

    // Dizionario per tenere traccia degli oggetti già processati
    private Dictionary<GameObject, bool> trackedItems = new();

    void Update()
    {
        DetectItemsAround();
    }

    void DetectItemsAround()
    {
        // Ottiene tutti i collider entro il raggio specificato
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);

        // Aggiorna la lista degli oggetti rilevati
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.GetComponent<ItemController>() == null) continue;
            var tagItem = hitCollider.gameObject.GetComponent<ItemController>().item.tagType.ToString();
            if (System.Enum.TryParse(tagItem, out Item.ItemTagType _))
            {
                GameObject item = hitCollider.gameObject;

                if (!trackedItems.ContainsKey(item) || !trackedItems[item])
                {
                    if (item.GetComponent<Bordo>() == null)
                    {
                        item.AddComponent<Bordo>();
                        // Debug.Log("Aggiunto Bordo a: " + item.name);
                    }
                    trackedItems[item] = true;
                }
            }
        }

        // Rimuovi il componente dagli oggetti non più nella sfera
        List<GameObject> itemsToRemove = new();
        foreach (var entry in trackedItems)
        {
            if (entry.Value && Vector3.Distance(transform.position, entry.Key.transform.position) > detectionRadius)
            {
                if (entry.Key.TryGetComponent<Bordo>(out var bordo))
                {
                    Destroy(bordo);
                    // Debug.Log("Rimosso Bordo da: " + entry.Key.name);
                }
                itemsToRemove.Add(entry.Key);
            }
        }

        // Aggiorna il dizionario
        foreach (GameObject item in itemsToRemove)
        {
            trackedItems[item] = false;
        }
    }

    private void OnDrawGizmos()
    {
        // Visualizza la sfera di rilevamento nell'editor per il debug
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
