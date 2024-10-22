using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTriggerDetection : MonoBehaviour
{
   
    // Metodo chiamato quando un oggetto entra nel trigger
    private void OnTriggerEnter(Collider other)
    {
        Item.ItemTagType tags;
        if (System.Enum.TryParse(other.gameObject.tag,out tags)){
           Debug.Log("Entra");
           other.gameObject.AddComponent<Bordo>();

        }
    }
    // Metodo chiamato quando un oggetto resta all'interno del trigger
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag!="Untagged")
        {
            Debug.Log("Oggetto ancora vicino: " + other.gameObject.tag);
            // Puoi aggiungere logica come aggiornare il comportamento in base alla distanza
        }
    }

    // Metodo chiamato quando un oggetto esce dal trigger
    private void OnTriggerExit(Collider other)
    {
        Bordo bordo=other.gameObject.GetComponent<Bordo>();
        Destroy(bordo);
    }
}