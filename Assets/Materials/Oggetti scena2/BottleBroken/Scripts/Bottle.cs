using UnityEngine;

public class Bottle : MonoBehaviour
{
    [SerializeField] GameObject brokenBottlePrefab;
    
    // void Update() // just for testing
    // {
    //     if (Input.GetKeyDown(KeyCode.Mouse0))
    //     {
    //         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //         if (Physics.Raycast(ray, out RaycastHit hit))
    //         {
    //             if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Shootable"))
    //             {
    //                 Explode(hit.collider.gameObject);
    //             }
    //             // Debug.Log(hit.collider.gameObject.name);
    //         }
    //     }
    // }
    
    public void Explode(GameObject bottle)
    {
        GameObject brokenBottle = Instantiate(brokenBottlePrefab, bottle.transform.position, bottle.transform.rotation);
        brokenBottle.GetComponent<BrokenBottle>().RandomVelocities();
        Destroy(bottle);
    }
}
