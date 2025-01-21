using UnityEngine;

public class AIBossAttackState : AIState<AIBossAgent>
{
    public void Enter(AIBossAgent agent)
    {
        Debug.Log("Attack");
        agent.mutantAttack.Agent ??= agent;
        agent.mutantAttack.AttackState ??= this;
        agent.mutantAttack.timeSinceLastAttack = agent.attackCooldown;
        agent.mutantAttack.isAttacking = false; // Resetta lo stato d'attacco
        agent.navMeshAgent.isStopped = true; // Ferma il movimento durante l'attacco
    }

    public void Exit(AIBossAgent agent)
    {
        agent.mutantAttack.isAttacking = false; // Resetta lo stato d'attacco
        agent.navMeshAgent.isStopped = false; // Riattiva il movimento
    }

    public AIStateId GetId() => AIStateId.Attack;

    public void Update(AIBossAgent agent)
    {
        if (!agent.status.IsEnemyAlive()) return;

        float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.player.transform.position);

        // Se il giocatore è fuori portata, torna a inseguire
        if (!agent.mutantAttack.isAttacking && distanceToPlayer > agent.maxDistanceRange)
        {
            // if (!ShouldChase()) PerformRangeAttack(agent);
            // else 
            agent.stateMachine.ChangeState(AIStateId.ChasePlayer);
            return;
        }

        // Decidi il tipo di attacco
        agent.mutantAttack.timeSinceLastAttack += Time.deltaTime;
        if (agent.mutantAttack.isAttacking || agent.mutantAttack.timeSinceLastAttack <= agent.attackCooldown) return;
        if (!agent.mutantAttack.isAttacking && distanceToPlayer <= agent.distanceMelee) // Attacco ravvicinato (melee)
        {
            agent.mutantAttack.PerformMeleeAttack(agent);
        }
        // else if (!isAttacking && distanceToPlayer > agent.minDistanceRange && distanceToPlayer <= agent.maxDistanceRange) // Attacco a distanza (range)
        // {
        //     if (ShouldChase())
        //     {
        //         agent.stateMachine.ChangeState(AIStateId.ChasePlayer);
        //         return;
        //     }
        //     PerformRangeAttack(agent);
        // }
    }

    public void ApplyDamage(AIBossAgent agent) // Invocato dall'animazione di attacco, quando colpisce il player
    {
        if (!agent.mutantAttack.isAttacking) return; // Se non sta attaccando, non fare nulla

        Vector3 rayDirection = (agent.player.transform.position - agent.transform.position).normalized;
        if (Physics.Raycast(agent.transform.position, rayDirection, out RaycastHit _, 2f, agent.layerMask)) // Se colpisce il player
        {
            agent.player.UpdateStatusPlayer(-agent.mutantAttack.CurrentDamage, -5);
            // Debug.Log($"{agent.name}.'{agent.mutantAttack.CurrentAttack}': (HP, SM) = (-{damage}, -10) -> ({agent.player.health}, {agent.player.sanitaMentale})");

            if (agent.player.health > 0) return; // Se è ancora vivo, non fare nulla
            agent.player.GetComponentInChildren<Rigidbody>().AddForce(rayDirection * 5f, ForceMode.Impulse);
        }
        // ResetAttackState(agent);
    }

    public void ResetAttackState(AIBossAgent agent) // Invocato dall'animazione di attacco, quando termina l'attacco
    {
        agent.mutantAttack.isAttacking = false;
        agent.navMeshAgent.isStopped = false;
    }
}
