using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TintCrosshair : MonoBehaviour {

    public Transform crosshairT;
    public Renderer crosshair;
    public Material otherCrosshairMaterial;
    public LineRenderer lineRenderer;
    public LayerMask layerMask;
    public Vector3 highlightScale = Vector3.one * 1.2f;
    public Color highlightColor = Color.red;
    Vector3 baseScale;
    Color baseColor;
    Transform cam;

    void Start() {
        cam = Camera.main.transform;
        baseScale = crosshair.transform.localScale;
        baseColor = crosshair.sharedMaterial.color;
    }

    void LateUpdate() {
        if (Time.timeScale == 0) {
            return;
        }
        // move crosshair to target
        if (Physics.Raycast(transform.position, transform.forward, out var hit, 100,layerMask.value)) {
            crosshairT.position = hit.point + hit.normal * 0.001f;
            crosshairT.forward = hit.normal;
            // lineRenderer.SetPosition(1, lineRenderer.transform.InverseTransformPoint(hit.point));
            if (hit.collider.CompareTag("Target")) {
                HighlightCrosshair(true);
            } else {
                HighlightCrosshair(false);
            }
        } else {
            crosshairT.position = Vector3.down * 10;
            // lineRenderer.SetPosition(1,Vector3.forward*10);
        }
    }
    void HighlightCrosshair(bool highlight) {
        crosshair.transform.localScale = highlight ? highlightScale : baseScale;
        crosshair.sharedMaterial.color = highlight ? highlightColor : baseColor;
    }
}
