using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoulantAttack : MonoBehaviour, IAttackAI
{
    public int damageSwiping = 20;
    public int damageBite = 25;
    public int damagePunching = 20;
    public AIAgent Agent { get; set; }
    public AIAttackState AttackState { get; set; }
    public string CurrentAttack { get; set; }
    public int CurrentDamage { get; set; }
    public bool hasPerformedFlyKick { get; set; } = false; // Indica se il flyKick è stato eseguito (solo per Scarnix)
    public Dictionary<string, (float probability, int damage)> Attacks { get; } = new()
    {
        { "swiping", (40f, 0) }, // Temporanei
        { "bite", (30f, 0) },
        { "punching", (20f, 0) },
        { "scream", (10f, 0) }
    };

    void Start()
    {
        // Assegna i valori di danno definitivi al dizionario
        Attacks["swiping"] = (Attacks["swiping"].probability, damageSwiping);
        Attacks["bite"] = (Attacks["bite"].probability, damageBite);
        Attacks["punching"] = (Attacks["punching"].probability, damagePunching);
        // scream rimane invariato perché ha danno 0
    }

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
                agent.player.UpdateStatusPlayer(0, -5);
                damageSwiping += 2;
                damageBite += 2;
                damagePunching += 2;
                break;

            default:
                Debug.LogWarning($"Attack {selectedAttack} not exists!");
                break;
        }
    }

    public void PerformBite() // Invocata dall'animazione del morso di Ghoulant
    {
        // if (Vector3.Distance(Agent.transform.position, Agent.player.transform.position) < 1.5f)
        // {
        //     var player = Agent.player;
        //     player.GetComponent<MovementStateManager>().enabled = false;
        //     player.GetComponent<Animator>().enabled = false;
        //     StartCoroutine(MovePlayerToBiteTarget(player.transform));
        // }
    }

    private IEnumerator MovePlayerToBiteTarget(Transform player)
    {
        while (true)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            if (Quaternion.Angle(transform.rotation, lookRotation) < 5f) yield break;
            yield return null;
        }
    }

    public void ResetBite()
    {
        // var player = Agent.player;
        // if (player.IsDead()) return;
        // player.GetComponent<MovementStateManager>().enabled = true;
        // player.GetComponent<Animator>().enabled = true;
    }

    public void HitPlayer() => AttackState.ApplyDamage(Agent, CurrentDamage); // Invocata dall'animazione dell'attacco di Ghoulant, non appena avviene la collisione

    public void ResetAttackState() => AttackState.ResetAttackState(Agent); // Invocata dall'animazione dell'attacco di Ghoulant, verso la fine
}
