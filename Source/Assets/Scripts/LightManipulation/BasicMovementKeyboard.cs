using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovementKeyboard : MonoBehaviour
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

        Vector3 moveVector = Vector3.forward * vertical;
        moveVector = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * moveVector;

        rb.AddForce(moveVector * extraForce * Time.deltaTime);
        rb.MoveRotation(Quaternion.Euler(rb.transform.eulerAngles + (Vector3.up * horizontal * 5)));
    }
}
