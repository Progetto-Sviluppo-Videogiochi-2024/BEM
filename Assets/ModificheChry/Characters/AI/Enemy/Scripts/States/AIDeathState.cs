using System.Collections;
using UnityEngine;

public class AIDeathState : AIState<AIAgent>
{
    public void Enter(AIAgent agent)
    {
        // Status dell'agente
        if (agent.status.IsDead) return; // Se l'agente è già morto, non fare nulla
        agent.status.IsDead = true;
        agent.id = GestoreScena.GenerateId(agent.gameObject, agent.transform);
        agent.player.hasEnemyDetectedPlayer = false;
        agent.detection.enemyInDetectionRange = false;
        agent.ragdollManager.TriggerRagdoll();
        agent.StartCoroutine(PlayDeathAudio(agent));

        if (agent.gameObject.name.Contains("Ghoulant") && agent.nameBoolBA != null && !BooleanAccessor.istance.GetBoolFromThis(agent.nameBoolBA))
        {
            SaveLoadSystem.Instance.SaveCheckpoint();
            agent.StartConversation();
        }

        // Disattiva i componenti dell'agente
        agent.enabled = false;
        agent.navMeshAgent.enabled = false;
        agent.detection.enabled = false;
        agent.status.enabled = false;
        agent.locomotion.enabled = false;
        agent.animator.enabled = false;
        agent.stateMachine = null;
    }

    public void Exit(AIAgent agent) { }

    public AIStateId GetId() => AIStateId.Death;

    public void Update(AIAgent agent) { }

    private IEnumerator PlayDeathAudio(AIAgent agent)
    {
        agent.audioSource.volume += 0.25f;
        agent.PlayAudio(3, false);
        yield return new WaitForSeconds(agent.audioSource.clip.length);
        agent.audioSource.clip = null;
        yield break;
    }
}
