using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RadioManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject RadioCanvas;
    public Button SalvaButton;
    public Button OnOffButton;
    public Button CloseRadioUI;

    [Header("Settings")]
    public bool isInRange = false; // Indica se il player può interagire con la radio
    private bool isRadioOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        // Assicura che la UI della Radio sia inizialmente nascosta
        RadioCanvas.SetActive(false);

        // Configura i listener per i pulsanti
        SalvaButton.onClick.AddListener(Salva);
        OnOffButton.onClick.AddListener(OnOff);
        CloseRadioUI.onClick.AddListener(CloseRadio);

    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.Space))
        {
            ToggleRadio();
        }


    }

    private void ToggleRadio()
    {
        isRadioOpen = !isRadioOpen;
        RadioCanvas.SetActive(isRadioOpen);

        // Mette in pausa il gioco quando la radio è attiva, lo riprende quando chiusa
        Time.timeScale = isRadioOpen ? 0 : 1;
    }

    // Metodo per ripristinare Time.timeScale e chiudere il menu
    private void ResumeGame()
    {
        isRadioOpen = false;
        RadioCanvas.SetActive(false);
        Time.timeScale = 1; // Riprendi il tempo di gioco
    }

    private void CloseRadio()
    {
        ResumeGame();
    }

    private void Salva()
    {
        print("Salva");
        ResumeGame();
    }

    private void OnOff()
    {
        print("OnOff");
        ResumeGame();
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
