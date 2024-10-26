// using UnityEngine;
// using DialogueEditor;
// using System.Collections;
// using UnityEngine.SceneManagement;
// using System.Linq;

// public class ConversationCharacters : MonoBehaviour
// {
//     [Header("NPC Conversation")]
//     [Tooltip("Nome della scena attuale, non è obbligatorio")]
//     public string nameScene; // Nome della scena attuale
//     [SerializeField] private NPCConversation dialogue; // Conversazione con il giocatore
//     private bool isInRange; // Giocatore nell'area di trigger

//     void Start()
//     {
//         StartDialogue();
//     }

//     // void Update()
//     // {
//     //     // Se il giocatore è nell'area di trigger e preme il tasto F, inizia la conversazione
//     //     if (isInRange && Input.GetKeyDown(KeyCode.F))
//     //     {
            
//     //     }

//     //     if (nameScene != null) // Se il nome della scena è assegnato
//     //     {
//     //         if (nameScene == SceneManager.GetActiveScene().name) // Se quella attuale
//     //         {
//     //             StartDialogue();
//     //             this.enabled = false; // Disabilitiamo l'update per evitare di ripetere la conversazione
//     //         }
//     //     }
//     // }

//     // // Inizia la conversazione con il giocatore se è nell'area di trigger 
//     // private void OnTriggerEnter(Collider other)
//     // {
//     //     if (other.CompareTag("Player"))
//     //     {
//     //         isInRange = true;
//     //     }
//     // }

//     // // Termina la conversazione con il giocatore se esce dall'area di trigger
//     // private void OnTriggerExit(Collider other)
//     // {
//     //     if (other.CompareTag("Player"))
//     //     {
//     //         isInRange = false;
//     //         StopConversation();
//     //     }
//     // }

//     // // Continua la conversazione con il giocatore se è nell'area di trigger
//     // private void OnTriggerStay(Collider other)
//     // {
//     //     if (other.CompareTag("Player"))
//     //     {
//     //         isInRange = true;
//     //     }
//     // }

//     // Avvia la conversazione con il giocatore
//     private void StartDialogue()
//     {
//         ConversationManager.Instance.StartConversation(dialogue);
//     }

//     // // Termina la conversazione con il giocatore
//     // public void StopConversation()
//     // {
//     //     ConversationManager.Instance.EndConversation();

//     //     /*if (nameScene != null && nameScene == SceneManager.GetActiveScene().name) // Se quella attuale
//     //     {
//     //         StartCoroutine(WaitAndChangeScene());
//     //     }*/
//     // }

//     // // Coroutine per aspettare e cambiare scena
//     // /*private IEnumerator WaitAndChangeScene()
//     // {
//     //     yield return new WaitForSeconds(4f); // Aspetta il tempo stabilito
//     //     SceneManager.LoadScene(GetNextScene()); // Cambia scena con la successiva
//     // }*/

//     // // Ottieni il nome della scena successiva (supponendo che le scene siano numerate con un numero alla fine e sequenziali cioè ordinate crescentemente)
//     public string GetNextScene()
//     {
//         if (char.IsDigit(nameScene.Last()))
//         {
//             // Ottieni l'ultimo carattere, converti in numero, incrementa e sostituisci
//             int lastDigit = int.Parse(nameScene.Last().ToString());
//             return nameScene[..^1] + (lastDigit + 1);
//         }
//         else // Se l'ultimo carattere non è un numero
//         {
//             return ""; // Nel nostro caso non succederà mai, essendo numerate, nel caso TODO: gestire questa situazione
//         }
//     }
// }
