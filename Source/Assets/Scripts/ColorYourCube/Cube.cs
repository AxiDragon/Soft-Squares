using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    Material brushMat;
    GameObject selectors;

    void Start()
    {
        brushMat = GameObject.Find("Palette").GetComponent<BrushColor>().brushMaterial;
        selectors = transform.Find("CubeSelectors").gameObject;
        selectors.SetActive(false);
    }

    public void OnCursor(bool paint)
    {
        if (paint)
            UpdateMaterial();

        selectors.SetActive(true);
    }

    public void OffCursor()
    {
        selectors.SetActive(false);
    }

    public void Destroy()
    {
        transform.root.GetComponent<MeshRenderer>().enabled = false;
    }

    void UpdateMaterial()
    {
        Material mat = GetComponent<Renderer>().material;
        mat.color = brushMat.color;
        mat.SetFloat("_Metallic", brushMat.GetFloat("_Metallic"));
        mat.SetFloat("_Smoothness", brushMat.GetFloat("_Smoothness"));
    }
}