using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    [HideInInspector] public int damage; // Danno inflitto al giocatore
    [HideInInspector] public bool isActive = false; // Stato di attivazione dell'attacco di lancio della palla di ferro del boss

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return; // Se il proiettile non è attivo, non fare nulla

        if (other.gameObject.CompareTag("Player")) // Se colpisce il player
        {
            Player player = other.GetComponent<Player>();
            player?.UpdateStatusPlayer(-damage, -5); // Esempio di metodo per applicare il danno
        }
    }
}
