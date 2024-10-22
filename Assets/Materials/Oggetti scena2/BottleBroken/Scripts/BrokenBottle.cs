using UnityEngine;

public class BrokenBottle : MonoBehaviour
{
    [SerializeField] GameObject[] pieces;
    [SerializeField] float velMultiplier = 2f;
    [SerializeField] float timeBeforeDestroying = 60f;

    void Start()
    {
        Destroy(this.gameObject, timeBeforeDestroying);
    }
    
    public void RandomVelocities()
    {
        foreach (GameObject piece in pieces)
        {
            float xVel = UnityEngine.Random.Range(0f, 1f);
            float yVel = UnityEngine.Random.Range(0f, 1f);
            float zVel = UnityEngine.Random.Range(0f, 1f);
            Vector3 vel = new(velMultiplier * xVel, velMultiplier * yVel, velMultiplier * zVel);
            piece.GetComponent<Rigidbody>().velocity = vel;
        }
    }
}