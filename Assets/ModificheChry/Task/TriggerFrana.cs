using System.Collections;
using DialogueEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerFrana : MonoBehaviour
{
    public Transform boss; // Riferimento al boss
    Transform frana; // Riferimento al GO della frana
    AudioSource audioSource; // Riferimento all'audio source
    public GameObject vfxExplosion; // Riferimento al VFX dell'esplosione della dinamite
    public AudioClip explosionSound; // Riferimento all'audio dell'esplosione della dinamite
    public AudioClip bossSound; // Riferimento all'audio del boss quando viene bloccato
    public TriggerPlayer safeZonePlayer; // Riferimento alla safe zone del player dopo la zona frana
    public NPCConversation postFrana; // Riferimento alla conversazione post frana tra i personaggi
    public GestoreScena gestoreScena; // Riferimento al gestore della scena

    bool playerPassed = false; // Flag per controllare se il player ha superato il boss
    bool mutantBlocked = false; // Flag per controllare se il boss è bloccato dalla frana caduta su di lui

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        frana = transform.GetChild(0);
        frana.gameObject.SetActive(false);
    }

    IEnumerator CheckPlayerSafeZone()
    {
        while (!safeZonePlayer.isPlayerPassed) // Aspetta finché il player non supera la safe zone
        {
            yield return null;
        }

        TriggerLandslide();
    }

    void TriggerLandslide()
    {
        if (mutantBlocked) return; // Evita di attivare la frana più volte
        mutantBlocked = true;

        if (Vector3.Distance(boss.position, transform.position) <= 10f)
        {
            boss.GetComponent<AudioSource>().clip = bossSound;
            boss.GetComponent<AudioSource>().Play();
        }

        GameObject vfx = Instantiate(vfxExplosion, transform.position, Quaternion.identity);
        Destroy(vfx, 2f);
        audioSource.clip = explosionSound;
        audioSource.Play();

        BooleanAccessor.istance.SetBoolOnDialogueE("LandslideCollapsed");
        frana.gameObject.SetActive(true);
        boss.gameObject.SetActive(false);

        ConversationManager.Instance.StartConversation(postFrana);
        GestoreScena.ChangeCursorActiveStatus(true, "TriggerFrana: TriggerLandslide");
        ConversationManager.OnConversationEnded += OnConversationEnded;
    }

    void OnConversationEnded()
    {
        ConversationManager.OnConversationEnded -= OnConversationEnded;
        BooleanAccessor.istance.SetBoolOnDialogueE("postFrana");
        GestoreScena.ChangeCursorActiveStatus(false, "TriggerFrana: OnConversationEnded");
        if (BooleanAccessor.istance.GetBoolFromThis("endDemo") && BooleanAccessor.istance.GetBoolFromThis("postFrana"))
            StartCoroutine(EndDemo());
    }

    IEnumerator EndDemo()
    {
        yield return new WaitForSeconds(2f);
        gestoreScena.GoToTransitionScene();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!playerPassed && other.CompareTag("Player")) // Se passa il player
        {
            playerPassed = true;
            StartCoroutine(CheckPlayerSafeZone());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Se esce il player
        {
            playerPassed = false;
        }
    }
}
