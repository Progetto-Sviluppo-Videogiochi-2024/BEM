using System.Collections.Generic;
using UnityEngine;

public class GhoulantAttack : MonoBehaviour, IAttackAI
{
    public AIAgent Agent { get; set; }
    public Dictionary<string, (float probability, int damage)> Attacks { get; } = new()
    {
        { "swiping", (30f, 15) },
        { "bite", (25f, 20) },
        { "punching", (20f, 10) },
        { "jump", (15f, 25) },
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

        float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.player.transform.position);
        if (selectedAttack == "jump" && distanceToPlayer <= 2.5f && distanceToPlayer > 1.3f)
        {
            agent.animator.SetTrigger("jump");
            // PerformJump(Agent); // Invocato già dall'animazione
            // AttackState.ApplyDamage(agent, CurrentDamage); // Invocato già dall'animazione
            return;
        }

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

            case "jump":
                Debug.Log("Attacco: Jump (distanza non valida, disponibile solo tra 1.3 e 2.5 metri dal giocatore)");
                break;

            default:
                Debug.LogWarning($"Attack {selectedAttack} not exists!");
                break;
        }
    }

    public void PerformJump() // Invocata dall'animazione del salto di Ghoulant, non appena l'animazione inizia
    {
        if (Agent.rb == null) return;
        Agent.rb.isKinematic = false;

        // Calcolare la direzione del salto (orizzontale)
        Vector3 jumpDirection = Agent.transform.forward.normalized; // Movimento in avanti normalizzato
        float jumpDistance = 5f; // Distanza orizzontale da percorrere

        // Resetta la velocità verticale per evitare che il Rigidbody influenzi il salto in modo indesiderato
        Agent.rb.velocity = new(Agent.rb.velocity.x, 0f, Agent.rb.velocity.z);

        // Calcolo della forza orizzontale basata sulla distanza
        float horizontalForce = jumpDistance * 2f; // Valore moltiplicatore per regolare la forza
        Agent.rb.AddForce(jumpDirection * horizontalForce, ForceMode.Impulse);

        // Aggiungere componente verticale per il salto
        float verticalForce = Mathf.Sqrt(2 * 9.81f * jumpDistance); // Forza per raggiungere l'altezza in base alla distanza
        Agent.rb.AddForce(Vector3.up * verticalForce, ForceMode.Impulse);

        Agent.rb.isKinematic = true;
    }

    public void HitPlayer() => AttackState.ApplyDamage(Agent, CurrentDamage); // Invocata dall'animazione dell'attacco di Ghoulant, non appena avviene la collisione

    public void ResetAttackState() => AttackState.ResetAttackState(Agent); // Invocata dall'animazione dell'attacco di Ghoulant, verso la fine
}
