using UnityEngine;
using Cinemachine;
using UnityEngine.Animations.Rigging;

public class AimStateManager : MonoBehaviour
{
    [Header("Mouse Settings")]
    #region Mouse Settings
    [SerializeField] float mouseSense = 1;
    float xAxis;
    float yAxis;
    private Transform camFollowPosition;
    #endregion

    [Header("Camera Aim Settings")]
    #region Camera Aim Settings
    [HideInInspector] public CinemachineVirtualCamera aimCam;
    public float aimFov = 40;
    [HideInInspector] public float idleFov;
    [HideInInspector] public float currentFov;
    public float fovSmoothSpeed = 10;

    float xFollowPosition;
    float yFollowPosition;
    float ogYposition;
    [SerializeField] float crouchCamHeight = 0.6f;
    [SerializeField] float shoulderSwapSpeed = 10;
    #endregion

    [Header("Aim Settings")]
    #region Aim Settings
    [HideInInspector] public Transform aimPos;
    [SerializeField] float aimSmoothSpeed = 20;
    [SerializeField] LayerMask aimMask;
    #endregion

    [Header("States")]
    #region States
    public AimBaseState currentState;
    public RifleIdleState rifleIdleState = new();
    public RifleIdleAimState rifleIdleAimState = new();
    #endregion

    [Header("Animation rigging settings")]
    #region Animation rigging settings
    MultiAimConstraint[] multiAimConstraints;
    WeightedTransform aimPositionWeightedTransform;
    #endregion

    [Header("References Scripts")]
    #region References Scripts
    MovementStateManager movement;
    #endregion

    [Header("References")]
    #region References
    [HideInInspector] public Animator animator;
    public GameObject crosshair;
    #endregion

    private void Awake()
    {
        if (aimPos != null) return; // aggiunto io, perché mi scoccio a resettare tutte le variabili della Scena
        aimPos = new GameObject("Aim Position").transform;
        aimPos.name = "AimPosition";

        aimPositionWeightedTransform.transform = aimPos;
        aimPositionWeightedTransform.weight = 1;

        multiAimConstraints = GetComponentsInChildren<MultiAimConstraint>();
        foreach (var multiAimConstraint in multiAimConstraints)
        {
            var data = multiAimConstraint.data.sourceObjects;
            data.Clear();
            data.Add(aimPositionWeightedTransform);
            multiAimConstraint.data.sourceObjects = data;
        }
    }

    void Start()
    {
        camFollowPosition = transform.GetChild(0); // Prende il primo figlio del player, il transform di CameraFollowPosition
        if (crosshair != null) crosshair.SetActive(false);
        movement = GetComponent<MovementStateManager>();
        xFollowPosition = camFollowPosition.localPosition.x;
        ogYposition = camFollowPosition.localPosition.y;
        yFollowPosition = ogYposition;
        aimCam = GetComponentInChildren<CinemachineVirtualCamera>();
        animator = GetComponent<Animator>();

        idleFov = aimCam.m_Lens.FieldOfView;
        SwitchState(rifleIdleState);
    }

    void Update()
    {
        xAxis += Input.GetAxisRaw("Mouse X") * mouseSense;
        yAxis -= Input.GetAxisRaw("Mouse Y") * mouseSense;
        yAxis = Mathf.Clamp(yAxis, -80, 80);

        aimCam.m_Lens.FieldOfView = Mathf.Lerp(aimCam.m_Lens.FieldOfView, currentFov, Time.deltaTime * fovSmoothSpeed);

        // Vector2 screenCentre = new(Screen.width / 2, Screen.height / 2);
        // Ray ray = Camera.main.ScreenPointToRay(screenCentre);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            // aimPos.position = hit.point;
            aimPos.position = Vector3.Lerp(aimPos.position, hit.point, Time.deltaTime * aimSmoothSpeed);
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red); // TODO: da togliere poi
        }
        // else se gira la visuale, aimPos FISSA poco più avanti del pg per evitare che faccia strani movimenti 

        // if (Input.GetMouseButton(0))
        // {
        //     crosshair.SetActive(true);
        //     //UpdateCrosshairPosition();
        // }
        // else
        // {
        //     crosshair.SetActive(false);
        // }

        MoveCamera();

        currentState.UpdateState(this);
    }

    private void LateUpdate()
    {
        camFollowPosition.localEulerAngles = new(yAxis, camFollowPosition.localEulerAngles.y, camFollowPosition.localEulerAngles.z);
        transform.eulerAngles = new(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
    }

    public void SwitchState(AimBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    void MoveCamera()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt)) xFollowPosition = -xFollowPosition;
        if (movement.currentState == movement.crouchState) yFollowPosition = crouchCamHeight;
        else yFollowPosition = ogYposition;

        Vector3 newFollowPosition = new(xFollowPosition, yFollowPosition, camFollowPosition.localPosition.z);
        camFollowPosition.localPosition = Vector3.Lerp(camFollowPosition.localPosition, newFollowPosition, Time.deltaTime * shoulderSwapSpeed);
    }
}
