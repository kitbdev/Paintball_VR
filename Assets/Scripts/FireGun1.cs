using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGun1 : MonoBehaviour
{
    public GameObject paintballPrefab;
    public Transform shootpoint;
    public float shootForce = 10;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) {
            Shoot();
        }
    }
    void Shoot() {
        GameObject pb = Instantiate(paintballPrefab, shootpoint.position, shootpoint.rotation);
        var rb = pb.GetComponent<Rigidbody>();
        rb.AddForce(shootForce * pb.transform.forward, ForceMode.Impulse);
    }
}
