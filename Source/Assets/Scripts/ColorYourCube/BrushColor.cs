using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class BrushColor : MonoBehaviour
{
    [System.NonSerialized]
    public Material brushMaterial;

    Text colorScaleText, modeText;

    string[] dictStrings = new string[] { "Hue", "Saturation", "Value", "Roughness", "Metallic" };
    string[] modeStrings = new string[] { "Color", "Move", "View", "Paint" };
    int currentString = 0;
    int currentMode = 0;
    Dictionary<string, float> hueSaturationValue = new Dictionary<string, float>();

    float changeSpeed = 5f;
    float holdMomentum = 1f;
    float holdMomentumChange = 0.001f;

    public float cursorMoveInterval;
    float lastCursorMove;

    bool brushValueUp, brushValueDown, goLeft, goRight, brushMaterialUpdated = false;

    SerialPort controller = new SerialPort("COM3", 9600);

    CubeSpawner cubeSpawner;
    CameraMove cameraMove;
    GameObject mainCamera;
    int[] cursorCoordinates = new int[] { 5, 5, 0 };

    void Start()
    {
        controller.ReadTimeout = 1000;

        controller.Open();

        foreach (string dictString in dictStrings)
            hueSaturationValue.Add(dictString, 0.5f);

        cubeSpawner = FindObjectOfType<CubeSpawner>();
        cameraMove = FindObjectOfType<CameraMove>();
        brushMaterial = GetComponent<MeshRenderer>().material;
        colorScaleText = GameObject.Find("ColorScaleText").GetComponent<Text>();
        modeText = GameObject.Find("ModeText").GetComponent<Text>();
        mainCamera = Camera.main.gameObject;

        colorScaleText.text = dictStrings[currentString];
        modeText.text = modeStrings[currentMode];

        lastCursorMove = Time.time;

        StartCoroutine(ReadFromSerialPort());

        UpdateMaterial();
    }

    void FixedUpdate()
    {
        if (brushValueUp && !brushValueDown)
        {
            switch (modeStrings[currentMode])
            {
                case "Color":
                    ChangeColorScaleValue(true);
                    break;
                case "View":
                    cameraMove.UpdateMode(2f, 'Y');
                    break;
                default:
                    CursorMove("Up");
                    break;
            }
        }

        if (!brushValueUp && brushValueDown)
        {
            switch (modeStrings[currentMode])
            {
                case "Color":
                    ChangeColorScaleValue(false);
                    break;
                case "View":
                    cameraMove.UpdateMode(-2f, 'Y');
                    break;
                default:
                    CursorMove("Down");
                    break;
            }
        }

        if (brushValueUp == brushValueDown)
        {
            cameraMove.UpdateMode(0f, 'Y');
        }

        if (goRight && !goLeft)
        {
            switch (modeStrings[currentMode])
            {
                case "Move":
                case "Paint":
                    CursorMove("Right");
                    break;
                case "View":
                    cameraMove.UpdateMode(-2f, 'X');
                    break;
            }
        }

        if (!goRight && goLeft)
        {
            switch (modeStrings[currentMode])
            {
                case "Move":
                case "Paint":
                    CursorMove("Left");
                    break;
                case "View":
                    cameraMove.UpdateMode(2f, 'X');
                    break;
            }
        }

        if (goRight == goLeft)
        {
            cameraMove.UpdateMode(0f, 'X');
        }

        if (brushMaterialUpdated)
            holdMomentum += holdMomentumChange;
        else
        {
            holdMomentum = 1f;
            brushMaterialUpdated = false;
        }
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

    void ChangePaintMode()
    {
        currentMode++;
        currentMode %= modeStrings.Length;

        modeText.text = modeStrings[currentMode];
    }

    void CursorMove(string direction)
    {
        if (lastCursorMove > Time.time)
            return;

        lastCursorMove = Time.time + cursorMoveInterval;

        //for camera rotation 0 0 0 - up is 0 0 1, down is 0 0 -1, right is 1 0 0 and left is -1 0 0

        Vector3 change = Vector3.zero;

        switch (direction)
        {
            case "Up":
                change = CameraMove.rotateVectorPerpendicular;
                break;
            case "Down":
                change = -CameraMove.rotateVectorPerpendicular;
                break;
            case "Right":
                change = CameraMove.rotateVector;
                break;
            case "Left":
                change = -CameraMove.rotateVector;
                break;
        }

        Vector3Int intChange = new Vector3Int(Mathf.RoundToInt(change.x), Mathf.RoundToInt(change.y), Mathf.RoundToInt(change.z));

        for (int i = 0; i < cursorCoordinates.Length; i++)
        {
            switch (i)
            {
                case 0:
                    cursorCoordinates[i] += intChange.x;
                    break;
                case 1:
                    cursorCoordinates[i] += intChange.y;
                    break;
                case 2:
                    cursorCoordinates[i] += intChange.z;
                    break;
            }

            cursorCoordinates[i] = Mathf.Clamp(cursorCoordinates[i], 0, CubeSpawner.cubeMatrix.GetLength(0) - 1);
        }

        cubeSpawner.MoveCursor(new Vector3Int(cursorCoordinates[0], cursorCoordinates[1], cursorCoordinates[2]), modeStrings[currentMode] == "Paint");
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
                string serialOutput = controller.ReadLine();

                switch (serialOutput)
                {
                    case "1C":
                    case "2C":
                        brushValueDown = !brushValueDown;
                        break;
                    case "1D":
                    case "2D":
                        brushValueUp = !brushValueUp;
                        break;
                    case "1M":
                        switch (modeStrings[currentMode])
                        {
                            case "Color":
                                ChangeMode(false);
                                break;
                            default:
                                goLeft = !goLeft;
                                break;
                        }
                        break;

                    case "2M":
                        switch (modeStrings[currentMode])
                        {
                            case "Color":
                                break;
                            default :
                                goLeft = !goLeft;
                                break;
                        }
                        break;

                    case "1J":
                        switch (modeStrings[currentMode])
                        {
                            case "Color":
                                ChangeMode(true);
                                break;
                            default:
                                goRight = !goRight;
                                break;
                        }
                        break;

                    case "2J":
                        switch (modeStrings[currentMode])
                        {
                            case "Color":
                                break;
                            default :
                                goRight = !goRight;
                                break;
                        }
                        break;

                    case "1P":
                        ChangePaintMode();
                        brushValueUp = brushValueDown = goLeft = goRight = brushMaterialUpdated = false;
                        break;
                    case "2P":
                        break;
                    case "3R":
                        cubeSpawner.DestroyCube();
                        break;

                    case "1S":
                        cubeSpawner.StartScreenshotCoroutine();
                        break;

                    default:
                        cameraMove.UpdateMode(serialOutput);
                        break;
                }
            }
            yield return new WaitForSecondsRealtime(0.07f);
        }
    }
}
