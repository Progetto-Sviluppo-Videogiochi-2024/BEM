using UnityEngine;

public class AIAttackState : AIState
{
    private float attackCooldown = 4f; // Tempo di cooldown tra un attacco e l'altro
    private float nextAttackTime = 0f; // Tempo in cui sarà possibile effettuare il prossimo attacco
    private bool isAttacking = false; // Indica se l'AI sta attaccando

    public void Enter(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = true; // Blocca il movimento durante l'attacco
        isAttacking = false; // Resetta lo stato d'attacco
    }

    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = false; // Riattiva il movimento
        isAttacking = false; // Resetta lo stato d'attacco
    }

    public AIStateId GetId() => AIStateId.Attack;

    public void Update(AIAgent agent)
    {
        float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.player.transform.position);

        if (agent.player.isDead)
        {
            agent.stateMachine.ChangeState(AIStateId.Patrol);
            return;
        }

        // Controllo distanza per tornare allo stato di inseguimento
        if (!isAttacking && distanceToPlayer > 2.5f/*agent.attackRange*/)
        {
            agent.stateMachine.ChangeState(AIStateId.ChasePlayer);
            return;
        }

        // Controlla se l'AI è attualmente in attacco
        if (isAttacking)
        {
            // L'AI rimane ferma durante l'attacco
            agent.navMeshAgent.isStopped = true;

            if (Time.time >= nextAttackTime)
            {
                isAttacking = false; // Termina l'attacco dopo il cooldown
                agent.navMeshAgent.isStopped = false; // Sblocca il movimento
            }
            return;
        }

        // Controlla se è possibile iniziare un nuovo attacco
        if (Time.time >= nextAttackTime)
        {
            PerformRandomAttack(agent); // Seleziona un attacco casuale in base alle probabilità
            nextAttackTime = Time.time + attackCooldown; // Aggiorna il tempo per il prossimo attacco
        }
    }

    private void PerformRandomAttack(AIAgent agent)
    {
        isAttacking = true; // L'AI è ora in stato di attacco
        agent.navMeshAgent.isStopped = true; // Blocca il movimento durante l'attacco

        // Probabilità degli attacchi (percentuale)
        float[] attackProbabilities = new float[] { 30f, 25f, 20f, 15f, 10f }; // Swiping, Bite, Punching, Jump, Scream
        string[] attackNames = new string[] { "swiping", "bite", "punching", "jump", "scream" };

        // Calcola l'attacco basato sulle probabilità
        string selectedAttack = GetAttackByProbability(attackProbabilities, attackNames);

        float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.player.transform.position);
        Debug.Log("Distanza al giocatore: " + distanceToPlayer);
        if (selectedAttack == "jump" && distanceToPlayer <= 2.5f && distanceToPlayer > 1.30f)
        {
            agent.animator.SetTrigger("jump");
            Debug.Log("Attacco: Jump");
            PerformJump(agent);
            ApplyDamage(agent, 25); // Infligge 25 danni
            return;
        }

        // Attiva l'animazione corrispondente e applica l'effetto
        switch (selectedAttack)
        {
            case "swiping":
                agent.animator.SetTrigger("swiping");
                Debug.Log("Attacco: Swiping");
                ApplyDamage(agent, 15); // Infligge 15 danni
                break;

            case "bite":
                agent.animator.SetTrigger("bite");
                Debug.Log("Attacco: Bite");
                ApplyDamage(agent, 20); // Infligge 20 danni
                break;

            case "punching":
                agent.animator.SetTrigger("punching");
                Debug.Log("Attacco: Punching");
                ApplyDamage(agent, 10); // Infligge 10 danni
                break;

            case "scream":
                agent.animator.SetTrigger("scream");
                Debug.Log("Attacco: Scream");
                // Non infligge danno diretto, ma può stordire il giocatore o avere altri effetti
                break;

            case "jump":
                Debug.Log("Attacco: Jump (distanza non valida, disponibile solo tra 1.3 e 2.5 metri dal giocatore)");
                break;

            default:
                Debug.LogWarning("Attack not exists: " + selectedAttack);
                break;
        }
    }

    private string GetAttackByProbability(float[] probabilities, string[] attackNames)
    {
        float totalProbability = 0f;
        foreach (float probability in probabilities) totalProbability += probability;

        float randomValue = Random.Range(0f, totalProbability);
        float cumulative = 0f;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulative += probabilities[i];
            if (randomValue <= cumulative) return attackNames[i];
        }

        return attackNames[0]; // Fallback nel caso qualcosa vada storto
    }

    private void ApplyDamage(AIAgent agent, int damage)
    {
        if (Vector3.Distance(agent.transform.position, agent.player.transform.position) <= 2f/*agent.attackRange*/)
        {
            agent.player.GetComponent<Player>().UpdateStatusPlayer(damage, 10);
        }
    }

    private void PerformJump(AIAgent agent)
    {
        if (agent.rb == null) return;

        // Calcolare la direzione del salto (orizzontale)
        Vector3 jumpDirection = agent.transform.forward; // Movimento in avanti
        float jumpDistance = 5f; // Distanza orizzontale da percorrere
        Vector3 targetPosition = agent.transform.position + agent.transform.forward * jumpDistance;

        // Resetta la velocità verticale per evitare che il Rigidbody influenzi il salto in modo indesiderato
        agent.rb.velocity = new Vector3(agent.rb.velocity.x, 0f, agent.rb.velocity.z);

        // Aggiungere forza orizzontale per il salto
        agent.rb.AddForce(jumpDirection * 10f, ForceMode.Impulse); // Forza orizzontale

        // Aggiungere componente verticale per il salto
        agent.rb.AddForce(Vector3.up * 250f, ForceMode.Impulse); // Forza verticale per saltare più in alto (aggiusta il valore per maggiore altezza)

        // Muovere il Transform in avanti per il salto (modifica la posizione dell'AI)
        agent.transform.position = Vector3.Lerp(agent.transform.position, targetPosition, Time.deltaTime * 5f); // Muove l'AI orizzontalmente

        // Muovi anche l'AI in verticale
        Vector3 newPosition = agent.transform.position + Vector3.up * 3f; // Distanza di salto verticale
        agent.transform.position = newPosition;
    }
}
