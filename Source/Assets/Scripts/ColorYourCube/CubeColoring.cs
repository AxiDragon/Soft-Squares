using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class CubeColoring : MonoBehaviour
{
    Material material;
    Text colorScaleText;

    string[] dictStrings = new string[] { "Hue", "Saturation", "Value", "Roughness", "Metallic" };
    int currentString = 0;
    Dictionary<string, float> hueSaturationValue = new Dictionary<string, float>();

    float changeSpeed = 5f;
    float holdMomentum = 1f;
    float holdMomentumChange = 0.001f;

    bool materialUpdated = false;

    //SerialPort controller = new SerialPort("COM3", 9600);

    void Start()
    {
        //controller.Open();

        foreach (string dictString in dictStrings)
            hueSaturationValue.Add(dictString, 0f);

        hueSaturationValue["Value"] = 1f;

        material = GetComponent<MeshRenderer>().material;
        colorScaleText = GameObject.Find("ColorScaleText").GetComponent<Text>();

        colorScaleText.text = dictStrings[currentString];
        //UpdateMaterial();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            ChangeMode(true);

        if (Input.GetKeyDown(KeyCode.A))
            ChangeMode(false);

        if (Input.GetKey(KeyCode.W))
            ChangeColorScaleValue(true);

        if (Input.GetKey(KeyCode.S))
            ChangeColorScaleValue(false);

        if (materialUpdated)
            holdMomentum += holdMomentumChange;
        else
            holdMomentum = 1f;

        materialUpdated = false;
    }

    void ChangeColorScaleValue(bool plus)
    {
        float change = Time.deltaTime / changeSpeed * Mathf.Pow(1.25f, holdMomentum);
        hueSaturationValue[dictStrings[currentString]] += plus ? change : -change;
        UpdateMaterial();
    }

    void ChangeMode(bool forward)
    {
        currentString += forward ? 1 : -1;
        if (currentString < 0)
            currentString = dictStrings.Length + currentString;

        currentString %= dictStrings.Length;
        colorScaleText.text = dictStrings[currentString];
    }

    void UpdateMaterial()
    {
        materialUpdated = true;

        hueSaturationValue["Hue"] %= 1f;

        if (hueSaturationValue["Hue"] < 0f)
            hueSaturationValue["Hue"] = 1f + hueSaturationValue["Hue"];

        hueSaturationValue["Value"] = Mathf.Clamp(hueSaturationValue["Value"], 0f, Mathf.Infinity);
        hueSaturationValue["Saturation"] = Mathf.Clamp(hueSaturationValue["Saturation"], 0f, Mathf.Infinity);

        material.color = Color.HSVToRGB(hueSaturationValue["Hue"], 
            hueSaturationValue["Saturation"], 
            hueSaturationValue["Value"]);

        material.SetFloat("_Metallic", hueSaturationValue["Metallic"]);
        material.SetFloat("_Smoothness", hueSaturationValue["Roughness"]);
    }

    void OnMouseEnter()
    {
        if (Input.GetMouseButton(1))
            material.color = Color.yellow;
    }
}
