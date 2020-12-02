using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintball1 : MonoBehaviour
{
    public float speed = 10;
    void Start()
    {
        
    }

    void Update()
    {
        // transform.Translate(transform.forward * speed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision other) {
        //destroy
    }
}
