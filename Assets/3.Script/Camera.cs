using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Camera : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 5f;
    public float distance = 5f;
    public float minVerticalAngle = -20f;
    public float maxVerticalAngle = 60f;
    public float verticalSpeed = 2f;

    private float currentRotation = 0f;
    private float currentVerticalAngle = 0f;



    private void LateUpdate()
    {

        // 마우스 움직임 입력
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 수평 및 수직 회전 업데이트
        currentRotation += mouseX * rotationSpeed;
        currentVerticalAngle -= mouseY * verticalSpeed;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);

        // 카메라 위치 업데이트
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentRotation, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
        transform.position = target.position + offset;

        // 카메라 회전 업데이트
        transform.rotation = Quaternion.Euler(currentVerticalAngle, currentRotation, 0f);
    }
}
