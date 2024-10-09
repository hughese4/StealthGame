using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    public GameObject mainMenuUI;

    void Start() {
        Time.timeScale = 0f;  // Freeze game time
        if (mainMenuUI != null) {
            mainMenuUI.SetActive(true);  // Activate the main menu UI
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            StartGame();
        }
    }

    void StartGame() {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(1);  // Load gameplay scene (Scene 1)
    }
}