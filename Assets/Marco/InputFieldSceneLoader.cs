using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InputFieldSceneLoader : MonoBehaviour
{
    private TMP_InputField inputField;

    void Start()
    {
        // Trova il componente TMP_InputField
        inputField = GetComponent<TMP_InputField>();
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(HandleInput);
        }
        else
        {
            Debug.LogError("TMP_InputField non trovato! Assicurati di aver aggiunto il componente.");
        }
    }

    void HandleInput(string inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText)) return;

        // Gestisce il caricamento della scena
        if (inputText.StartsWith("Scena") && int.TryParse(inputText.Substring(5), out int sceneNumber))
        {
            if (sceneNumber >= 0 && sceneNumber <= 3)
            {
                SceneManager.LoadScene(inputText);
            }
            else
            {
                Debug.LogWarning("Scena non valida!");
            }
        }
        else
        {
            Debug.LogWarning("Comando non riconosciuto!");
        }

        // Pulisce il campo di input
        inputField.text = string.Empty;
    }
}
