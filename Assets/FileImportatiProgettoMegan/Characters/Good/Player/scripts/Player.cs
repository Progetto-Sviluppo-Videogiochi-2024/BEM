using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player instance;

    [Header("Player Components")]
    public int health = 100;
    public int stamina = 100;
    public int sanita_mentale = 100; // TODO: da cambiare

    [Header("Player Bars Components")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI staminaText;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (health <= sanita_mentale) // TODO: da implementare
        {}
    }

    public void IncreaseHealth(int amount)
    {
        health += amount;
        healthText.text = $"HP: {health}";
    }

    public void IncreaseStamina(int amount)
    {
        stamina += amount;
        staminaText.text = $"ST: {stamina}";
    }
}