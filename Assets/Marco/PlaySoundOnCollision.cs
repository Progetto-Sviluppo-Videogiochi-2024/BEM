using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip collisionSound;
    private bool oneTime = false;
    public Player player;

    private void Start()
    {
        audioSource ??= GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider _)
    {
        if (!oneTime && !player.hasEnemyDetectedPlayer && !BooleanAccessor.istance.GetBoolFromThis("stealth"))
        {
            audioSource.PlayOneShot(collisionSound);
            oneTime = true;
        }
    }
}
