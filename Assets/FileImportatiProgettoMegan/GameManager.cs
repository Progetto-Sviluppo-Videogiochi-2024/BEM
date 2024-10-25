using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform playerHead;

    private void Awake()
    {
        var camera = Camera.main.gameObject;
        if (camera.GetComponent<CinemachineBrain>() == null) camera.AddComponent<CinemachineBrain>();
        // else Debug.LogWarning("CinemachineBrain already exists.");
    }

    // private void Start() // TODO: anche eliminabile se non disabilitiamo il cursore del mouse
    // {
    //     Cursor.lockState = CursorLockMode.Locked;
    //     Cursor.visible = false;
    // }
}