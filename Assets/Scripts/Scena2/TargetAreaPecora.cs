using UnityEngine;

public class SecchioScript : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) // Cambiare il tag a pecora e aggiungere && pecoraIsInArea
        {
            // Fare in modo che la pecora appena raggiunge l'area, raggiunga un punto specifico di Zona Baita (cambiare la destination dell'AI Pecora)
            Debug.Log("La pecora ha raggiunto l'area");
            BooleanAccessor.istance.SetBoolOnDialogueE("acquaDone"); // Cambiare il nome della variabile a pecoraIsInArea       
        }
    }
}
