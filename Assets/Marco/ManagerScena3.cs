using System.Collections;
using UnityEngine;

public class ManagerScena3 : MonoBehaviour
{
    [HideInInspector] public BooleanAccessor booleanAccessor; // Riferimento al BooleanAccessor
    [HideInInspector] private AudioSource audioSource; // Riferimento all'AudioSource

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        booleanAccessor = BooleanAccessor.istance;
    }

    public void PlayAudio(AudioClip audioClip) => audioSource.PlayOneShot(audioClip);

    public IEnumerator PlayAudioAndWait(float seconds, AudioClip audioClip)
    {
        if (!audioSource.isPlaying) PlayAudio(audioClip);
        yield return new WaitForSeconds(seconds);
        audioSource.Stop();
    }

    public void SetDEBool(string nomeBool) // Da invocare nel DE per settare i valori booleani del BooleanAccessor
    {
        booleanAccessor.SetBoolOnDialogueE(nomeBool);
    }
}
