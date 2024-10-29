using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCompletaMissione : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            // Controlla se l'istanza di BooleanAccessor è inizializzata
            if (BooleanAccessor.istance != null)
            {
                // Chiama il metodo sull'istanza
                BooleanAccessor.istance.SetBoolOnDialogueE("tolto");
            }
            else
            {
                Debug.LogError("BooleanAccessor.istance non è stato inizializzato.");
            }
        }   
    }
}
