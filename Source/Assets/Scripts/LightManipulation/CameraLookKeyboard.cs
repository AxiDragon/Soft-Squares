using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookKeyboard : MonoBehaviour
{
    public float sensitivity = 100;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseY = (Input.GetKey(KeyCode.E) ? 1f : 0f) * sensitivity * Time.deltaTime;
        mouseY += (Input.GetKey(KeyCode.Q) ? -1f : 0f) * sensitivity * Time.deltaTime;
        
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
