using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteLocation : MonoBehaviour
{
    void Start() => transform.position += Vector3.right * (Screen.width / 1250f);
}
