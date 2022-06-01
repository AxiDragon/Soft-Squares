using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommitDie : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DieLol()); 
    }

    IEnumerator DieLol()
    {
        yield return new WaitForSecondsRealtime(0.6f);
        Destroy(transform.root.gameObject);
    }
}
