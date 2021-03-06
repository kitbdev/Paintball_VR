using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour {
    public bool paused;
    CanvasGroup pauseMenu;
    public Text startbtnText;
    public InputActionReference pauseBtn;
    public List<GameObject> pauseOnlyGOs = new List<GameObject>();
    public List<GameObject> unpauseOnlyGOs = new List<GameObject>();

    void Start() {
        pauseBtn.asset.Enable();
        pauseBtn.action.performed += c => {
            if (!paused) {
                ShowMenu();
            } else {
                StartGame();
            }
        };
        pauseMenu = GetComponent<CanvasGroup>();
        // SetPaused(false);
#if !UNITY_EDITOR
        paused = true;
#endif
        if (paused) {
            ShowMenu();
        } else {
            StartGame();
        }
    }

    void Update() {
        // if (Input.GetKeyDown(KeyCode.Escape)) {
        //     // SetPaused(!paused);
        //     if (!paused) {
        //         ShowMenu();
        //     } else {
        //         StartGame();
        //     }
        // }
    }
    public void SetPaused(bool pause) {
        paused = pause;
        if (paused) {
            Time.timeScale = 0;
            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
        } else {
            Time.timeScale = 1;
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }
        foreach (var pogo in pauseOnlyGOs) {
            pogo.SetActive(paused);
        }
        foreach (var pogo in unpauseOnlyGOs) {
            pogo.SetActive(!paused);
        }
    }
    public void StartGame() {
        SetPaused(false);
        pauseMenu.alpha = 0;
        pauseMenu.blocksRaycasts = false;
        pauseMenu.interactable = false;
        startbtnText.text = "Resume";
    }
    public void RestartGame() {
        SceneManager.LoadScene(0);
        StartGame();
    }
    public void ShowMenu() {
        SetPaused(true);
        pauseMenu.alpha = 1;
        pauseMenu.blocksRaycasts = true;
        pauseMenu.interactable = true;
    }
    public void OptionsBtn() {
        // todo
    }
    public void Exit() {
        Application.Quit();
    }
}
