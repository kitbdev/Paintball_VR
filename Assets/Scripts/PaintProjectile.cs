using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintProjectile : MonoBehaviour
{
    public float speed = 10;
    public int colorIndx = 0;
    public float ttl = 10;
    public GameObject splatFXPrefab;
    PaintHandler paintHandler;

    void Start()
    {
        paintHandler = GameObject.FindGameObjectWithTag("PaintHandler").GetComponent<PaintHandler>();
        Destroy(gameObject, ttl);
    }

    void Update()
    {
        // transform.Translate(transform.forward * speed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision other) {
        var contact = other.GetContact(0);
        paintHandler.PaintSplat(transform.position, contact.point - transform.position, colorIndx);
        Instantiate(splatFXPrefab, contact.point, Quaternion.Euler(contact.normal));
        Destroy(gameObject, 0.01f);
    }
}
