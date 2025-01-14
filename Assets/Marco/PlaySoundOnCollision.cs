using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collisionSound;
    private bool oneTime = false;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (audioSource != null && collisionSound != null && !oneTime)
        {
            audioSource.PlayOneShot(collisionSound);
            oneTime = true;
        }
    }
}
