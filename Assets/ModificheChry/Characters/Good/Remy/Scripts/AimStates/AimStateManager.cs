using UnityEngine;
using Cinemachine;

public class AimStateManager : MonoBehaviour
{
    [Header("Mouse Settings")]
    #region Mouse Settings
    [SerializeField] float mouseSense = 1;
    float xAxis;
    float yAxis;
    [HideInInspector] public Transform camFollowPosition;
    #endregion

    [Header("Camera General Settings")]
    #region Camera General Settings
    [HideInInspector] public CinemachineVirtualCamera aimCam;
    public float fovSmoothSpeed = 10;
    #endregion

    [Header("Camera Fov Settings")]
    #region Camera Fov Settings
    public float aimFov = 40;
    [HideInInspector] public float idleFov;
    [HideInInspector] public float currentFov;
    #endregion

    [Header("Camera Position Settings")]
    #region Camera Position Settings
    [SerializeField] float crouchXposition = 1f;  // Offset orizzontale (asse X) per il crouch
    [SerializeField] public float crouchYPosition = 0.6f; // Offset verticale (asse Y) per il crouch
    [SerializeField] float crouchZPosition = -6f;  // Posizione lungo l'asse Z per il crouch
    private float walkXPosition; // Offset orizzontale (asse X) per il walk
    private float walkYPosition; // Offset verticale (asse Y) per il walk
    private float walkZPosition; // Posizione lungo l'asse Z per il walk
    [SerializeField] float shoulderSwapSpeed = 10;
    #endregion

    [Header("Aim Settings")]
    #region Aim Settings
    public Transform aimPos;
    [SerializeField] float aimSmoothSpeed = 20;
    [SerializeField] LayerMask aimMask;
    #endregion

    [Header("States")]
    #region States
    public AimBaseState currentState;
    public RifleIdleState rifleIdleState = new();
    public RifleIdleAimState rifleIdleAimState = new();
    #endregion

    [Header("References Scripts")]
    #region References Scripts
    public RigSwitcher cambiaRig;
    MovementStateManager movement;
    #endregion

    [Header("References")]
    #region References
    public GameObject crosshair;
    [HideInInspector] public Animator animator;
    private Player player;
    public Transform torch;
    #endregion

    public float offsetXTorch = 0.05f;
    public float offsetZTorch = -0.05f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        aimCam = GetComponentInChildren<CinemachineVirtualCamera>();
        movement = GetComponent<MovementStateManager>();
    }

    void Start()
    {
        player = GetComponent<Player>();
        crosshair?.SetActive(false);

        camFollowPosition = transform.GetChild(0); // Transform di CameraFollowPosition
        idleFov = aimCam.m_Lens.FieldOfView;
        walkXPosition = camFollowPosition.localPosition.x;
        walkYPosition = camFollowPosition.localPosition.y;
        walkZPosition = camFollowPosition.localPosition.z;

        //SwitchState(rifleIdleState);
        currentFov = idleFov;
        cambiaRig = GetComponentInChildren<RigSwitcher>();
    }

    void Update()
    {
        if (animator.GetBool("sit")) return;
        if (player.isDead) return;

        xAxis += Input.GetAxisRaw("Mouse X") * mouseSense;
        yAxis -= Input.GetAxisRaw("Mouse Y") * mouseSense;
        yAxis = Mathf.Clamp(yAxis, -80, 80);

        aimCam.m_Lens.FieldOfView = Mathf.Lerp(aimCam.m_Lens.FieldOfView, currentFov, Time.deltaTime * fovSmoothSpeed);

        Ray ray = Camera.main.ScreenPointToRay(new(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            aimPos.position = Vector3.Lerp(aimPos.position, hit.point, Time.deltaTime * aimSmoothSpeed);
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red); // TODO: da togliere poi
        }

        MoveCamera();

        currentState?.UpdateState(this);
    }

    private void LateUpdate()
    {
        if (animator.GetBool("sit")) return;

        if (!player.isDead)
        {
            camFollowPosition.localEulerAngles = new(yAxis, camFollowPosition.localEulerAngles.y, camFollowPosition.localEulerAngles.z);
            transform.eulerAngles = new(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);

            if (torch != null) MoveTorch();
        }
        else HandleCameraDeath();
    }

    public void SwitchState(AimBaseState newState)
    {
        currentState = newState;
        currentState?.EnterState(this);
    }

    void MoveCamera()
    {
        if (movement.currentState == movement.crouchState)
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt)) crouchXposition = -crouchXposition;
            camFollowPosition.localPosition = new(crouchXposition, crouchYPosition, crouchZPosition);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt)) walkXPosition = -walkXPosition;
            camFollowPosition.localPosition = new(walkXPosition, walkYPosition, walkZPosition);
        }

        // Calcolo della nuova posizione della telecamera
        Vector3 newFollowPosition = new(camFollowPosition.localPosition.x, camFollowPosition.localPosition.y, camFollowPosition.localPosition.z);

        // Movimentazione fluida verso la nuova posizione
        camFollowPosition.localPosition = Vector3.Lerp(camFollowPosition.localPosition, newFollowPosition, Time.deltaTime * shoulderSwapSpeed);
    }

    private void HandleCameraDeath()
    {
        // 1. Parametri configurabili
        float altezzaSopraStefano = 2f;  // Altezza della telecamera sopra Stefano
        float distanzaDietroStefano = 0.5f; // Quanto la telecamera è dietro Stefano
        float smoothSpeed = 5f; // Velocità di transizione della camera

        // 2. Posizione della camera sopra Stefano
        Vector3 nuovaPosizione = player.transform.position
                               + Vector3.up * altezzaSopraStefano
                               - player.transform.forward * distanzaDietroStefano;

        // 3. Movimentazione fluida verso la nuova posizione
        camFollowPosition.position = Vector3.Lerp(camFollowPosition.position, nuovaPosizione, Time.deltaTime * smoothSpeed);

        // 4. La telecamera guarda sempre Stefano
        camFollowPosition.LookAt(player.transform.position + Vector3.up * (altezzaSopraStefano / 2));
    }

    void MoveTorch()
    {
        // Aggiungi un offset alla posizione della torcia per mantenerla a una distanza desiderata dalla telecamera
        Vector3 torchOffset = new(offsetXTorch, 0f, offsetZTorch);  // Usa offsetXTorch per lo spostamento laterale

        // Muovi la torcia per seguire la rotazione della telecamera
        torch.SetPositionAndRotation(
            camFollowPosition.position
                + (camFollowPosition.right * torchOffset.x) // Sposta lateralmente (destra con positivo, sinistra con negativo)
                + (camFollowPosition.forward * torchOffset.z), // Sposta in avanti
            Quaternion.Euler(camFollowPosition.eulerAngles.x, camFollowPosition.eulerAngles.y, 0));
    }
}
