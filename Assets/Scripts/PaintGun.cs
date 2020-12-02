using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintGun : MonoBehaviour {
    public GameObject paintballPrefab;
    public Transform shootpoint;
    public float shootForce = 10;
    [Range(-1,3)]
    public int colorIndx = 0;
    public float shootRepeatDur = 0.1f;
    public float lastShotTime = 0;
    Transform cam;

    void Start() {
        cam = Camera.main.transform;
    }

    void Update() {
        if (Time.timeScale == 0) {
            return;
        }
        if (Input.GetButton("Fire1")) {
            if (Time.time >= lastShotTime + shootRepeatDur) {
                Shoot();
            }
        }
    }
    void Shoot() {
        GameObject pb = Instantiate(paintballPrefab, shootpoint.position, shootpoint.rotation);
        PaintProjectile projectile = pb.GetComponent<PaintProjectile>();
        Vector3 shootDir = pb.transform.forward;
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 100)) {
            shootDir = hit.point - shootpoint.position;
            shootDir.Normalize();
        }
        projectile.colorIndx = colorIndx;
        Rigidbody rb = pb.GetComponent<Rigidbody>();
        rb.AddForce(shootForce * shootDir, ForceMode.Impulse);
        lastShotTime = Time.time;
    }
}
