using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    [Header("Movement Settings")]
    #region Movement Settings
    public float currentMoveSpeed;
    public float walkSpeed = 3f;
    public float walkBackSpeed = 2f;
    public float runSpeed = 7f;
    public float runBackSpeed = 5f;
    public float crouchSpeed = 2f;
    public float crouchBackSpeed = 1.5f;
    [HideInInspector] public Vector3 moveDirection;
    #endregion

    [Header("Input Settings")]
    #region Input Settings
    [HideInInspector] public float h;
    [HideInInspector] public float v;
    #endregion

    [Header("Gravity Settings")]
    #region Gravity Settings
    [SerializeField] readonly float gravity = -9.81f;
    Vector3 velocity;
    #endregion

    [Header("Ground Check Settings")]
    #region Ground Check Settings
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePosition;
    #endregion

    [Header("Inactivity Settings")]
    #region Inactivity Settings
    private readonly float idleTimeThreshold = 5.0f; // Tempo di inattività
    public float elapsedTime = 0.0f; // Tempo trascorso dall'ultima inattività
    #endregion

    [Header("States")]
    #region States
    public MovementBaseState currentState;
    public IdleState idleState = new();
    public WalkingState walkingState = new();
    public RunningState runningState = new();
    public CrouchState crouchState = new();
    #endregion

    [Header("Audio Settings")]
    #region Audio Settings
    //public AudioClip walkCrouchClip;
    public AudioClip runClip;
    //private float footstepInterval = 0.5f;
    //float footstepTimer;
    #endregion

    [Header("References")]
    #region References
    private CharacterController controller;
    [HideInInspector] public Animator animator;
    private AudioSource audioSource;
    #endregion

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        SwitchState(idleState);
    }

    void Update()
    {
        GetDirectionAndMove();
        Gravity();

        if (!CanInactivity()) ToggleInactivity(animator, false);

        animator.SetFloat("hInput", h);
        animator.SetFloat("vInput", v);

        currentState.UpdateState(this);
        Inactive();
    }

    public void SwitchState(MovementBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);

        if (currentState == crouchState) controller.center = new(controller.center.x, -0.29f, controller.center.z); // testato con questo valore perché stava sopra il terreno else sprofonda un po', magari dovuto al bug dell'animazione?
        else controller.center = new(controller.center.x, 0.0f, controller.center.z);
    }

    private void GetDirectionAndMove()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        moveDirection = transform.forward * v + transform.right * h;
        controller.Move(currentMoveSpeed * Time.deltaTime * moveDirection.normalized);
    }

    private bool IsGrounded()
    {
        spherePosition = new(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if (Physics.CheckSphere(spherePosition, controller.radius - 0.05f, groundMask)) return true;
        else return false;
    }

    private void Gravity()
    {
        if (!IsGrounded()) velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0) velocity.y = -2f; // Piccolo valore per mantenere il personaggio a contatto con il suolo
        controller.Move(velocity * Time.deltaTime);
    }

    private void Inactive()
    {
        if (h != 0 || v != 0) { ToggleInactivity(animator, false); return; }
        if (!animator.GetBool("inactive") && CanInactivity() && CheckInactivityTimer()) ToggleInactivity(animator, true);
    }

    private bool CheckInactivityTimer()
    {
        if (h == 0 && v == 0 && !animator.GetBool("inactive")) elapsedTime += Time.deltaTime;
        else elapsedTime = 0.0f;
        return elapsedTime >= idleTimeThreshold;
    }

    private void ToggleInactivity(Animator animator, bool isInactive)
    {
        elapsedTime = 0.0f;
        animator.SetBool("inactive", isInactive);
        animator.SetInteger("nInactive", 1);
    }

    private bool CanInactivity() =>
        !(animator.GetBool("hasCutWeapon") || animator.GetBool("hasFireWeapon")) // Se il giocatore ha un'arma bianca o da fuoco equipaggiata in mano
            || animator.GetBool("aiming") || animator.GetBool("reloading") // Se il giocatore sta mirando o ricaricando
            || animator.GetBool("pickingUp") // Se il giocatore sta raccogliendo un oggetto
            || h != 0 || v != 0 // Se il giocatore si sta muovendo
            || animator.GetBool("sit"); // Se il giocatore è seduto

    public void PlayRunning()
    {
        audioSource.clip = runClip;
        audioSource.volume = 0.2f;
        if (audioSource.clip != null) audioSource.Play();
        else print("Clip == null");
    }

    public void StopRunning()
    {
        if (audioSource.clip != null) audioSource.Stop();
        else print("Clip == null");
    }
}
