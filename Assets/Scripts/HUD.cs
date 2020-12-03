using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {
    
    // display hits, paint %, etc
    void Start() {
        
    }

    void Update() {
        if (Time.timeScale == 0) {
            return;
        }
        // todo tint crosshair?
    }
}
