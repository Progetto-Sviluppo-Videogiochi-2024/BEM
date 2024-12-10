using UnityEngine;

public class GestoreTransizione : MonoBehaviour
{
    public ChapterTransition chapterTransition; // Riferimento al capitolo della scena di transizione

    private void Start()
    {
        // Imposta il testo del capitolo all'avvio del gioco o scena
        chapterTransition?.SetChapterText("Capitolo x");
    }
}
