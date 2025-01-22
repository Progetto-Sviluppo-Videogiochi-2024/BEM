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
    public int damageThrowBall = 20;
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
        { "spasm", (0f, 0) } // Solo quando arriva a 30% di vita e si triggera per aumentare la potenza di attacco
    };

    Dictionary<string, (float probability, int damage)> RangeAttacks { get; } = new()
    {
        { "telepathic", (25f, 0) }, // Temporanei
        { "throwBall", (40f, 0) },
        { "shootLaser", (35f, 0) },
        { "spasm", (0f, 0) } // Solo quando arriva a 30% di vita e si triggera per aumentare la potenza di attacco
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

    void SetCurrentAttackDamage(AIBossAgent agent)
    {
        isAttacking = true; // L'AI è ora in stato di attacco
        agent.navMeshAgent.isStopped = true; // Blocca il movimento durante l'attacco

        // Calcola l'attacco basato sulle probabilità
        string selectedAttack = GetAttackByProbability(MeleeAttacks);
        CurrentAttack = selectedAttack;
        CurrentDamage = MeleeAttacks[selectedAttack].damage;

        if (!hasSpasm && agent.status.Health <= rageThresholds[0]) CurrentAttack = "spasm"; // Se arriva a 20% di vita, si triggera per aumentare la potenza di attacco
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
        SetCurrentAttackDamage(agent);
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
                Debug.LogWarning($"Attack {CurrentAttack} not exists!");
                break;
        }
    }

    public void PerformRangeAttack(AIBossAgent agent)
    {
        SetCurrentAttackDamage(agent);
        print($"Range: {CurrentAttack}");

        switch (CurrentAttack)
        {
            // TODO: implementare la SFX VFX e logica per questi attacchi: telepatia, spara laser e lancia palla
            case "telepathic":
                ResetAttackState();
                // agent.animator.SetTrigger(CurrentAttack);
                // ApplyDamage(); // Invocato già dall'animazione
                break;

            case "throwBall":
                ResetAttackState();
                // agent.animator.SetTrigger(CurrentAttack);
                // ApplyDamage(); // Invocato già dall'animazione
                break;

            case "shootLaser":
                ResetAttackState();
                // agent.animator.SetTrigger(CurrentAttack);
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
                Debug.LogWarning($"Attack {CurrentAttack} not exists!");
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

    public void HitPlayer() => AttackState.ApplyDamage(Agent);

    public void ResetAttackState() => AttackState.ResetAttackState(Agent);
}
