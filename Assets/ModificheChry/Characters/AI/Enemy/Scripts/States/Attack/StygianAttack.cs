using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StygianAttack : MonoBehaviour
{
    [HideInInspector] public bool isAttacking = false; // Indica se l'AI sta attaccando
    [HideInInspector] public bool hasSpasm = false; // Indica se il mutante si è triggerato
    public float timeSinceLastAttack = 0f; // Tempo trascorso dall'ultimo attacco

    public int damagePunch = 20;
    public int damageJumpBall = 30;
    public int damagePunchBall = 25;
    public int damageThrowBall = 10;
    public int damageShootLaser = 30;
    public List<float> rageThresholds = new() { 0.2f, 0.1f }; // Percentuali (dal valore più alto al più basso)

    public AIBossAttackState AttackState { get; set; }
    public AIBossAgent Agent { get; set; }
    public string CurrentAttack { get; set; }
    public int CurrentDamage { get; set; }
    Dictionary<string, (float probability, int damage)> MeleeAttacks { get; } = new()
    {
        { "punch", (40f, 0) }, // Temporanei
        { "jumpBall", (35f, 0) },
        { "punchBall", (25f, 0) },
        { "spasm", (0f, 0) } // Solo quando arriva a 10-20% di vita e si triggera per aumentare la potenza di attacco
    };

    Dictionary<string, (float probability, int damage)> RangeAttacks { get; } = new()
    {
        { "telepathic", (25f, 0) }, // Temporanei
        { "throwBall", (60f, 0) },
        { "shootLaser", (15f, 0) },
        { "spasm", (0f, 0) } // Solo quando arriva a 10-20% di vita e si triggera per aumentare la potenza di attacco
    };

    void Start()
    {
        SetDamageAttacks();
    }

    void SetDamageAttacks()
    {
        MeleeAttacks["punch"] = (MeleeAttacks["punch"].probability, damagePunch);
        MeleeAttacks["jumpBall"] = (MeleeAttacks["jumpBall"].probability, damageJumpBall);
        MeleeAttacks["punchBall"] = (MeleeAttacks["punchBall"].probability, damagePunchBall);
        RangeAttacks["throwBall"] = (RangeAttacks["throwBall"].probability, damageThrowBall);
        RangeAttacks["shootLaser"] = (RangeAttacks["shootLaser"].probability, damageShootLaser);
        // spasm rimane invariato perché si ha quando il mutante si triggera e aumenta la sua potenza di attacco
    }

    void SetCurrentAttackDamage(AIBossAgent agent, Dictionary<string, (float probability, int damage)> attacks)
    {
        isAttacking = true; // L'AI è ora in stato di attacco
        agent.navMeshAgent.isStopped = true; // Blocca il movimento durante l'attacco

        // Calcola l'attacco basato sulle probabilità
        string selectedAttack = GetAttackByProbability(attacks);
        CurrentAttack = selectedAttack;
        CurrentDamage = attacks[selectedAttack].damage;

        if (!hasSpasm && rageThresholds.Count > 0 && agent.status.Health <= rageThresholds[0]) CurrentAttack = "spasm";
    }

    string GetAttackByProbability(Dictionary<string, (float probability, int damage)> attacks)
    {
        float totalProbability = attacks.Values.Sum(x => x.probability);
        float randomValue = Random.Range(0f, totalProbability);
        float cumulative = 0f;

        foreach (var attack in attacks)
        {
            cumulative += attack.Value.probability;
            if (randomValue <= cumulative)
            {
                return attack.Key;
            }
        }
        return attacks.Keys.First(); // Fall-back di sicurezza (non dovrebbe mai accadere)
    }

    public void PerformMeleeAttack(AIBossAgent agent)
    {
        agent.AlignToPlayer();
        SetCurrentAttackDamage(agent, MeleeAttacks);
        print($"Melee: {CurrentAttack}");

        switch (CurrentAttack)
        {
            case "punch":
            case "jumpBall":
            case "punchBall":
                agent.animator.SetTrigger(CurrentAttack);
                // ApplyDamage(); // Invocato già dall'animazione
                break;

            case "spasm":
                hasSpasm = true;
                agent.animator.SetTrigger(CurrentAttack);
                agent.player.UpdateStatusPlayer(0, -5);
                if (rageThresholds.Count > 0)
                {
                    PerformSpasm(rageThresholds.Count == 2 ? 2 : 5); // Aumenta la potenza di attacco
                    rageThresholds.RemoveAt(0); // Rimuove il valore più alto
                }
                break;

            default:
                Debug.LogWarning($"Attack melee {CurrentAttack} not exists!");
                break;
        }
    }

    public void PerformRangeAttack(AIBossAgent agent)
    {
        agent.AlignToPlayer();
        SetCurrentAttackDamage(agent, RangeAttacks);
        print($"Range: {CurrentAttack}");

        switch (CurrentAttack)
        {
            // TODO: implementare la SFX VFX e logica per questi attacchi: telepatia, spara laser e lancia palla
            case "telepathic":
                agent.animator.SetTrigger(CurrentAttack);
                // ApplyDamage(); // Telecinesi non applica danni al player, lo avvicina solo all'AI
                break;

            case "throwBall":
                agent.ironBall.damage = CurrentDamage;
                agent.animator.SetTrigger(CurrentAttack);
                // ApplyDamage(); // Invocato già dallo script di ProjectileCollision presente nella palla di ferro
                break;

            case "shootLaser":
                // ResetAttackState();
                agent.animator.SetTrigger(CurrentAttack);
                // ApplyDamage(); // Invocato già dall'animazione
                break;

            case "spasm":
                hasSpasm = true;
                agent.animator.SetTrigger(CurrentAttack);
                agent.player.UpdateStatusPlayer(0, -5);
                if (rageThresholds.Count > 0)
                {
                    PerformSpasm(rageThresholds.Count == 2 ? 2 : 5); // Aumenta la potenza di attacco
                    rageThresholds.RemoveAt(0); // Rimuove il valore più alto
                }
                break;

            default:
                Debug.LogWarning($"Attack range {CurrentAttack} not exists!");
                break;
        }
    }

    void PerformSpasm(int damage)
    {
        damagePunch += damage;
        damageJumpBall += damage;
        damagePunchBall += damage;
        damageThrowBall += damage;
        damageShootLaser += damage;

        SetDamageAttacks();
    }

    public void PerformShootLaser() // Invocata dall'animazione di sparo del laser
    {
        // Calcola la direzione verso il player
        Vector3 directionToPlayer = (Agent.player.transform.position - Agent.laserSpawnPoint.position).normalized;

        // Lancia un raycast nella direzione del player
        Ray ray = new(Agent.laserSpawnPoint.position, directionToPlayer);
        if (Physics.Raycast(ray, out RaycastHit hit, Agent.maxDistanceRange, Agent.layerMask))
        {
            // Istanzia la VFX nel punto in cui il raycast ha colpito
            GameObject vfxInstance = Instantiate(Agent.vfxLaser, hit.point, Quaternion.identity);
            if (hit.collider.GetComponentInParent<Player>().CompareTag("Player"))
            {
                Agent.player.UpdateStatusPlayer(-CurrentDamage, 0); // Riduce gli HP del player
            }

            // Distruggi la VFX dopo un certo tempo
            Destroy(vfxInstance, 2f);
        }
    }

    public void PerformTelepathic() // Invocata dall'animazione di Telecinesi
    {
        // Esegue il Raycast per verificare se il boss "prende" il player
        Vector3 directionToPlayer = (Agent.player.transform.position - Agent.transform.position).normalized;
        if (Physics.Raycast(Agent.transform.position, directionToPlayer, out RaycastHit hit, Agent.maxDistanceRange, Agent.layerMask))
        {
            var player = hit.collider.gameObject.GetComponentInParent<Player>();
            if (player == null) return; // Se non è il player, esce dalla funzione

            Agent.player.animator.enabled = false;
            Agent.player.isBlocked = true;
            Vector3 targetPosition = Agent.transform.position + (directionToPlayer * 1.5f);
            StartCoroutine(MovePlayerToTarget(player, targetPosition));
        }
    }

    private IEnumerator MovePlayerToTarget(Player player, Vector3 targetPosition)
    {
        while (Vector3.Distance(player.transform.position, targetPosition) > 0.75f) // Continua finché non raggiunge la posizione target
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, 5f * Time.deltaTime);
            yield return null; // Attende il prossimo frame
        }
        yield break;
    }

    public void PerformThrowBall() => Agent.ironBall.isActive = true; // Invocato dall'animazione di lancio

    public void ResetThrowBall() => Agent.ironBall.isActive = false; // Invocato dall'animazione di lancio

    public void HitPlayer() => AttackState.ApplyDamage(Agent); // Invocato dall'animazione di attacco, quando colpisce il player

    public void ResetAttackState() => AttackState.ResetAttackState(Agent);
}
