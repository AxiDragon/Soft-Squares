using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    Rigidbody rb;
    GameObject sphereCheckPos;
    LayerMask ground;
    [SerializeField]
    public float extraForce = 1;

    bool isGrounded;

    void Start()
    {
        ground = LayerMask.GetMask("Ground");
        sphereCheckPos = GameObject.Find("SphereCheck");
        rb = GetComponent<Rigidbody>(); 
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(sphereCheckPos.transform.position, 0.4f, ground);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            rb.AddForce(Vector3.up * extraForce * Time.deltaTime, ForceMode.Impulse);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveVector = new Vector3(horizontal, 0f, vertical);
        moveVector = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * moveVector;

        rb.AddForce(moveVector * extraForce * Time.deltaTime);
    }
}
