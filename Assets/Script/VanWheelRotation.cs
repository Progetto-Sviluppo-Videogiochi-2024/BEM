using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanWheelRotation : MonoBehaviour
{

    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    public float rotationSpeed = 360f; // degrees per second

    void Update()
    {
        RotateWheels();
    }

    void RotateWheels()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        frontLeftWheel.Rotate(Vector3.right, rotationAmount);
        frontRightWheel.Rotate(Vector3.right, rotationAmount);
        rearLeftWheel.Rotate(Vector3.right, rotationAmount);
        rearRightWheel.Rotate(Vector3.right, rotationAmount);
    }

}
