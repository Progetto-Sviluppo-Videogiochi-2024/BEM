using UnityEngine;

public class BrokenBottle : MonoBehaviour
{
    [SerializeField] GameObject[] pieces;
    [SerializeField] float velMultiplier = 2f;
    // [SerializeField] float timeBeforeDestroying = 6f;

    // void Start() => Destroy(this.gameObject, timeBeforeDestroying); // Se scommentato, si distruggono anziché rimanere in scena (a terra)

    public void RandomVelocities()
    {
        foreach (GameObject piece in pieces)
        {
            float xVel = Random.Range(0f, 1f);
            float yVel = Random.Range(0f, 1f);
            float zVel = Random.Range(0f, 1f);
            Vector3 vel = new(velMultiplier * xVel, velMultiplier * yVel, velMultiplier * zVel);
            piece.GetComponent<Rigidbody>().velocity = vel;
        }
    }
}