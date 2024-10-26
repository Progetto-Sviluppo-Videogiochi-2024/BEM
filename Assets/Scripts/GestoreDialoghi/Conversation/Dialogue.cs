using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("Dialogue Properties")]
    public TextMeshProUGUI textDisplay; // Variabile per il testo
    public string[] sentences; // Array di frasi
    public float typingSpeed = -4f; // Velocità di scrittura
    private int index; // Indice per le frasi

    void Start()
    {
        // Inizializza il testo e avvia la conversazione
        textDisplay.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Se 'Spacebar', passa alla frase successiva
        {
            if (textDisplay.text == sentences[index]) // Se il testo è uguale alla frase corrente
            {
                NextSentence();
            }
            else // Se il testo non è uguale alla frase corrente
            {
                StopAllCoroutines();
                textDisplay.text = sentences[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
            textDisplay.text = string.Empty;
        }
    }
}
