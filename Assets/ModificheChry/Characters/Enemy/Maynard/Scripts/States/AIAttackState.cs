using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIAttackState : AIState
{
    [HideInInspector] public bool isAttacking = false; // Indica se l'AI sta attaccando
    private float timeSinceLastAttack = 0f; // Tempo trascorso dall'ultimo attacco

    public void Enter(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = true; // Blocca il movimento durante l'attacco
        isAttacking = false; // Resetta lo stato d'attacco
        agent.mutantAttack.AttackState ??= this;
        agent.mutantAttack.Agent ??= agent;
        timeSinceLastAttack = agent.attackCooldown;
    }

    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = false; // Riattiva il movimento
        isAttacking = false; // Resetta lo stato d'attacco
    }

    public AIStateId GetId() => AIStateId.Attack;

    public void Update(AIAgent agent)
    {
        if (agent.player.isDead)
        {
            agent.StopAudio();
            agent.stateMachine.ChangeState(AIStateId.Patrol);
            return;
        }

        // Controllo distanza per tornare allo stato di inseguimento
        float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.player.transform.position);
        if (!isAttacking && distanceToPlayer > agent.minDistanceAttack)
        {
            agent.stateMachine.ChangeState(AIStateId.ChasePlayer);
            return;
        }

        // Controlla se è possibile iniziare un nuovo attacco
        timeSinceLastAttack += Time.deltaTime;
        if (!isAttacking && timeSinceLastAttack >= agent.attackCooldown)
        {
            agent.mutantAttack.PerformRandomAttack(agent); // Seleziona un attacco casuale in base alle probabilità
            timeSinceLastAttack = 0f; // Reset del tempo dal ultimo attacco
        }
    }

    public string GetAttackByProbability(Dictionary<string, (float probability, int damage)> attacks)
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

    public void ApplyDamage(AIAgent agent, int damage)
    {
        if (!isAttacking) return; // Se non sta attaccando, non fare nulla

        Vector3 rayDirection = (agent.player.transform.position - agent.transform.position).normalized;
        if (Physics.Raycast(agent.transform.position, rayDirection, out RaycastHit _, 2f, agent.layerMask)) // Se colpisce il player
        {
            agent.player.UpdateStatusPlayer(-damage, -10);
            Debug.Log($"{agent.name}.'{agent.mutantAttack.CurrentAttack}': (HP, SM) = (-{damage}, -10) -> ({agent.player.health}, {agent.player.sanitaMentale})");

            if (agent.player.health > 0) return; // Se è ancora vivo, non fare nulla
            agent.player.GetComponentInChildren<Rigidbody>().AddForce(rayDirection * 5f, ForceMode.Impulse);
        }
        // ResetAttackState(agent);
    }

    public void ResetAttackState(AIAgent agent)
    {
        isAttacking = false;
        agent.navMeshAgent.isStopped = false;
    }
}
