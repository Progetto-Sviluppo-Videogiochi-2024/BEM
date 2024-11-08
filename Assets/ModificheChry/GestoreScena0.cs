using UnityEngine;

public class GestoreScena0 : MonoBehaviour
{
    [Header("GameObjects")]
    #region GameObjects
    public GameObject backPackPlayer;
    #endregion

    void Start()
    {
        PlayerPrefs.SetInt("hasBackpack", 0);
        PlayerPrefs.SetInt("hasTorch", 0);
        backPackPlayer.SetActive(false);
    }

    void Update()
    {
        HideBackPack();
    }

    private void HideBackPack() => backPackPlayer.SetActive(PlayerPrefs.GetInt("hasBackpack") == 1);
}