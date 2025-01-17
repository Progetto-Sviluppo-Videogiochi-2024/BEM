using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScarnixAttack : MonoBehaviour, IAttackAI
{
    public AIAgent Agent { get; set; }
    public AIAttackState AttackState { get; set; }
    public string CurrentAttack { get; set; }
    public int CurrentDamage { get; set; }
    public bool hasPerformedFlyKick { get; set; } = false; // Indica se il flyKick è stato eseguito (solo per Scarnix)
    public Dictionary<string, (float probability, int damage)> Attacks { get; } = new()
    {
        { "flyKick", (0f, 15) }, // Sempre il primo
        { "meleeKick", (35f, 10) },
        { "punch", (34f, 10) },
        { "meleePunch", (31f, 10) }
    };

    public void PerformRandomAttack(AIAgent agent)
    {
        AttackState.isAttacking = true; // L'AI è ora in stato di attacco
        agent.navMeshAgent.isStopped = true; // Blocca il movimento durante l'attacco

        if (!hasPerformedFlyKick)
        {
            // Esegui il flyKick per primo
            CurrentAttack = Attacks.Keys.First();
            CurrentDamage = Attacks[CurrentAttack].damage;
            hasPerformedFlyKick = true;
            agent.animator.SetTrigger(CurrentAttack);
        }
        else
        {
            // Calcola l'attacco basato sulle probabilità
            string selectedAttack = AttackState.GetAttackByProbability(Attacks);
            CurrentAttack = selectedAttack;
            CurrentDamage = Attacks[selectedAttack].damage;
            agent.animator.SetTrigger(CurrentAttack);
        }
    }

    public void HitPlayer() => AttackState.ApplyDamage(Agent, CurrentDamage); // Invocata dall'animazione dell'attacco di Scarnix, non appena avviene la collisione

    public void ResetAttackState() => AttackState.ResetAttackState(Agent); // Invocata dall'animazione dell'attacco di Scarnix, verso la fine
}
