using UnityEngine;

public class ManagerScena3 : MonoBehaviour
{
    [HideInInspector] public BooleanAccessor booleanAccessor; // Riferimento al BooleanAccessor

    public void SetDEBool(string nomeBool) // Da invocare nel DialogueEditor per settare i valori booleani del BooleanAccessor
    {
        booleanAccessor.SetBoolOnDialogueE(nomeBool);
    }
}
