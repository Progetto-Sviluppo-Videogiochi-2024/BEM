using System.Collections;
using UnityEngine;

public class TriggerFrana : MonoBehaviour
{
    public Transform boss; // Riferimento al boss
    Transform frana; // Riferimento al GO della frana
    AudioSource audioSource; // Riferimento all'audio source
    public GameObject vfxExplosion; // Riferimento al VFX dell'esplosione della dinamite
    public AudioClip explosionSound; // Riferimento all'audio dell'esplosione della dinamite
    public AudioClip bossSound; // Riferimento all'audio del boss quando viene bloccato

    bool playerPassed = false; // Flag per controllare se il player ha superato il boss
    bool mutantBlocked = false; // Flag per controllare se il boss è bloccato dalla frana caduta su di lui

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        frana = transform.GetChild(0);
        frana.gameObject.SetActive(false);
    }

    IEnumerator WaitPlayerPassed(Collider other)
    {
        while (playerPassed) yield return null;
        yield return new WaitForSeconds(3.5f);

        if (Vector3.Distance(boss.position, transform.position) > 14f || other.GetComponentInParent<AIBossAgent>())
        {
            print($"Mutant passed: {other.name}");
            boss.GetComponent<AudioSource>().clip = bossSound;
            boss.GetComponent<AudioSource>().Play();

            GameObject vfx = Instantiate(vfxExplosion, boss.position, Quaternion.identity);
            Destroy(vfx, 2f);
            audioSource.clip = explosionSound;
            audioSource.Play();

            BooleanAccessor.istance.SetBoolOnDialogueE("LandslideCollapsed");
            frana.gameObject.SetActive(true);
            boss.gameObject.SetActive(false);
            mutantBlocked = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!playerPassed && other.CompareTag("Player")) // Se passa il player
        {
            print($"Player passed: {other.name}");
            playerPassed = true;
        }

        if (playerPassed && !mutantBlocked) // Se passa il boss ma dopo che sia passato il player
        {
            StartCoroutine(WaitPlayerPassed(other));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Se esce il player
        {
            print($"Player exited: {other.name}");
            playerPassed = false;
        }
    }
}
