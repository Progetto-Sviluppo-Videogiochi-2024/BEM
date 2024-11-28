using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    #region Recoil Settings
    [HideInInspector] public Transform recoilFollowPosition;
    public float kickBackAmount = -1;
    public float kickBackSpeed = 10; 
    public float returnSpeed = 20;
    float currentRecoilPosition;
    float finalRecoilPosition;
    #endregion

    void Update()
    {
        currentRecoilPosition = Mathf.Lerp(currentRecoilPosition, 0, returnSpeed * Time.deltaTime);
        finalRecoilPosition = Mathf.Lerp(finalRecoilPosition, currentRecoilPosition, kickBackSpeed * Time.deltaTime);
        recoilFollowPosition.localPosition = new(0, 0, finalRecoilPosition);
    }

    public void TriggerRecoil() => currentRecoilPosition += kickBackAmount;
}
