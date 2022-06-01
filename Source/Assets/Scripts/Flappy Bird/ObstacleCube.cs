using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCube : MonoBehaviour
{
    BorderMove border;
    void Start()
    {
        border = FindObjectOfType<BorderMove>().GetComponent<BorderMove>();
    }

    void FixedUpdate()
    {
        transform.position += Vector3.left;   
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            border.Restart();
        }
    }
}
