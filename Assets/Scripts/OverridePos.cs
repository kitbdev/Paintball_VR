using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverridePos : MonoBehaviour {
    
    public Vector3 offsetpos;
    public Quaternion offsetrot;

    private void Awake() {
        offsetpos = transform.localPosition;
        offsetrot = transform.localRotation;
    }

    void LateUpdate() {
        transform.localPosition = offsetpos;
        transform.localRotation = offsetrot;
    }
}
