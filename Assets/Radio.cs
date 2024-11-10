using DialogueEditor;
using UnityEngine;

public class Radio : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    public bool isOn = true; // Indica se la radio è accesa
    public bool canInteract = false; // Indica se il player può interagire con la radio
    #endregion

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.Space))
        {
            isOn = false;
            var boolAccessor = BooleanAccessor.istance;
            boolAccessor.SetBoolOnDialogueE("radio");
            ConversationManager.Instance.SetBool("radio", boolAccessor.GetBoolFromThis("radio"));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !BooleanAccessor.istance.GetBoolFromThis("radio")) canInteract = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) canInteract = false;
    }
}
