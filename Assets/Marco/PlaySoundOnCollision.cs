using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip collisionSound;
    private bool oneTime = false;
    public Player player;
    public string boolName;

    void Start()
    {
        audioSource ??= GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!oneTime && other.CompareTag("Player") && !player.hasEnemyDetectedPlayer && !BooleanAccessor.istance.GetBoolFromThis(boolName))
        {
            BooleanAccessor.istance.SetBoolOnDialogueE(boolName);
            audioSource.PlayOneShot(collisionSound);
            oneTime = true;
        }
    }
}
