using System.Collections;
using UnityEngine;

public class AIBossDeathState : AIState<AIBossAgent>
{
    public void Enter(AIBossAgent agent)
    {
        // Status dell'agente
        if (agent.status.IsDead) return; // Se l'agente è già morto, non fare nulla
        agent.status.IsDead = true;
        agent.id = GestoreScena.GenerateId(agent.gameObject, agent.transform);
        agent.StartCoroutine(PlayDeathAudio(agent));
        SaveLoadSystem.Instance.SaveCheckpoint();

        // Disattiva i componenti dell'agente
        agent.enabled = false;
        agent.navMeshAgent.enabled = false;
        agent.status.enabled = false;
        agent.animator.enabled = false;
        agent.stateMachine = null;
    }

    public void Exit(AIBossAgent agent) { }

    public AIStateId GetId() => AIStateId.Death;

    public void Update(AIBossAgent agent) { }

    private IEnumerator PlayDeathAudio(AIBossAgent agent)
    {
        agent.audioSource.volume += 0.25f;
        // agent.PlayAudio(3, false);
        yield return new WaitForSeconds(agent.audioSource.clip.length);
        agent.audioSource.clip = null;
        yield break;
    }
}
