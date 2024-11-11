using UnityEngine;

public class Inactivity : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

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

        // Se l'animazione di inattività è finita
        CycleToNextInactivity(animator);

        // Controllo per capire se l'inattività deve essere interrotta a causa di un movimento del pg
        if (IsMoving(animator)) ResetInactivity(animator);
    }

    // private float GetLengthAnimationInactivity(Animator animator) => animator.GetCurrentAnimatorStateInfo(0).length;

    private bool CanInactivity(Animator animator) =>
        !(animator.GetBool("aiming") || animator.GetBool("reloading") // Se il giocatore sta mirando o ricaricando
            || animator.GetBool("pickingUp") // Se il giocatore sta raccogliendo un oggetto
            || IsMoving(animator) // Se il giocatore si sta muovendo
            || animator.GetBool("hasCutWeapon") || animator.GetBool("hasFireWeapon")); // Se il giocatore ha un'arma bianca o da fuoco equipaggiata in mano

    private void ResetInactivity(Animator animator)
    {
        animator.SetBool("inactive", false);
        animator.SetInteger("nInactive", 0);
    }

    private void CycleToNextInactivity(Animator animator)
    {
        if (animator.GetBool("sit")) animator.SetInteger("nInactive", (animator.GetInteger("nInactive") % 3) + 1); // Cicla 1, 2, 3
        else // Se il giocatore non è seduto
        {
            int currentInactive = animator.GetInteger("nInactive");

            if (currentInactive == 1) animator.SetInteger("nInactive", 3);
            else if (currentInactive == 3) animator.SetInteger("nInactive", 1);
        }
    }

    private bool IsMoving(Animator animator) => animator.GetFloat("hInput") != 0 || animator.GetFloat("vInput") != 0;

    private bool IsInactivityFinished(AnimatorStateInfo stateInfo) => stateInfo.normalizedTime >= 1.0f;
}
