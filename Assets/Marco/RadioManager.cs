using System.Collections.Generic;
using DialogueEditor;
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
    [HideInInspector] public bool isOn = false; // Indica se la radio è accesa
    private int currentSongIndex = 0; // Indice della canzone attuale
    private Color onColor = Color.green;
    private Color offColor = Color.red;
    #endregion

    [Header("References")]
    #region References
    private Transform player; // Il giocatore
    private AudioSource audioSource; // L'audio source della radio
    #endregion

    void Start()
    {
        player = FindAnyObjectByType<Player>().transform;

        radioCanvas.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        PlayAudio();

        // Configura i listener per i pulsanti
        SalvaButton.onClick.AddListener(() => { Save(); RemoveButtonFocus(); });
        OnOffButton.onClick.AddListener(() => { OnOff(); RemoveButtonFocus(); });
        CloseRadioUI.onClick.AddListener(() => { CloseRadio(); RemoveButtonFocus(); });
    }

    void Update()
    {
        if (!audioSource.isPlaying && audioSource.clip != null) // Se la canzone è finita e c'è una canzone attuale
        {
            NextSong();
        }

        if (isInRange && Input.GetKeyDown(KeyCode.Space)) // Se il giocatore è vicino alla radio e 'Space'
        {
            ToggleRadio(!isRadioOpen);
        }

        if (isRadioOpen && Input.GetMouseButtonDown(0) && !IsPointerOverUI()) // Se il canvas della radio è aperto e il click non è sulla UI della radio
        {
            ToggleRadio(false);
        }

        // TODO: manca la funzione per fargli impostare la canzone scorrendo la rotella della radio
    }

    private void RemoveButtonFocus()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void ToggleRadio(bool isOpen)
    {
        // Se il canvas è aperto chiudilo, else viceversa
        isRadioOpen = isOpen;
        radioCanvas.SetActive(isRadioOpen);
        GestoreScena.ChangeCursorActiveStatus(isRadioOpen, "Radio");
        player.GetComponent<AimStateManager>().enabled = !isRadioOpen; // Per la visuale

        Time.timeScale = isRadioOpen ? 0 : 1; // 0 = pausa, 1 = gioco normale
    }

    private bool IsPointerOverUI()
    {
        // Verifica se il puntatore è su un elemento UI
        PointerEventData pointerData = new(EventSystem.current) { position = Input.mousePosition };
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
        isOn = true;
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
        ToggleRadio(false);
    }

    private void Save()
    {
        // TODO: fare una classe per salvare e caricare i dati e invocare qui quella funzione
        print("Salva");
        Time.timeScale = 0;
    }

    private void OnOff()
    { // TODO: vedere a fine gioco se deve essere riprodotta una canzone random, la prossima oppure "mutarla" quando spammo il pulsante
        var boolAccessor = BooleanAccessor.istance;
        var convManager = ConversationManager.Instance;
        if (isRadioOpen && isOn) // Se accesa, spegni
        {
            isOn = false;
            boolAccessor.SetBoolOnDialogueE("radio");
            convManager.SetBool("radio", boolAccessor.GetBoolFromThis("radio"));
            audioSource.clip = null;
            audioSource.Stop();
            OnOffButton.GetComponent<Image>().color = offColor;
        }
        else if (isRadioOpen && !isOn) // Se spenta, accendi
        {
            boolAccessor.ResetBoolValue("radio");
            convManager.SetBool("radio", boolAccessor.GetBoolFromThis("radio"));
            PlayAudio();
        }

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
