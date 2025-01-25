using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CombinazioneManager : MonoBehaviour
{

    [Header("References")]
    #region References
    public GameObject CombinazioneCanvas; // Il canvas che contiene la UI della Combinazione
    private Button CloseCombinazioneUI; // Il pulsante per chiudere la UI della Combinazione
    List<TMP_Dropdown> dropdowns = new(); // Lista di dropdown per la combinazione
    public Player player; // Riferimento al giocatore
    [SerializeField] private AudioClip combinazione; // Audio che viene riprodotto quando la combinazione è corretta
    public GameObject doorToOpen; // Porta da aprire quando la combinazione è corretta
    public GameObject doorToClose; // Porta da chiudere quando la combinazione è corretta 
    public ManagerScena3 managerScena3; // Riferimento al ManagerScena3
    public Transform charactersZonaBoss; // Riferimento ai personaggi della zona boss
    #endregion

    bool isInRange = false; // Indica se il giocatore è vicino alla Combinazione
    bool isCombinazioneOpen = false; // Indica se il canvas della Combinazione è aperto
    public bool sbloccato = false; // Stato di sblocco (combinazione corretta o no)

    void Start()
    {
        if (BooleanAccessor.istance.GetBoolFromThis("doorUnlocked")) { ToggleDoor(true); enabled = false; return; }
        else ToggleDoor(false);
        CloseCombinazioneUI = CombinazioneCanvas.transform.GetChild(1).GetComponent<Button>(); // Il pulsante per chiudere la UI della Combinazione
        CloseCombinazioneUI.onClick.AddListener(() => { ToggleCombinazione(false); RemoveButtonFocus(); });

        // Imposta il listener per i cambiamenti nei dropdown
        var panel = CombinazioneCanvas.transform.GetChild(0); // Il pannello che contiene i dropdown
        foreach (Transform child in panel)
        {
            dropdowns.Add(child.GetComponent<TMP_Dropdown>());
            dropdowns[^1].onValueChanged.AddListener(delegate { VerificaCombinazione(); }); // Aggiunge all'ultimo corrente il listener per la verifica
        }
    }

    void Update()
    {
        if (!sbloccato && !player.hasEnemyDetectedPlayer && isInRange && Input.GetKeyDown(KeyCode.Space)) // Se il giocatore è vicino alla combinazione
        {
            ToggleCombinazione(!isCombinazioneOpen);
        }

        if (isCombinazioneOpen && Input.GetMouseButtonDown(0) && !IsPointerOverUI()) // Se il canvas è aperto e non stai cliccando su un elemento UI
        {
            ToggleCombinazione(false);
        }
    }

    void VerificaCombinazione()
    {
        if (dropdowns[0].value == 7 && dropdowns[1].value == 0 && dropdowns[2].value == 7 && dropdowns[3].value == 0) // 7 0 7 0
        {
            sbloccato = true;
            BooleanAccessor.istance.SetBoolOnDialogueE("doorUnlocked"); // Per il LG
            managerScena3.PlayThisAudioInCoroutine(combinazione);
            SaveLoadSystem.Instance.SaveCheckpoint();
            ToggleDoor(true);
            ToggleCombinazione(false);

            charactersZonaBoss.GetChild(0).gameObject.SetActive(true); // Angelica
            charactersZonaBoss.GetChild(1).gameObject.SetActive(true); // Jacob
            // Stygian no perché quello avviene durante il dialogo
        }
    }

    public void ToggleDoor(bool isOpen)
    {
        doorToOpen.SetActive(isOpen);
        doorToClose.SetActive(!isOpen);
    }

    private void ToggleCombinazione(bool isOpen)
    {
        // Se il canvas è aperto chiudilo, else viceversa
        isCombinazioneOpen = isOpen;
        CombinazioneCanvas.SetActive(isCombinazioneOpen);
        GestoreScena.ChangeCursorActiveStatus(isCombinazioneOpen, "Combinazione");
        player.GetComponent<AimStateManager>().enabled = !isCombinazioneOpen; // Per la visuale

        Time.timeScale = isCombinazioneOpen ? 0 : 1; // 0 = pausa, 1 = gioco normale
    }

    private void RemoveButtonFocus() => EventSystem.current.SetSelectedGameObject(null);

    private bool IsPointerOverUI()
    {
        // Verifica se il puntatore è su un elemento UI
        PointerEventData pointerData = new(EventSystem.current) { position = Input.mousePosition };
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        // Controlla se uno degli elementi colpiti appartiene al Canvas della radio
        foreach (var result in raycastResults)
        {
            if (result.gameObject.transform.IsChildOf(CombinazioneCanvas.transform)) return true;
        }

        return false; // Non stai cliccando su un elemento del Canvas
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
