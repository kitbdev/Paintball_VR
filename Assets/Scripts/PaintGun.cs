using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintGun : MonoBehaviour {
    public GameObject paintballPrefab;
    public Transform shootpoint;
    public LayerMask paintableLayer;
    public float shootForce = 10;
    [Range(-1, 3)]
    public int colorIndx = 0;
    public float shootRepeatDur = 0.1f;
    public float lastShotTime = 0;
    Transform cam;
    public Rigidbody rb;
    PaintHandler paintHandler;

    void Start() {
        cam = Camera.main.transform;
        paintHandler = GameObject.FindGameObjectWithTag("PaintHandler").GetComponent<PaintHandler>();
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
        float inpscroll = Input.GetAxis("Mouse ScrollWheel");
        bool inp1 = Input.GetKeyDown(KeyCode.Alpha1);
        bool inp2 = Input.GetKeyDown(KeyCode.Alpha2);
        bool inp3 = Input.GetKeyDown(KeyCode.Alpha3);
        bool inp4 = Input.GetKeyDown(KeyCode.Alpha4);
        if (inpscroll != 0) {
            colorIndx += -1 * (int)Mathf.Sign(inpscroll);
            colorIndx = Mathf.Clamp(colorIndx, -1, 3);
        }
        if (inp1) {
            colorIndx = 0;
        } else if (inp2) {
            colorIndx = 1;
        } else if (inp3) {
            colorIndx = 2;
        } else if (inp4) {
            colorIndx = -1;
        }
        // todo tint crosshair?
    }
    void Shoot() {
        GameObject pb = Instantiate(paintballPrefab, shootpoint.position, shootpoint.rotation);
        PaintProjectile projectile = pb.GetComponent<PaintProjectile>();

        Vector3 shootDir = pb.transform.forward;
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 100, paintableLayer.value)) {
            shootDir = hit.point - shootpoint.position;
            shootDir.Normalize();
        }

        projectile.colorIndx = colorIndx;
        Renderer r = pb.GetComponentInChildren<Renderer>();
        r.material.SetColor("_Tint", paintHandler.GetColor(colorIndx));

        Rigidbody rb = pb.GetComponent<Rigidbody>();
        rb.AddForce(shootForce * shootDir + rb.velocity, ForceMode.Impulse);
        lastShotTime = Time.time;
    }
}
