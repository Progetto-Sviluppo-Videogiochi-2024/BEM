using UnityEngine;

public class VanScript : MonoBehaviour
{
    public float speed = 40f; // Velocità di movimento del van

    void Update()
    {
        // Aggiorna la posizione del van
        transform.position += Vector3.forward * speed * 10 * Time.deltaTime;
    }

    /*
    public Transform ruota_avanti_destra;
        void Update()
        {

        Vector3 movementDirection = new Vector3(0,0,1);
           print(movementDirection.normalized);
           transform.Translate(movementDirection.normalized * 8 * Time.deltaTime);
           //transform.Rotate(0, Input.GetAxisRaw("Horizontal") * Input.GetAxisRaw("Vertical") * 40 * Time.deltaTime, 0);
           //ruota_avanti_destra.Rotate(0,0,Input.GetAxisRaw("Horizontal") * Input.GetAxisRaw("Vertical") * 40 * Time.deltaTime);
        }
    */
}
