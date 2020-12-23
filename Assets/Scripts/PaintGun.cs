using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public bool aimAtTarget = false;
    public Transform secondHandle;
    public GameObject gunAimGO;
    public InputActionReference shootBtn;
    public InputActionReference nextColorBtn;
    public InputActionReference triggerValue;

    void Start() {
        cam = Camera.main.transform;
        paintHandler = GameObject.FindGameObjectWithTag("PaintHandler").GetComponent<PaintHandler>();
        shootBtn.asset.Enable();
        shootBtn.action.performed += c => { TryShoot(); };
        nextColorBtn.action.performed += c => { NextPaintColor(); };
        triggerValue.action.performed += c => {
            if (c.ReadValue<float>() > 0.1f) { gunAimGO.SetActive(true); } else { gunAimGO.SetActive(false); }
        };
        triggerValue.action.canceled += c => { gunAimGO.SetActive(false); };
    }

    // void Update() {

        // if (Input.GetButton("Fire1")) {

        // }
        // float inpscroll = Input.GetAxis("Mouse ScrollWheel");
        // if (inpscroll != 0) {
        //     colorIndx += -1 * (int)Mathf.Sign(inpscroll);
        //     colorIndx = Mathf.Clamp(colorIndx, -1, 3);
        // }
        // if (Input.GetKeyDown(KeyCode.Alpha1)) {
        //     colorIndx = 0;
        // } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
        //     colorIndx = 1;
        // } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
        //     colorIndx = 2;
        // } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
        //     colorIndx = -1;
        // }
    // }
    void NextPaintColor() {
        if (Time.timeScale == 0) {
            return;
        }
        colorIndx++;
        if (colorIndx >= 3) {
            colorIndx = -1;
        }
        paintBottle.SetColor(paintHandler.GetColor(colorIndx));
    }
    void TryShoot() {
        if (Time.timeScale == 0) {
            return;
        }
        if (Time.time >= lastShotTime + shootRepeatDur) {
            Shoot();
        }
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
        if (aimAtTarget && Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 100, paintableLayer.value)) {
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
