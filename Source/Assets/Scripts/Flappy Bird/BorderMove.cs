using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BorderMove : MonoBehaviour
{
    Movement movement;
    GameObject bird;
    Vector3 birdStart;
    void Start()
    {
        bird = GameObject.FindGameObjectWithTag("Player");
        birdStart = bird.transform.position;
        movement = FindObjectOfType<Movement>().GetComponent<Movement>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            Restart();
        }
    }

    public void Restart()
    {
        bird.transform.position = birdStart;
        bird.GetComponent<Rigidbody>().velocity = Vector3.zero;
        movement.SendSerialMessage("o");
    }
}
