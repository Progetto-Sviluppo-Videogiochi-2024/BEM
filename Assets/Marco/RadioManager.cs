using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadioManager : MonoBehaviour
{
    [Header("Structure Data")]
    #region Structure Data
    public List<AudioClip> songs; // Lista delle canzoni della radio
    #endregion

    [Header("UI Elements")]
    #region UI Elements
    public GameObject radioCanvas; // Il canvas che contiene la UI della radio
    public Button SalvaButton; // Il pulsante per salvare la radio
    public Button OnOffButton; // Il pulsante per accendere/spegnere la radio
    public Button CloseRadioUI; // Il pulsante per chiudere la UI della radio
    #endregion

    [Header("Settings")]
    #region Settings
    private bool isInRange = false; // Indica se il giocatore è vicino alla radio
    private bool isRadioOpen = false; // Indica se il canvas della radio è aperto
    private bool isOn; // Indica se la radio è accesa
    private int currentSongIndex = 0; // Indice della canzone attuale
    private Color onColor = Color.green;
    private Color offColor = Color.red;
    #endregion

    [Header("References")]
    #region References
    private AudioSource audioSource; // L'audio source della radio
    #endregion

    void Start()
    {
        radioCanvas.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        PlayAudio();

        // Configura i listener per i pulsanti
        SalvaButton.onClick.AddListener(Save);
        OnOffButton.onClick.AddListener(OnOff);
        CloseRadioUI.onClick.AddListener(CloseRadio);
    }

    void Update()
    {
        if (!audioSource.isPlaying && audioSource.clip != null) // Se la canzone è finita e c'è una canzone attuale
        {
            NextSong();
        }

        if (isInRange && Input.GetKeyDown(KeyCode.Space)) // Se il giocatore è vicino alla radio e 'Space'
        {
            ToggleRadio();
        }

        if (isRadioOpen && Input.GetMouseButtonDown(0) && !IsPointerOverUI()) // Se il canvas della radio è aperto e il click non è su un UI
        {
            ToggleRadio();
        }
    }

    private void ToggleRadio()
    {
        isRadioOpen = !isRadioOpen;
        radioCanvas.SetActive(isRadioOpen);

        Time.timeScale = isRadioOpen ? 0 : 1; // 0 = pausa, 1 = gioco normale
    }

    private bool IsPointerOverUI()
    {
        // Verifica se il puntatore è su un elemento UI
        PointerEventData pointerData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        // Controlla se uno degli elementi colpiti appartiene al Canvas della radio
        foreach (var result in raycastResults)
        {
            if (result.gameObject.transform.IsChildOf(radioCanvas.transform)) return true;
        }

        return false; // Non stai cliccando su un elemento del Canvas
    }

    private void PlayAudio()
    {
        if (songs.Count <= 0) return;

        currentSongIndex = Random.Range(0, songs.Count);
        audioSource.clip = songs[currentSongIndex];
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.Play();
        OnOffButton.GetComponent<Image>().color = onColor;
    }

    private void NextSong()
    {
        currentSongIndex = (currentSongIndex + 1) % songs.Count; // Incrementa l'indice e lo riporta a 0 se è maggiore della lunghezza della lista
        audioSource.clip = songs[currentSongIndex];
        audioSource.Play();
    }

    private void CloseRadio()
    {
        ToggleRadio();
    }

    private void Save()
    {
        // Fare una classe per salvare e caricare i dati e invocare qui quella funzione
        print("Salva");
        Time.timeScale = 0;
    }

    private void OnOff()
    {
        if (isRadioOpen && isOn) // Se accesa, spegni
        {
            audioSource.clip = null;
            audioSource.Stop();
            OnOffButton.GetComponent<Image>().color = offColor;
        }
        else if (isRadioOpen && !isOn) // Se spenta, accendi
        {
            PlayAudio();
        }
        isOn = !isOn;

        Time.timeScale = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isInRange = false;
    }
}
