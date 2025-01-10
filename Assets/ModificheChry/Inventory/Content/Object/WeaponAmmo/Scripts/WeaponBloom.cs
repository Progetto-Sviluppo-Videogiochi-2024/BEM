using UnityEngine;

public class WeaponBloom : MonoBehaviour
{
    [Header("Bloom Properties")]
    #region Bloom Properties
    [SerializeField] float defaultBloomAngle = 3f;
    [SerializeField] float walkBloomMultiplier = 1.5f;
    [SerializeField] float runBloomMultiplier = 2f;
    [SerializeField] float crouchBloomMultiplier = 0.5f;
    [SerializeField] float aimBloomMultiplier = 0.5f;
    #endregion

    [Header("Bloom Settings")]
    #region Bloom Settings
    float currentBloom;
    #endregion

    [Header("References Scripts")]
    #region References
    MovementStateManager movement;
    AimStateManager aim;
    #endregion

    void Start()
    {
        var weaponClassManager = GetComponent<WeaponManager>().weaponClassManager;
        movement = weaponClassManager.GetComponent<MovementStateManager>();
        aim = weaponClassManager.GetComponent<AimStateManager>();
    }

    public Vector3 BloomAngle(Transform bulletSpawnPoint)
    {
        if (movement.currentState == movement.idleState) currentBloom = defaultBloomAngle;
        else if (movement.currentState == movement.walkingState) currentBloom = defaultBloomAngle * walkBloomMultiplier;
        else if (movement.currentState == movement.runningState) currentBloom = defaultBloomAngle * runBloomMultiplier;
        else if (movement.currentState == movement.crouchState)
        {
            if (movement.moveDirection.magnitude == 0) currentBloom = defaultBloomAngle * crouchBloomMultiplier;
            else currentBloom = defaultBloomAngle * crouchBloomMultiplier * walkBloomMultiplier;
        }

        if (aim.currentState == aim.rifleIdleAimState) currentBloom *= aimBloomMultiplier;

        float randX = Random.Range(-currentBloom, currentBloom);
        float randY = Random.Range(-currentBloom, currentBloom);
        float randZ = Random.Range(-currentBloom, currentBloom);

        Vector3 randomRotation = new(randX, randY, randZ);
        return bulletSpawnPoint.localEulerAngles + randomRotation;
    }
}
