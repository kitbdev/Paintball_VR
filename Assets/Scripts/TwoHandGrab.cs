using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandGrab : MonoBehaviour {

    public Transform firstHand;
    XRBaseInteractor activeInteractor;
    Quaternion initialRot;
    Quaternion initialRotOffset;

    private void Update() {
        if (activeInteractor != null && activeInteractor.enabled) {
            Vector3 newForward = activeInteractor.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(newForward, firstHand.up);
        }
    }
    public void On2hSelect(XRBaseInteractor interactor) {
        initialRot = transform.localRotation;
        activeInteractor = interactor;
        // initialRotOffset = Quaternion.Inverse();
    }
    public void On2hDeselect(XRBaseInteractor interactor) {
        activeInteractor = null;
        transform.localRotation = initialRot;
    }
}
