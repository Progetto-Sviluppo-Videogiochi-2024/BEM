using UnityEngine;

public class PlayerTriggerDetection : MonoBehaviour
{

    // Metodo chiamato quando un oggetto entra nel trigger
    private void OnTriggerEnter(Collider other)
    {   Debug.Log("Rilevamento:"+other.gameObject.name);
        if (System.Enum.TryParse(other.gameObject.tag, out Item.ItemTagType tags))
        {
            //Debug.Log("Entra");
            other.gameObject.AddComponent<Bordo>();

        }
    }
    // Metodo chiamato quando un oggetto resta all'interno del trigger
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Untagged"))
        {
            //Debug.Log("Oggetto ancora vicino: " + other.gameObject.tag);
            // Puoi aggiungere logica come aggiornare il comportamento in base alla distanza
        }
    }

    // Metodo chiamato quando un oggetto esce dal trigger
    private void OnTriggerExit(Collider other)
    {
        Bordo bordo = other.gameObject.GetComponent<Bordo>();
        Destroy(bordo);
    }
}