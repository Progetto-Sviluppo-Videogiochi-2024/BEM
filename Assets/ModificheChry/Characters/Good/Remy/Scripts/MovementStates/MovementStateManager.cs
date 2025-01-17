using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    [Header("Movement Settings")]
    #region Movement Settings
    public float currentMoveSpeed; // Velocità di movimento attuale
    public float walkSpeed = 3f; // Velocità di camminata
    public float walkBackSpeed = 2f; // Velocità di camminata all'indietro
    public float runSpeed = 7f; // Velocità di corsa
    public float runBackSpeed = 5f; // Velocità di corsa all'indietro
    public float crouchSpeed = 2f; // Velocità di movimento in stealth
    public float crouchBackSpeed = 1.5f; // Velocità di movimento in stealth all'indietro
    [HideInInspector] public Vector3 moveDirection; // Direzione di movimento
    #endregion

    [Header("Input Settings")]
    #region Input Settings
    [HideInInspector] public float h; // Input per l'asse orizzontale (AD)
    [HideInInspector] public float v; // Input per l'asse verticale (WS)
    #endregion

    [Header("Gravity Settings")]
    #region Gravity Settings
    [SerializeField] readonly float gravity = -9.81f; // Gravità
    Vector3 velocity; // Velocità
    #endregion

    [Header("Ground Check Settings")]
    #region Ground Check Settings
    [SerializeField] float groundYOffset; // Offset per il controllo del terreno
    [SerializeField] LayerMask groundMask; // Maschera per il terreno
    Vector3 spherePosition; // Posizione della sfera per il controllo del terreno
    #endregion

    [Header("Inactivity Settings")]
    #region Inactivity Settings
    private readonly float maxTimeInactivity = 5.0f; // Timer di inattività
    public float timerInactivity = 0.0f; // Tempo trascorso dall'ultima inattività
    #endregion

    [Header("States")]
    #region States
    public MovementBaseState currentState; // Stato attuale
    public IdleState idleState = new(); // Stato di movimento per l'inattività
    public WalkingState walkingState = new(); // Stato di movimento per la camminata
    public RunningState runningState = new(); // Stato di movimento per la corsa
    public CrouchState crouchState = new(); // Stato di movimento per lo stealth
    #endregion

    [Header("Audio Settings")]
    #region Audio Settings
    public AudioClip runClip; // Clip per la corsa
    #endregion

    [Header("References")]
    #region References
    private CharacterController controller; // Controller per il movimento
    [HideInInspector] public Animator animator; // Animator per le animazioni
    private AudioSource audioSource; // AudioSource per l'audio
    [HideInInspector] public SphereCollider noiseAura; // Trigger per il rilevamento acustico del nemico
    #endregion

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        noiseAura = GetComponent<SphereCollider>();

        SwitchState(idleState);
    }

    void Update()
    {
        GetDirectionAndMove();
        Gravity();

        if (!CanInactivity()) ToggleInactivity(animator, false);
        else if (CheckInactivityTimer()) Inactive();

        animator.SetFloat("hInput", h);
        animator.SetFloat("vInput", v);

        currentState.UpdateState(this);
    }

    // Per gli stati di movimento
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

    // Per la gravità
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

    // Per l'inattività
    private void Inactive()
    {
        if (!animator.GetBool("inactive") && CheckInactivityTimer()) { timerInactivity = 0; animator.SetInteger("nInactive", 1); animator.SetBool("inactive", true); }
        else if (!IsIdleFinished()) return; // Se è in corso, non fare nulla
        else if (CheckInactivityTimer()) CycleToNextInactivity();
    }

    private bool IsMoving() => h != 0 || v != 0; // Si muove quindi

    private bool CheckInactivityTimer()
    {
        // Se viene rilevato un input da tastiera, resetta il timer
        if (Input.anyKey || Input.GetMouseButton(0) || Input.GetMouseButton(1) ||
        Mathf.Abs(Input.GetAxis("Mouse X")) > 0.01f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.01f)
        {
            timerInactivity = 0.0f;
            return false;
        }

        // Incrementa il timer di inattività
        timerInactivity += Time.deltaTime;

        // Ritorna true solo se supera il tempo massimo di inattività
        return timerInactivity >= maxTimeInactivity;
    }

    private void ToggleInactivity(Animator animator, bool isInactive)
    {
        timerInactivity = 0.0f;
        animator.SetBool("inactive", isInactive);
        animator.SetInteger("nInactive", 0);
    }

    private void CycleToNextInactivity()
    {
        timerInactivity = 0.0f;
        if (animator.GetBool("sit")) animator.SetInteger("nInactive", animator.GetInteger("nInactive") == 1 ? 2 : 1); // Cicla 1, 2 se è seduto
        else animator.SetInteger("nInactive", animator.GetInteger("nInactive") == 1 ? 3 : 1); // Cicla 1, 3 se non è seduto
    }

    private bool CanInactivity() =>
        !(animator.GetBool("hasFireWeapon") // Se il giocatore ha un'arma da fuoco equipaggiata in mano (la ricarica è implicita nell'avere l'arma in mano o equipaggiata)
            || animator.GetBool("aiming") // Se il giocatore sta mirando
            || animator.GetBool("pickingUp") // Se il giocatore sta raccogliendo un oggetto
            || IsMoving() // Se il giocatore si sta muovendo
            || animator.GetBool("sit")); // Se il giocatore è seduto

    private bool TakeBoolIdle(AnimatorStateInfo stateInfo, string boolAnimName) => stateInfo.IsName(boolAnimName) && stateInfo.normalizedTime >= 1f;

    private bool IsIdleFinished()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(3); // Indice del "Layer" dell'animator
        return TakeBoolIdle(stateInfo, "Shoulder Rubbing Inactive") ||
            TakeBoolIdle(stateInfo, "Listening To Music Inactive") ||
            TakeBoolIdle(stateInfo, "Looking Around Inactive");
    }

    // Per l'SFX
    public void PlayRunning()
    {
        audioSource.clip = runClip;
        audioSource.volume = 0.15f;
        if (audioSource.clip != null) audioSource.Play();
    }

    public void StopRunning()
    {
        if (audioSource.clip != null) audioSource.Stop();
    }
}
