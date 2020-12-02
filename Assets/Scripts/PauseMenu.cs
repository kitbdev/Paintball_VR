using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    public bool paused;

    void Start() {
        SetPaused(false);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SetPaused(!paused);
        }
    }
    void SetPaused(bool pause) {
        paused = pause;
        if (paused) {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
