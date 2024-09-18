/*using UnityEngine;
using NAudio.CoreAudioApi;
using System.Collections;
using UnityEngine.UI;

public class VolumeSpriteManager : MonoBehaviour
{ // TODO: dovrei scaricare NuGet ma dava diversi errori: quindi non so se farlo
    [Header("Volume Sprites")]
    public Sprite sprite0; // Sprite per volume mutato
    public Sprite sprite1; // Sprite per volume 0
    public Sprite sprite2; // Sprite per volume 1-35
    public Sprite sprite3; // Sprite per volume 35-70
    public Sprite sprite4; // Sprite per volume 70-100

    [Header("Manual Sprite Display")]
    public float displayTime = 3f; // Tempo in secondi per cui il sprite sarà visibile
    private bool isShowing = false; // Flag per controllare se il sprite è attualmente visibile

    [Header("NAudio Settings")]
    private MMDeviceEnumerator deviceEnumerator; // Enumerator per ottenere i dispositivi audio
    private MMDevice audioDevice; // Dispositivo audio predefinito per ottenere il volume

    [Header("UI Elements")]
    public Image iconImage; // Riferimento all'oggetto Image per visualizzare l'icona

    [Header("Settings")]
    public KeyCode muteKey = KeyCode.F9; // Tasto configurabile per mutare/smutare l'audio

    private bool isMuted = false; // Flag per controllare se l'audio è muto

    private void Start()
    {
        // Inizializza NAudio per ottenere il dispositivo audio
        deviceEnumerator = new MMDeviceEnumerator();
        audioDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        // Assicurati che l'icona sia inizialmente nascosta
        iconImage.gameObject.SetActive(false);

        // Posiziona l'icona al centro dello schermo
        RectTransform rt = iconImage.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        // Controlla se "M" è premuto per mostrare il sprite manualmente
        ShowSpriteManually();
    }

    private void ShowSpriteManually()
    {
        // Controlla se il tasto è premuto e il sprite manuale non è già visibile
        if (Input.GetKeyDown(KeyCode.M) && !isShowing)
        {
            // Mostra lo sprite appropriato in base allo stato di mute
            if (Input.GetKeyDown(muteKey))
            {
                ToggleMute();
                StartCoroutine(ShowManualSprite(sprite0));
            }
            else StartCoroutine(ShowManualSprite(UpdateVolumeSprite()));
        }
    }

    private void ToggleMute()
    {
        // Cambia lo stato di mute
        isMuted = !isMuted;

        // Imposta il volume a 0 se mutato, altrimenti ripristina il volume corrente
        audioDevice.AudioEndpointVolume.Mute = isMuted;
    }

    private IEnumerator ShowManualSprite(Sprite spriteToShow)
    {
        // Imposta il flag su true per evitare chiamate multiple
        isShowing = true;

        // Aggiorna quale icona mostrare
        iconImage.sprite = spriteToShow;

        // Rende visibile l'icona
        iconImage.gameObject.SetActive(true);

        // Attendi per il tempo specificato
        yield return new WaitForSeconds(displayTime);

        // Nascondi l'icona e resetta il flag
        iconImage.gameObject.SetActive(false);
        isShowing = false;
    }

    private Sprite UpdateVolumeSprite()
    {
        // Ottieni il livello di volume corrente
        float volume = audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100; // Converti in percentuale

        // Controlla il livello del volume e restituisce lo sprite corrispondente
        if (volume == 0) return sprite1;
        else if (volume > 0 && volume < 35) return sprite2;
        else if (volume >= 35 && volume < 70) return sprite3;
        else if (volume >= 70 && volume <= 100) return sprite4;
        return null; // Caso di default, nessuno sprite da mostrare; In teoria, non dovrebbe mai accadere
    }

    private void OnDestroy()
    {
        // Libera le risorse NAudio quando l'oggetto viene distrutto o disabilitato per evitare memory leak o errori di runtime
        deviceEnumerator.Dispose();
    }
}*/
