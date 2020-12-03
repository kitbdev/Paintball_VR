using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// keep track of hits and updates its ui
public class Target : MonoBehaviour {

    public TMP_Text hitText;
    public int hits { get; private set; } = 0;
    public float coverage { get; private set; } = 0;
    void Start() {
        hits = 0;
        coverage = 0;
        UpdateText();
    }
    private void OnCollisionEnter(Collision other) {
        if (other.collider.gameObject.layer == LayerMask.NameToLayer("Paintball")) {
            hits++;
            UpdateText();
        }
    }
    public void UpdateText() {
        hitText.text = hits+"";
    }

}
