using System.Collections.Generic;
using UnityEngine;

public class GhoulantAttack : MonoBehaviour, IAttackAI
{
    public AIAgent Agent { get; set; }
    public Dictionary<string, (float probability, int damage)> Attacks { get; } = new()
    {
        { "swiping", (40f, 15) },
        { "bite", (30f, 20) },
        { "punching", (20f, 10) },
        { "scream", (10f, 0) }
    };

    public AIAttackState AttackState { get; set; }
    public string CurrentAttack { get; set; }
    public int CurrentDamage { get; set; }

    public void PerformRandomAttack(AIAgent agent)
    {
        AttackState.isAttacking = true; // L'AI è ora in stato di attacco
        agent.navMeshAgent.isStopped = true; // Blocca il movimento durante l'attacco

        // Calcola l'attacco basato sulle probabilità
        string selectedAttack = AttackState.GetAttackByProbability(Attacks);
        CurrentAttack = selectedAttack;
        CurrentDamage = Attacks[selectedAttack].damage;

        // Attiva l'animazione corrispondente e applica l'effetto
        switch (selectedAttack)
        {
            case "swiping":
            case "bite":
            case "punching":
                agent.animator.SetTrigger(selectedAttack);
                // AttackState.ApplyDamage(agent, CurrentDamage); // Invocato già dall'animazione
                break;

            case "scream":
                agent.animator.SetTrigger("scream");
                // Non infligge danno diretto
                break;

            default:
                Debug.LogWarning($"Attack {selectedAttack} not exists!");
                break;
        }
    }

    public void HitPlayer() => AttackState.ApplyDamage(Agent, CurrentDamage); // Invocata dall'animazione dell'attacco di Ghoulant, non appena avviene la collisione

    public void ResetAttackState() => AttackState.ResetAttackState(Agent); // Invocata dall'animazione dell'attacco di Ghoulant, verso la fine
}
