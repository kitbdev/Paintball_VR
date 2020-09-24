using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed = 10f;

    void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
