using System.Collections;
using UnityEngine;

public class ManagerScena3 : MonoBehaviour
{
    [HideInInspector] public BooleanAccessor booleanAccessor; // Riferimento al BooleanAccessor
    [HideInInspector] private AudioSource audioSource; // Riferimento all'AudioSource
    [SerializeField] private AudioClip forestSound; // Suono della foresta

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        booleanAccessor = BooleanAccessor.istance;

        StopPlayAudio(forestSound, true);
    }

    public void PlayThisAudioInCoroutine(AudioClip audioClip) => StartCoroutine(PlayAudioAndWait(audioClip));

    private IEnumerator PlayAudioAndWait(AudioClip audioClip)
    {
        StopPlayAudio(audioClip, false);
        yield return new WaitForSeconds(audioClip.length);
        StopPlayAudio(forestSound, true);
    }

    private void StopPlayAudio(AudioClip audioClip, bool loop)
    {
        // Ferma l'audio corrente e riproduce il nuovo clip
        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void SetDEBool(string nomeBool) // Da invocare nel DE per settare i valori booleani del BooleanAccessor
    {
        booleanAccessor.SetBoolOnDialogueE(nomeBool);
    }
}
