using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintGun : MonoBehaviour {
    public GameObject paintballPrefab;
    public GameObject shootFxPrefab;
    public Transform shootpoint;
    public LayerMask paintableLayer;
    public float shootForce = 10;
    [Range(-1, 3)]
    public int colorIndx = 0;
    public float shootRepeatDur = 0.1f;
    public float lastShotTime = 0;
    Transform cam;
    public Rigidbody rb;
    public Bottle paintBottle;
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
        paintBottle.SetColor(paintHandler.GetColor(colorIndx));
        // todo tint crosshair?
    }
    void Shoot() {
        GameObject sfxgo = Instantiate(shootFxPrefab, shootpoint.position, shootpoint.rotation);
        Color color = paintHandler.GetColor(colorIndx);
        var sfxps = sfxgo.GetComponent<ParticleSystem>().main;
        sfxps.startColor = new ParticleSystem.MinMaxGradient(color);
        sfxgo.transform.parent = transform;
        GameObject pbgo = Instantiate(paintballPrefab, shootpoint.position, shootpoint.rotation);
        PaintProjectile projectile = pbgo.GetComponent<PaintProjectile>();

        Vector3 shootDir = pbgo.transform.forward;
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 100, paintableLayer.value)) {
            shootDir = hit.point - shootpoint.position;
            shootDir.Normalize();
        }

        projectile.colorIndx = colorIndx;
        Renderer r = pbgo.GetComponentInChildren<Renderer>();
        r.material.SetColor("_Tint", color);

        Rigidbody rb = pbgo.GetComponent<Rigidbody>();
        rb.AddForce(shootForce * shootDir + rb.velocity, ForceMode.Impulse);
        lastShotTime = Time.time;
    }
}
