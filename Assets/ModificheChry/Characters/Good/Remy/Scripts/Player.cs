using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Player Status")]
    #region Player Status
    public int health = 100;
    public int stamina = 100;
    public int sanita_mentale = 100; // TODO: da cambiare
    [HideInInspector] public bool isDead = false;
    #endregion

    [Header("Player UI")]
    #region Player UI
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI staminaText;
    #endregion

    private void Update()
    {
        if (health <= sanita_mentale) // TODO: da implementare
        { }
    }

    public void UpdateHealth(int amount)
    {
        if (IsDead()) return; // Se è morto, non fare nulla
        health += amount;
        // healthText.text = $"HP: {health}";
    }

    public void UpdateStamina(int amount)
    {
        stamina += amount;
        // staminaText.text = $"ST: {stamina}";
    }

    private bool IsDead()
    {
        if (isDead) return true; // Se era già morto, non fare nulla

        if (health <= 0) // Se è appena morto
        {
            health = 0;
            isDead = true;
            // healthText.text = $"HP: {health}";
            // Settare l'animazione/ragdoll della morte del player
            return true;
        }
        return false;
    }
}
