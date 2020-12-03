using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TintCrosshair : MonoBehaviour {

    public RawImage crosshair;
    public Vector3 highlightScale = Vector3.one * 1.2f;
    public Color highlightColor = Color.red;
    Vector3 baseScale;
    Color baseColor;
    Transform cam;

    void Start() {
        cam = Camera.main.transform;
        baseScale = crosshair.transform.localScale;
        baseColor = crosshair.color;
    }

    void LateUpdate() {
        if (Time.timeScale == 0) {
            return;
        }
        if (Physics.Raycast(cam.position, cam.forward, out var hit, 100)) {
            if (hit.collider.CompareTag("Target")) {
                HighlightCrosshair(true);
            } else {
                HighlightCrosshair(false);
            }
        }
    }
    void HighlightCrosshair(bool highlight) {
        crosshair.transform.localScale = highlight ? highlightScale : baseScale;
        crosshair.color = highlight ? highlightColor : baseColor;
    }
}
