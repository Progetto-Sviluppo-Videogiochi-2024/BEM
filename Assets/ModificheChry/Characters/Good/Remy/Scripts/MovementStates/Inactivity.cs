using UnityEngine;

public class Inactivity : StateMachineBehaviour
{
    private float animationInterval; // Intervallo di tempo in secondi tra ogni animazione di inattività
    public float inactivityTimer = 0.0f; // Timer per tenere traccia del tempo trascorso dall'ultima animazione di inattività

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animationInterval = GetLengthAnimationInactivity(animator, layerIndex);
        inactivityTimer = 0.0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Controllo per capire se l'inattività deve essere interrotta a causa di un'azione del giocatore
        if (!CanInactivity(animator))
        {
            ResetInactivity(animator);
            return;
        }

        // Se l'animazione di inattività non è finita
        if (!IsInactivityFinished(stateInfo)) return;

        inactivityTimer += Time.deltaTime;
        if (inactivityTimer >= animationInterval) CycleToNextInactivity(animator); // Se il timer raggiunge l'intervallo (animationInterval)

        // Controllo per capire se l'inattività deve essere interrotta a causa di un movimento del pg
        if (IsMoving(animator)) ResetInactivity(animator);
    }

    private float GetLengthAnimationInactivity(Animator animator, int layerIndex) => animator.GetCurrentAnimatorStateInfo(layerIndex).length;

    private bool CanInactivity(Animator animator) =>
        !(animator.GetBool("hasCutWeapon") || animator.GetBool("hasFireWeapon")) // Se il giocatore ha un'arma bianca o da fuoco equipaggiata in mano
            || animator.GetBool("aiming") || animator.GetBool("reloading") // Se il giocatore sta mirando o ricaricando
            || animator.GetBool("pickingUp") // Se il giocatore sta raccogliendo un oggetto
            || IsMoving(animator); // Se il giocatore si sta muovendo

    private void ResetInactivity(Animator animator)
    {
        inactivityTimer = 0.0f;
        animator.SetBool("inactive", false);
        animator.SetInteger("nInactive", 1);
    }

    private void CycleToNextInactivity(Animator animator)
    {
        inactivityTimer = 0.0f;
        if (animator.GetBool("sit")) animator.SetInteger("nInactive", (animator.GetInteger("nInactive") % 2) + 1); // Cicla 1, 2 se è seduto
        else animator.SetInteger("nInactive", animator.GetInteger("nInactive") == 1 ? 3 : 1); // Cicla 1, 3 se non è seduto
    }

    private bool IsMoving(Animator animator) => animator.GetFloat("hInput") != 0 || animator.GetFloat("vInput") != 0;

    private bool IsInactivityFinished(AnimatorStateInfo stateInfo) => stateInfo.normalizedTime >= 1.0f;
}
