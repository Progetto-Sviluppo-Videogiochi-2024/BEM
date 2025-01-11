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
    public Button CloseCombinazioneUI; // Il pulsante per chiudere la UI della Combinazione
    private Transform player; // Il giocatore
    [SerializeField] private AudioClip combinazione; // Audio che viene riprodotto quando la combinazione è corretta
    public GameObject oggettoDaDisattivare; // Oggetto da disattivare quando la combinazione è corretta
    public ManagerScena3 managerScena3; // Riferimento al ManagerScena3
    #endregion

    private bool isInRange = false; // Indica se il giocatore è vicino alla Combinazione
    private bool isCombinazioneOpen = false; // Indica se il canvas della radio è aperto
    public bool sbloccato = false; // Stato di sblocco (combinazione corretta o no)

    public TMP_Dropdown dropdown0; // Riferimento al primo dropdown (7)
    public TMP_Dropdown dropdown1; // Riferimento al secondo dropdown (0)
    public TMP_Dropdown dropdown2; // Riferimento al terzo dropdown (7)
    public TMP_Dropdown dropdown3; // Riferimento al quarto dropdown (0)

    void Start()
    {
        player = FindAnyObjectByType<Player>().transform;
        CombinazioneCanvas.SetActive(false);
        CloseCombinazioneUI.onClick.AddListener(() => { ToggleCombinazione(false); RemoveButtonFocus(); });

        // Imposta il listener per i cambiamenti nei dropdown
        dropdown0.onValueChanged.AddListener(delegate { VerificaCombinazione(); });
        dropdown1.onValueChanged.AddListener(delegate { VerificaCombinazione(); });
        dropdown2.onValueChanged.AddListener(delegate { VerificaCombinazione(); });
        dropdown3.onValueChanged.AddListener(delegate { VerificaCombinazione(); });
    }

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.Space)) // Se il giocatore è vicino alla combinazione
        {
            ToggleCombinazione(!isCombinazioneOpen);
        }

        if (isCombinazioneOpen && Input.GetMouseButtonDown(0) && !IsPointerOverUI()) // Se il canvas della radio è aperto e il click non è sulla UI della radio
        {
            ToggleCombinazione(false);
        }
    }

    void VerificaCombinazione()
    {
        if (dropdown0.value == 7 && dropdown1.value == 0 && dropdown2.value == 7 && dropdown3.value == 0)
        {
            sbloccato = true;
            StartCoroutine(managerScena3.PlayAudioAndWait(2f, combinazione));
            oggettoDaDisattivare?.SetActive(false);
            ToggleCombinazione(false);
        }
        else sbloccato = false;
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
