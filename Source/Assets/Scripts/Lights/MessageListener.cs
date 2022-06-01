using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageListener : MonoBehaviour
{
    LightSwitch lightSwitch;

    void Start()
    {
        lightSwitch = FindObjectOfType<LightSwitch>().GetComponent<LightSwitch>();
    }
    void OnMessageArrived(string msg)
    {
        Debug.Log("Message arrived: " + msg);

        lightSwitch.ChangeMaterial(msg);
    }

    void OnConnectionEvent(bool success)
    {
        if (success)
            Debug.Log("Connection established");
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }
}
