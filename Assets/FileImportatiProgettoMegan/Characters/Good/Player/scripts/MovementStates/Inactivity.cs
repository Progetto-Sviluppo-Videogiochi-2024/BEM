using UnityEngine;

public class Inactivity : StateMachineBehaviour
{
    private float animationInterval; // Intervallo di tempo in secondi tra ogni animazione di inattività
    public float inactivityTimer = 0.0f; // Timer per tenere traccia del tempo trascorso dall'ultima animazione di inattività

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animationInterval = GetLengthAnimationInactivity(animator);
        inactivityTimer = 0.0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (StopInactivity(animator))
        {
            ResetInactivity(animator);
            return;
        }

        if (!IsInactivityFinished(stateInfo)) return;

        inactivityTimer += Time.deltaTime;
        if (inactivityTimer >= animationInterval) // Se il timer raggiunge l'intervallo (animationInterval)
        {
            RestartAnimation(animator, stateInfo, layerIndex);
        }

        // Controllo per interrompere l'inattività e iniziare il movimento
        if (IsMoving(animator)) ResetInactivity(animator);
    }

    private float GetLengthAnimationInactivity(Animator animator)
    {
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }

    private bool StopInactivity(Animator animator)
    {
        return animator.GetBool("aiming") || animator.GetBool("reloading") // Se il giocatore sta mirando o ricaricando
            || IsMoving(animator) // Se il giocatore si sta muovendo
            || animator.GetBool("hasCutWeapon") || animator.GetBool("hasFireWeapon"); // Se il giocatore ha un'arma bianca o da fuoco equipaggiata in mano
    }

    private void ResetInactivity(Animator animator)
    {
        inactivityTimer = 0.0f;
        animator.SetBool("inactive", false);
    }

    private void RestartAnimation(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.Play(stateInfo.fullPathHash, layerIndex, 0.0f);
        inactivityTimer = 0.0f;
    }

    private bool IsMoving(Animator animator)
    {
        return animator.GetFloat("hInput") != 0 || animator.GetFloat("vInput") != 0;
    }

    private bool IsInactivityFinished(AnimatorStateInfo stateInfo)
    {
        return stateInfo.normalizedTime >= 1.0f;
    }
}
