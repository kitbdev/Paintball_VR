using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintProjectile : MonoBehaviour {
    public float speed = 10;
    public int colorIndx = 0;
    public float ttl = 10;
    public GameObject splatFXPrefab;
    PaintHandler paintHandler;

    void Start() {
        paintHandler = GameObject.FindGameObjectWithTag("PaintHandler").GetComponent<PaintHandler>();
        Destroy(gameObject, ttl);
    }

    void Update() {
        // transform.Translate(transform.forward * speed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision other) {
        var contact = other.GetContact(0);
        Color color = paintHandler.GetColor(colorIndx);
        paintHandler.PaintSplat(transform.position, contact.point - transform.position, colorIndx);
        GameObject splatgo = Instantiate(splatFXPrefab, contact.point, Quaternion.Euler(contact.normal));
        ParticleSystem[] pss = splatgo.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < pss.Length; i++) {
            var main = pss[i].main;
            main.startColor = new ParticleSystem.MinMaxGradient(color);
        }
        Renderer[] rs = splatgo.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rs.Length; i++) {
            rs[i].material.color = color;
        }
        Destroy(gameObject, 0.01f);
    }
}
