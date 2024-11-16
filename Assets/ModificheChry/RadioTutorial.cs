using DialogueEditor;
using UnityEngine;

public class RadioTutorial : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    public bool isOn = true; // Indica se la radio è accesa
    public bool isInRange = false; // Indica se il player può interagire con la radio
    #endregion

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.Space))
        {
            isOn = false;
            var boolAccessor = BooleanAccessor.istance;
            boolAccessor.SetBoolOnDialogueE("radio");
            ConversationManager.Instance.SetBool("radio", boolAccessor.GetBoolFromThis("radio"));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !BooleanAccessor.istance.GetBoolFromThis("radio")) isInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isInRange = false;
    }
}
