using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class BrushColorBackup : MonoBehaviour
{
    [System.NonSerialized]
    public Material brushMaterial;

    Text colorScaleText;

    string[] dictStrings = new string[] { "Hue", "Saturation", "Value", "Roughness", "Metallic" };
    int currentString = 0;
    Dictionary<string, float> hueSaturationValue = new Dictionary<string, float>();

    float changeSpeed = 5f;
    float holdMomentum = 1f;
    float holdMomentumChange = 0.001f;
    float modeChangeTime;
    float modeChangeTimeChange = .75f;

    bool brushValueUp, brushValueDown, modeDown, modeUp, paintMode, brushMaterialUpdated = false;

    SerialPort controller = new SerialPort("COM3", 9600);

    void Start()
    {
        controller.ReadTimeout = 1000;

        controller.Open();

        foreach (string dictString in dictStrings)
            hueSaturationValue.Add(dictString, 0f);

        hueSaturationValue["Value"] = 1f;

        brushMaterial = GetComponent<MeshRenderer>().material;
        colorScaleText = GameObject.Find("ColorScaleText").GetComponent<Text>();

        colorScaleText.text = dictStrings[currentString];

        modeChangeTime = Time.time + modeChangeTimeChange;

        StartCoroutine(ReadFromSerialPort());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) || (modeUp && !modeDown))
            ChangeMode(true);

        if (Input.GetKeyDown(KeyCode.A) || (modeDown && !modeUp))
            ChangeMode(false);

        if (Input.GetKey(KeyCode.W) || (brushValueUp && !brushValueDown))
            ChangeColorScaleValue(true);

        if (Input.GetKey(KeyCode.S) || (!brushValueUp && brushValueDown))
            ChangeColorScaleValue(false);

        if (brushMaterialUpdated)
            holdMomentum += holdMomentumChange;
        else
            holdMomentum = 1f;

        brushMaterialUpdated = false;
    }

    void ChangeColorScaleValue(bool plus)
    {
        float change = Time.deltaTime / changeSpeed * Mathf.Pow(1.25f, holdMomentum);
        hueSaturationValue[dictStrings[currentString]] += plus ? change : -change;
        UpdateMaterial();
    }

    void ChangeMode(bool forward)
    {
        if (modeChangeTime > Time.time)
            return;

        modeChangeTime = Time.time + modeChangeTimeChange;

        currentString += forward ? 1 : -1;
        if (currentString < 0)
            currentString = dictStrings.Length + currentString;

        currentString %= dictStrings.Length;
        colorScaleText.text = dictStrings[currentString];
    }

    void UpdateMaterial()
    {
        brushMaterialUpdated = true;

        hueSaturationValue["Hue"] %= 1f;

        if (hueSaturationValue["Hue"] < 0f)
            hueSaturationValue["Hue"] = 1f + hueSaturationValue["Hue"];

        hueSaturationValue["Value"] = Mathf.Clamp(hueSaturationValue["Value"], 0f, Mathf.Infinity);
        hueSaturationValue["Saturation"] = Mathf.Clamp(hueSaturationValue["Saturation"], 0f, Mathf.Infinity);

        brushMaterial.color = Color.HSVToRGB(hueSaturationValue["Hue"],
            hueSaturationValue["Saturation"],
            hueSaturationValue["Value"]);

        brushMaterial.SetFloat("_Metallic", hueSaturationValue["Metallic"]);
        brushMaterial.SetFloat("_Smoothness", hueSaturationValue["Roughness"]);
    }

    IEnumerator ReadFromSerialPort()
    {
        while (true)
        {
            if (controller.BytesToRead > 0)
            {
                switch (controller.ReadLine())
                {
                    case "1C":
                        brushValueUp = !brushValueUp;
                        break;
                    case "2C":
                        brushValueUp = !brushValueUp;
                        break;
                    case "1D":
                        brushValueDown = !brushValueDown;
                        break;
                    case "2D":
                        brushValueDown = !brushValueDown;
                        break;
                    case "1M":
                        modeUp = !modeUp;
                        break;
                    case "2M":
                        modeUp = !modeUp;
                        break;
                    case "1J":
                        modeDown = !modeDown;
                        break;
                    case "2J":
                        modeDown = !modeDown;
                        break;
                    case "1P":
                        paintMode = !paintMode;
                        break;
                }
            }
            yield return new WaitForSecondsRealtime(0.07f);
        }
    }
}
