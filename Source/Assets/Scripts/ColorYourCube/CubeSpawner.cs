using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject cubes;

    Camera gridCamera;

    public static GameObject[,,] cubeMatrix = new GameObject[5, 5, 5];

    int width, height, depth;

    int[] curCoords = new int[] { 0, 0, 0 };

    void Start()
    {
        width = cubeMatrix.GetLength(1);
        height = cubeMatrix.GetLength(0);
        depth = cubeMatrix.GetLength(2);
        curCoords[0] = width / 2;
        curCoords[1] = height / 2;
        curCoords[2] = depth / 2;
        gridCamera = FindObjectOfType<Camera>();
        GenerateGrid();
        PlaceCamera();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    GameObject spawnedCube = Instantiate(cubes, new Vector3(x, y, z), Quaternion.identity);
                    spawnedCube.name = $"Tile {x} {y} {z}";
                    cubeMatrix[x, y, z] = spawnedCube;

                    if ((x == width / 2) && (y == height / 2) && (z == depth / 2))
                    {
                        GameObject emptyObject = new GameObject("Empty");
                        GameObject pivot = Instantiate(emptyObject, new Vector3(x, y, z), Quaternion.identity);
                        pivot.name = "Pivot";
                        Destroy(emptyObject);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void StartScreenshotCoroutine()
    {
        StartCoroutine(Screenshot());
    }

    IEnumerator Screenshot()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("ScreenshotRemove");

        foreach (GameObject gameObject in gameObjects)
            gameObject.SetActive(false);

        cubeMatrix[curCoords[0], curCoords[1], curCoords[2]].GetComponent<Cube>().OffCursor();

        yield return new WaitForEndOfFrame();

        string folderPath = Directory.GetCurrentDirectory() + "/Screenshots/";

        if (!File.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string screenshotName = "Artwork_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";
        ScreenCapture.CaptureScreenshot(Path.Combine(folderPath + screenshotName));

        yield return new WaitForEndOfFrame();

        foreach (GameObject gameObject in gameObjects)
            gameObject.SetActive(true);

        cubeMatrix[curCoords[0], curCoords[1], curCoords[2]].GetComponent<Cube>().OnCursor(false);
    }

    public void MoveCursor(Vector3Int direction, bool painting)
    {
        cubeMatrix[curCoords[0], curCoords[1], curCoords[2]].GetComponent<Cube>().OffCursor();

        curCoords[0] = (int)Mathf.Ceil(direction.x);
        curCoords[1] = (int)Mathf.Ceil(direction.y);
        curCoords[2] = (int)Mathf.Ceil(direction.z);

        //CheckCoords();

        cubeMatrix[curCoords[0], curCoords[1], curCoords[2]].GetComponent<Cube>().OnCursor(painting);
    }

    //void CheckCoords()
    //{
    //    for (int i = 0; i < curCoords.Length; i++)
    //    {
    //        bool beginSide = curCoords[i] < cubeMatrix.Length / 2;
    //        int cubesToCheck;

    //        if (beginSide)
    //            cubesToCheck = Mathf.Abs(curCoords[i] - 12);
    //        else
    //            cubesToCheck = 25 - curCoords[i];

    //        curCoords[i] = FurthestActiveCube(i, cubesToCheck, beginSide);
    //    }
    //}

    //int FurthestActiveCube(int iteration, int cubes, bool side)
    //{
    //    int x = curCoords[0];
    //    int y = curCoords[1];
    //    int z = curCoords[2];

    //    int mod = side ? -1 : 1;

    //    switch (iteration)
    //    {
    //        case 0:
    //            for (int i = 0; i < cubes; i++)
    //                if ((!cubeMatrix[x + i * mod, y, z].GetComponent<MeshRenderer>().enabled) || i == cubes)
    //                    return x + i * mod;
    //            break;
    //        case 1:
    //            for (int i = 0; i < cubes; i++)
    //                if ((!cubeMatrix[x, y + i * mod, z].GetComponent<MeshRenderer>().enabled) || i == cubes)
    //                    return y + i * mod;
    //            break;
    //        case 2:
    //            for (int i = 0; i < cubes; i++)
    //            {
    //                if ((!cubeMatrix[x, y, z + i * mod].GetComponent<MeshRenderer>().enabled) || i == cubes)
    //                    return x + i * mod;
    //                print(z + i * mod);
    //            }
    //            break;
    //    }
    //    return 69;
    //}

    void PlaceCamera()
    {
        float verticalFOVdeg = gridCamera.fieldOfView;
        float horizontalFOVdeg = Mathf.Atan(Mathf.Tan(gridCamera.fieldOfView * Mathf.Deg2Rad * 0.5f) * gridCamera.aspect) * 2f * Mathf.Rad2Deg;

        float cameraDistance = Mathf.Min(CameraDistanceValue(verticalFOVdeg, height), CameraDistanceValue(horizontalFOVdeg, width));
        gridCamera.transform.position = new Vector3(CameraCenterValue(width), CameraCenterValue(height), cameraDistance * 1.5f);
    }

    public void DestroyCube()
    {
        cubeMatrix[curCoords[0], curCoords[1], curCoords[2]].GetComponent<Cube>().Destroy();
    }

    float CameraCenterValue(float value)
    {
        float result = (value / 2f) - 0.5f;
        return result;
    }

    float CameraDistanceValue(float angle, float value)
    {
        float result = (-value / (2f * Mathf.Tan(angle / 2f * Mathf.Deg2Rad))) - 0.5f;
        return result;
    }
}
