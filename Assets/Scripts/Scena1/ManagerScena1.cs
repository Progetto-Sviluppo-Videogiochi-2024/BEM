using UnityEngine;
using DialogueEditor;
using UnityEngine.SceneManagement;
using System.Linq;

public class ManagerScena1 : MonoBehaviour
{
    [Header("Audio Properties")]
    #region Audio Properties
    [SerializeField] private AudioClip audioClip; // Clip audio da riprodurre all'inizio della scena
    [SerializeField] private float audioVolume; // Volume dell'audio
    #endregion

    [Header("Settings")]
    #region Settings
    private bool isMuted = false; // Stato di mutamento dell'audio
    private string nameScene; // Nome della scena attuale
    #endregion

    [Header("References")]
    #region References
    [SerializeField] private NPCConversation dialogue; // Riferimento alla conversazione del gruppo di ragazzi
    private AudioSource audioSource; // Riferimento all'AudioSource
    public BooleanAccessor booleanAccessor; // Riferimento al BooleanAccessor
    #endregion

    void Start()
    {
        nameScene = SceneManager.GetActiveScene().name;
        StartDialogue();
        audioSource = GetComponent<AudioSource>(); // Per ottenere l'AudioSource
        PlayAudio();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Se 'Return' (tasto invio)
        {
            SceneManager.LoadScene(GetNextScene()); // Per caricare la scena successiva
        }

        if (Input.GetKeyDown(KeyCode.M)) // Se 'M'
        {
            ToggleMute();
        }
    }

    // Metodo per avviare l'audio
    private void PlayAudio()
    {
        if (audioSource == null) // Se l'AudioSource non è assegnato
        {
            Debug.LogWarning("AudioSource don't assigned to the script. Please assign it in the inspector.");
            return;
        }

        if (audioClip != null) // Se la clip audio è stata assegnata
        {
            audioSource.clip = audioClip; // Per impostare la clip audio
            audioSource.playOnAwake = false; // Per evitare la riproduzione automatica
            audioSource.loop = false; // Per evitare la ripetizione
            audioSource.volume = audioVolume; // Per impostare il volume
            audioSource.Play(); // Per avviarlo
        }
        else // Se la clip audio non è stata assegnata
        {
            Debug.LogWarning("AudioClip don't assigned to the script. Please assign it in the inspector.");
        }
    }

    // Metodo per mutare o riattivare l'audio
    private void ToggleMute()
    {
        isMuted = !isMuted;
        if (audioSource != null) // Se l'AudioSource è assegnato
        {
            audioSource.mute = isMuted;
        }
    }

    private void StartDialogue()
    {
        ConversationManager.Instance.StartConversation(dialogue);
    }

    public string GetNextScene()
    {
        if (char.IsDigit(nameScene.Last()))
        {
            // Ottieni l'ultimo carattere, converti in numero, incrementa e sostituisci
            int lastDigit = int.Parse(nameScene.Last().ToString());
            return nameScene[..^1] + (lastDigit + 1);
        }
        else // Se l'ultimo carattere non è un numero
        {
            return ""; // Nel nostro caso non succederà mai, essendo numerate, nel caso TODO: gestire questa situazione
        }
    }

    public void SetDEBool(string nomeBool) // Da invocare nel DialogueEditor per settare i valori booleani del BooleanAccessor
    {
        booleanAccessor.SetBoolOnDialogueE(nomeBool);
    }
}
