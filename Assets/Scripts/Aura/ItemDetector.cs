using UnityEngine;
using System.Collections.Generic;

public class ItemDetector : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    [Tooltip("Raggio max per il rilevamento")] public float detectionRadius = 5f; // Raggio massimo per il rilevamento
    #endregion

    [Header("References")]
    #region References
    private Dictionary<GameObject, bool> trackedItems = new(); // Dizionario per tenere traccia degli oggetti già processati
    #endregion

    void Update()
    {
        DetectItemsAround();
    }

    private void DetectItemsAround()
    {
        UpdateListItemsDetection(); // Aggiorna la lista degli oggetti rilevati
        RemoveItemsDetection(); // Rimuovi il componente dagli oggetti non più nella sfera
    }

    private void UpdateListItemsDetection()
    {
        // Ottiene tutti i collider entro il raggio specificato
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (!hitCollider.gameObject.TryGetComponent<ItemController>(out var itemDetect)) continue;

            if (System.Enum.TryParse(itemDetect.item.tagType.ToString(), out Item.ItemTagType _))
            {
                GameObject item = hitCollider.gameObject;
                if (!trackedItems.ContainsKey(item) || !trackedItems[item])
                {
                    if (item.GetComponent<Bordo>() == null) item.AddComponent<Bordo>();
                    trackedItems[item] = true;
                }
            }
        }
    }

    private void RemoveItemsDetection()
    {
        List<GameObject> itemsToRemove = new();
        foreach (var entry in trackedItems)
        {
            if (entry.Key == null) continue;
            if (entry.Value && Vector3.Distance(transform.position, entry.Key.transform.position) > detectionRadius)
            {
                if (entry.Key.TryGetComponent<Bordo>(out var bordo))
                {
                    Destroy(bordo);
                }
                itemsToRemove.Add(entry.Key);
            }
        }

        // Aggiorna il dizionario
        foreach (GameObject item in itemsToRemove)
        {
            trackedItems[item] = false;
            trackedItems.Remove(item);
        }
    }

    public void RemoveItemDetection(GameObject item)
    {
        foreach (var entry in trackedItems)
        {
            if (entry.Key != item) continue;

            if (entry.Key.TryGetComponent<Bordo>(out var bordo))
            {
                Destroy(bordo);
                trackedItems[item] = false;
                trackedItems.Remove(item);
                break;
            }
        }
    }

    private void OnDrawGizmos() // Solo per debug
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
