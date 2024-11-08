using UnityEngine;

public class GestoreScena0 : MonoBehaviour
{
    [Header("GameObjects")]
    #region GameObjects
    public GameObject backPackScena0;
    public GameObject backPackPlayer;
    #endregion

    void Start()
    {
        backPackPlayer.SetActive(false);
    }

    void Update()
    {
        HideBackPack();
    }

    private void HideBackPack()
    {
        if (PlayerPrefs.GetInt("hasBackPack") == 1)
        {
            backPackScena0.SetActive(false);
            backPackPlayer.SetActive(true);
        }
    }
}