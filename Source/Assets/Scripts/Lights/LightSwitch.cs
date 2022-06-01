using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    Material green;
    Material blue;

    void Start()
    {
        green = GameObject.Find("Green").GetComponent<Renderer>().material;
        blue = GameObject.Find("Blue").GetComponent<Renderer>().material;
    }

    public void ChangeMaterial(string msg)
    {
        switch (msg)
        {
            case "Blue Off":
                blue.SetColor("_EmissionColor", Color.black);
                break;
            case "Blue On":
                blue.SetColor("_EmissionColor", Color.blue);
                break;
            case "Green Off":
                green.SetColor("_EmissionColor", Color.black);
                break;
            case "Green On":
                green.SetColor("_EmissionColor", Color.green);
                break;
        }
    }
}
