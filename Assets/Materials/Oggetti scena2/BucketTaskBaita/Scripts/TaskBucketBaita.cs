using UnityEngine;

public class TaskBucketBaita : MonoBehaviour
{
    public Material waterMaterial; // Indica il materiale dell'acqua
    private GameObject internalBucket; // Oggetto interno del secchio (l'acqua, se è pieno)
    private Renderer internalBucketRenderer; // Renderer dell'oggetto interno
    private bool isFilled = false; // Indica se il secchio è pieno
    public Transform weaponHolder; // Indica la posizione in cui il secchio appare dopo il pickup (mano del giocatore)
    private bool canPickup = false; // Indica se il giocatore può raccogliere il secchio
    private bool isPicked = false; // Indica se il secchio è stato raccolto

    void Start()
    {
        // Ottieni il renderer dell'oggetto interno
        internalBucket = transform.Find("Internal").gameObject;
        internalBucketRenderer = internalBucket.GetComponent<Renderer>();
        internalBucketRenderer.enabled = false;
    }

    void Update()
    {
        if (!isFilled && canPickup && !isPicked && Input.GetKeyDown(KeyCode.Space))
        {
            isPicked = true;
            canPickup = false;
            this.gameObject.transform.SetParent(weaponHolder);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().taskBucketBaita = this;
        }
    }

    public void Fill()
    {
        internalBucketRenderer.enabled = true;
        internalBucketRenderer.material = waterMaterial;
        isFilled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isPicked)
        {
            canPickup = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canPickup = false;
        }
    }
}
