using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class Movement : MonoBehaviour
{
    Rigidbody rb;
    SerialPort serialPort = new SerialPort("COM3", 9600);

    float vertical = 0;

    [Tooltip("By how much the force should be multiplied.")]
    public float force;

    void Awake()
    {
        serialPort.Open();
        serialPort.ReadTimeout = 100;

        rb = GetComponent<Rigidbody>();
        StartCoroutine(ReadFromSerialPort());
    }

    IEnumerator ReadFromSerialPort()
    {
        while (true)
        {
            print(serialPort.ReadLine());

            if (float.TryParse(serialPort.ReadLine(), out vertical))
                vertical = float.Parse(serialPort.ReadLine());

            yield return new WaitForSecondsRealtime(0.01f); //should be same as Arduino loop delay
        }
    }

    void Update()
    {
        rb.AddForce(Vector3.up * vertical * Time.deltaTime * force, ForceMode.Impulse);
        rb.AddForce(0.75f * Physics.gravity);

        float rotateZ = rb.velocity.y * 4f - 90f;
        rotateZ = Mathf.Clamp(rotateZ, -135f, 55f);
        rb.MoveRotation(Quaternion.Euler(0f, 0f, rotateZ));
    }

    public void SendSerialMessage(string msg)
    {
        serialPort.Write(msg);
    }
}
