using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastShoot : MonoBehaviour
{
    GameObject player, directLight;
    public GameObject hitMark;
    Vector3 lightVector, reversedVector;
    public float rayLength = 100f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        directLight = FindObjectOfType<Light>().gameObject;
        StartCoroutine(ShootLight());
    }

    IEnumerator ShootLight()
    {
        while (true)
        {
            lightVector = directLight.transform.forward;
            reversedVector = -lightVector;

            RaycastHit hitObject;
            if (Physics.Raycast(player.transform.position, reversedVector, out hitObject, rayLength))
            {
                if (hitObject.collider.gameObject != hitMark)
                    Instantiate(hitMark, hitObject.point, Quaternion.Euler(hitObject.normal));
            }
            else
                LightRotation.HitByLight();

            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
