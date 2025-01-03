using DialogueEditor;
using UnityEngine;

public class AngelicaHitBall : NPCAIBase
{
    [Header("Settings")]
    #region Settings
    [HideInInspector] public bool isArrived = false; // Flag per controllare se l'agente è arrivato al target
    [HideInInspector] public bool hitBall = false; // Flag per controllare se la palla è stata colpita
    #endregion

    [Header("References")]
    #region References
    public Transform stopTarget; // Riferimento al target di sosta
    #endregion

    protected override void Start()
    {
        base.Start();
        stopDistance = 2f;
    }

    protected override void MoveTowardsTarget(Vector3 destination)
    {
        if (!BooleanAccessor.istance.GetBoolFromThis("preHitBallJA") || !ConversationManager.Instance.hasClickedEnd) return;

        float distanceToTarget = Vector3.Distance(transform.position, destination);
        if (!hitBall && distanceToTarget > stopDistance) MoveAgent(destination, 2f); // Se sei lontano dal target, inseguilo
        else if (hitBall) StopAgent(); // Se hai colpito la palla, fermati
    }

    public void MoveTowardsStopTarget(Transform destination)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, destination.position);

        if (distanceToPlayer > stopDistance) // Se il target è abbastanza lontano, l'agente si muove verso di lui
        {
            MoveAgent(destination.position, Mathf.Lerp(agent.speed, 2f, Time.deltaTime * 5f)); // Interpolazione graduale
        }
        else StopAgent(); // Se è abbastanza vicino, l'agente si ferma
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Target")) { isArrived = true; Destroy(other.gameObject); }
    }
}
