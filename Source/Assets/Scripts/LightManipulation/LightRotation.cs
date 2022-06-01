using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;

public class LightRotation : MonoBehaviour
{
    static SerialPort serialPort = new SerialPort("COM3", 9600);
    float yRotation;
    GameObject sunlight;

    void Awake()
    {
        serialPort.Open();
        //serialPort.ReadTimeout = 75;
        sunlight = FindObjectOfType<Light>().gameObject;
        yRotation = sunlight.transform.eulerAngles.y;
        StartCoroutine(ReadFromSerialPort());
        serialPort.Write("c");
    }

    IEnumerator ReadFromSerialPort()
    {
        while (true)
        {
            if (float.TryParse(serialPort.ReadLine(), out yRotation))
                yRotation = float.Parse(serialPort.ReadLine());

            if (serialPort.ReadLine() == "d")
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            sunlight.transform.rotation = Quaternion.Euler(sunlight.transform.eulerAngles.x, yRotation, sunlight.transform.eulerAngles.z);

            yield return new WaitForSecondsRealtime(0.01f); //should be same as Arduino loop delay
        }
    }

    public static void HitByLight()
    {
        print("Sending a");
        serialPort.Write("a");
    }
}
