using UnityEngine;

public class GestoreTransizione : MonoBehaviour
{
    public ChapterTransition chapterTransition; // Assegna il riferimento dal Inspector

    private void Start()
    {
        if (chapterTransition != null)
        {
            // Imposta il testo del capitolo all'avvio del gioco o scena
            chapterTransition.SetChapterText("Capitolo x");
        }
    }
}
