using UnityEngine;

public class GothicJumpscare : MonoBehaviour
{
    [Header("References")]
    #region References
    public Transform player; // Riferimento al player
    private Transform gothicSitting; // Riferimento alla gotica seduta
    private Transform gothicDeath; // Riferimento alla gotica morta
    public Transform blood; // Riferimento al sangue
    public Transform mutante; // Riferimento al mutante
    #endregion

    [Header("Settings")]
    #region Settings
    [HideInInspector] public bool hasEnd = false; // Se uno scenario è finito cioè se è tornato a Stefano
    private int nScene = 0; // Numero scenario del jumpscare della gotica (0 = seduta, 1 = fissata dal mutante, 2 = morta con sangue senza il mutante, 3 = allucinazione di Stefano => stanza vuota)
    #endregion

    void Start()
    {
        // Prendo i riferimenti delle due animazioni della gotica
        gothicSitting = transform.GetChild(0);
        gothicDeath = transform.GetChild(1);

        // Disattivo sangue, mutante e la gotica morta
        gothicDeath.gameObject.SetActive(false);
        mutante.gameObject.SetActive(false);
        blood.gameObject.SetActive(false);
    }

    private void Scene1()
    {
        nScene += 1;
    }
}
