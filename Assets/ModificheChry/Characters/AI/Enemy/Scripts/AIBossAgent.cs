using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIBossAgent : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    [HideInInspector] public string id = ""; // ID dell'IA: utile nel caricamento dei dati per capire se è stato ucciso 
    #endregion

    [Header("State Machine")]
    #region State Machine
    public AIStateId initialState; // Stato iniziale
    public AIStateMachine<AIBossAgent> stateMachine; // Macchina a stati finitia
    #endregion

    [Header("Config AI")]
    #region Config AI
    public float distanceMelee = 2f; // Distanza minima per l'attacco ravvicinato (melee)
    public float minDistanceRange = 5f; // Distanza minima per l'attacco a distanza (range)
    public float maxDistanceRange = 13f; // Distanza massima per l'attacco a distanza (range)
    public float attackCooldown = 2.5f; // Tempo di cooldown tra un attacco e l'altro
    #endregion

    [Header("SFX")]
    #region SFX
    [SerializeField] private AudioClip[] soundsAI; // Suoni dell'IA: 0 = non triggerato, 1 = triggerato, 2 = insegue, 3 = morte
    [HideInInspector] public AudioSource audioSource; // Riferimento all'audio source
    #endregion

    [Header("AI References")]
    #region References
    public Player player; // Riferimento al giocatore
    public GameObject vfxLaser; // Riferimento all'effetto visivo del laser
    public Transform laserSpawnPoint; // Punto di spawn del laser
    public GameObject vfxRing; // Riferimento all'effetto visivo dell'anello
    [HideInInspector] public ProjectileCollision ironBall; // Riferimento al proiettile dell'IA (palla di ferro)
    [HideInInspector] public NavMeshAgent navMeshAgent; // Riferimento all'agente di navigazione
    [HideInInspector] public AIBossStatus status; // Riferimento allo stato dell'IA
    [HideInInspector] public AIBossLocomotion locomotion; // Riferimento al componente di movimento
    [HideInInspector] public Animator animator; // Riferimento all'animatore
    [SerializeField] public LayerMask layerMask; // LayerMask per il rilevamento e gli attacchi
    [HideInInspector] public StygianAttack mutantAttack; // Riferimento al componente script dell'attacco di ogni mutante
    #endregion

    void Start()
    {
        // Al caricamento della scena, se l'agente è stato ucciso precedentemente || il video non è stato ancora visto, disattivalo
        if (GestoreScena.killedEnemyIds.Contains(id) || !BooleanAccessor.istance.GetBoolFromThis("videoMutant")) { gameObject.SetActive(false); return; } // TODO: da scommentare a fine gioco

        navMeshAgent = GetComponent<NavMeshAgent>();
        status = GetComponent<AIBossStatus>();
        animator = GetComponent<Animator>();
        mutantAttack = GetComponent<StygianAttack>();
        locomotion = GetComponent<AIBossLocomotion>();
        audioSource = GetComponent<AudioSource>();
        ironBall = GetComponentInChildren<ProjectileCollision>();

        stateMachine = new AIStateMachine<AIBossAgent>(this);
        stateMachine.RegisterState(new AIBossChasePlayerState());
        stateMachine.RegisterState(new AIBossAttackState());
        stateMachine.RegisterState(new AIBossDeathState());
        stateMachine.ChangeState(initialState);
    }

    void Update() => stateMachine?.Update();

    public void PlayAudio(int index, bool loop)
    {
        StopAudio();
        audioSource.loop = loop;
        audioSource.clip = soundsAI[index];
        audioSource.Play();
    }

    public void StopAudio() => audioSource.Stop();

    public IEnumerator PlayNextAudio(int index)
    {
        if (!status.IsEnemyAlive()) yield break; // Interrompe immediatamente la coroutine
        PlayAudio(index, false);

        yield return new WaitForSeconds(audioSource.clip.length);

        if (!status.IsEnemyAlive()) yield break; // Interrompe immediatamente la coroutine
        PlayAudio(index + 1, true);
    }

    public void AlignToPlayer()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        directionToPlayer.y = 0; // Ignora la componente verticale per evitare inclinazioni
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Interpolazione fluida
        }
    }

    public bool ShouldChase()
    {
        // Definisci un range per determinare la probabilità di inseguire
        // Ad esempio, potremmo voler aumentare la probabilità di inseguire se il mutante è in difficoltà
        // o se il giocatore ha pochi HP, e diminuirla altrimenti
        float chaseProbability = CalculateChaseProbability(status.Health / 700f, player.health / player.maxHealth); // % di HP rimasti per il mutante e per il giocatore
        return Random.Range(0f, 1f) < chaseProbability;
    }

    private float CalculateChaseProbability(float mutantHealthPercentage, float playerHealthPercentage)
    {
        // La probabilità di inseguire varia da 0 a 1
        // Formula che bilancia gli HP del mutante e del giocatore

        // Se il mutante ha poca salute, la probabilità di inseguire aumenta
        float mutantFactor = 1 - mutantHealthPercentage; // Maggiore la salute persa, maggiore la probabilità

        // Se il giocatore ha poca salute, la probabilità di inseguire aumenta
        float playerFactor = 1 - playerHealthPercentage; // Maggiore la salute persa, maggiore la probabilità

        // Combinazione dei due fattori (puoi aggiungere pesi o modificare come preferisci)
        float probability = (mutantFactor + playerFactor) / 2f;

        // Limita la probabilità tra 0 e 1
        return Mathf.Clamp(probability, 0f, 1f);
    }

    public bool Range(float distance, float minDistance, float maxDistance) => distance > minDistance && distance <= maxDistance;
}
