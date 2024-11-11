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
    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;
    #endregion

    [Header("Ground Check Settings")]
    #region Ground Check Settings
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePosition;
    #endregion

    [Header("States")]
    #region States
    public MovementBaseState currentState;
    public IdleState idleState = new();
    public WalkingState walkingState = new();
    public RunningState runningState = new();
    public CrouchState crouchState = new();
    #endregion

    [Header("References")]
    #region References
    private CharacterController controller;
    [HideInInspector] public Animator animator;
    #endregion

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        SwitchState(idleState);
    }

    void Update()
    {
        GetDirectionAndMove();
        Gravity();

        animator.SetFloat("hInput", h);
        animator.SetFloat("vInput", v);

        currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        Inactive();
    }

    public void SwitchState(MovementBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);

        if (currentState == crouchState) controller.center = new Vector3(controller.center.x, -0.29f, controller.center.z); // testato con questo valore perché stava sopra il terreno else sprofonda un po', magari dovuto al bug dell'animazione?
        else controller.center = new Vector3(controller.center.x, 0.0f, controller.center.z);
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
        if (!IsGrounded())
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f; // Piccolo valore per mantenere il personaggio a contatto con il suolo
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void Inactive()
    {
        if (!animator.GetBool("inactive") && CanInactivity())
        {
            animator.SetBool("inactive", true);
            animator.SetInteger("nInactive", 1);
        }
    }

    private bool CanInactivity()
    {
        return !(animator.GetBool("aiming") || animator.GetBool("reloading") // Se il giocatore sta mirando o ricaricando
            || animator.GetBool("pickingUp") // Se il giocatore sta raccogliendo un oggetto
            || animator.GetBool("sit") // Se il giocatore è seduto
            || animator.GetFloat("hInput") != 0 || animator.GetFloat("vInput") != 0 // Se il giocatore si sta muovendo
            || animator.GetBool("hasCutWeapon") || animator.GetBool("hasFireWeapon")); // Se il giocatore ha un'arma bianca o da fuoco equipaggiata in mano
    }
}
