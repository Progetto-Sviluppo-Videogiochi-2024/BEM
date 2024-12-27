using UnityEngine;

[ExecuteInEditMode]
public class ResetAnimatorPose : MonoBehaviour
{
    private void OnValidate() // Richiamato ogni volta che modifichi qualcosa in Edit Mode
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Rebind(); // Resetta lo stato del rig
            // animator.Update(0); // Aggiorna l'animatore alla sua posa iniziale // Farlo solo alla prima
        }
    }
}
