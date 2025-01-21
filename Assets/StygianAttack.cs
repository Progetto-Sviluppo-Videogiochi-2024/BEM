using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StygianAttack : MonoBehaviour
{
    [HideInInspector] public bool isAttacking = false; // Indica se l'AI sta attaccando
    [HideInInspector] public bool spasm = false; // Indica se il mutante si è triggerato
    [HideInInspector] public float currentHealthSpasm = 0.2f; // Salva la vita attuale per lo spasm
    public float timeSinceLastAttack = 0f; // Tempo trascorso dall'ultimo attacco

    public int damagePunch = 20;
    public int damageJumpBall = 30;
    public int damagePunchBall = 25;
    public List<float> rageThresholds = new() { 0.2f, 0.1f }; // Percentuali (dal valore più alto al più basso)

    public AIBossAttackState AttackState { get; set; }
    public AIBossAgent Agent { get; set; }
    public string CurrentAttack { get; set; }
    public int CurrentDamage { get; set; }
    public Dictionary<string, (float probability, int damage)> MeleeAttacks { get; } = new()
    {
        { "punch", (40f, 0) }, // Temporanei
        { "jumpBall", (30f, 0) },
        { "punchBall", (20f, 0) },
        { "spasm", (0f, 0) } // Solo quando arriva a 30% di vita e si triggera per aumentare la potenza di attacco
    };

    void Start()
    {
        // Per gli attacchi ravvicinati
        MeleeAttacks["punch"] = (MeleeAttacks["punch"].probability, damagePunch);
        MeleeAttacks["jumpBall"] = (MeleeAttacks["jumpBall"].probability, damageJumpBall);
        MeleeAttacks["punchBall"] = (MeleeAttacks["punchBall"].probability, damagePunchBall);
        // spasm rimane invariato perché si ha quando il mutante si triggera e aumenta la sua potenza di attacco
    }

    bool ShouldChase() =>
        Random.Range(0f, 1f) > 0.5f; // 50% di probabilità di inseguire il giocatore

    public void PerformMeleeAttack(AIBossAgent agent)
    {
        isAttacking = true; // L'AI è ora in stato di attacco
        agent.navMeshAgent.isStopped = true; // Blocca il movimento durante l'attacco

        // Calcola l'attacco basato sulle probabilità
        string selectedAttack = GetMeleeByProbability();
        if (spasm && agent.status.Health <= currentHealthSpasm) selectedAttack = "spasm"; // Se arriva a 20% di vita, si triggera per aumentare la potenza di attacco
        CurrentAttack = selectedAttack;
        CurrentDamage = MeleeAttacks[selectedAttack].damage;

        // Attiva l'animazione corrispondente e applica l'effetto
        switch (selectedAttack)
        {
            case "punch":
            case "jumpBall":
            case "punchBall":
                agent.animator.SetTrigger(selectedAttack);
                // ApplyDamage(); // Invocato già dall'animazione
                break;

            case "spasm":
                spasm = true;
                currentHealthSpasm -= 0.1f;
                agent.animator.SetTrigger("spasm");
                agent.player.UpdateStatusPlayer(0, -5);
                if (rageThresholds.Count > 0)
                {
                    PerformSpasm(rageThresholds.Count == 2 ? 2 : 5); // Aumenta la potenza di attacco
                    rageThresholds.RemoveAt(0); // Rimuove il valore più alto
                }
                break;

            default:
                Debug.LogWarning($"Attack {selectedAttack} not exists!");
                break;
        }
    }

    public void PerformRangeAttack(AIBossAgent agent)
    {
        Debug.Log("Attacco a distanza non implementato");
    }

    void PerformSpasm(int damage)
    {
        damagePunch += damage;
        damageJumpBall += damage;
        damagePunchBall += damage;

        MeleeAttacks["punch"] = (MeleeAttacks["punch"].probability, damagePunch);
        MeleeAttacks["jumpBall"] = (MeleeAttacks["jumpBall"].probability, damageJumpBall);
        MeleeAttacks["punchBall"] = (MeleeAttacks["punchBall"].probability, damagePunchBall);
    }

    string GetMeleeByProbability()
    {
        float totalProbability = MeleeAttacks.Values.Sum(x => x.probability);
        float randomValue = Random.Range(0f, totalProbability);
        float cumulative = 0f;

        foreach (var attack in MeleeAttacks)
        {
            cumulative += attack.Value.probability;
            if (randomValue <= cumulative)
            {
                return attack.Key;
            }
        }
        return MeleeAttacks.Keys.First(); // Fall-back di sicurezza (non dovrebbe mai accadere)
    }

    public void HitPlayer() => AttackState.ApplyDamage(Agent);

    public void ResetAttackState() => AttackState.ResetAttackState(Agent);
}
