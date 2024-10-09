using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GameObject pauseMenuUI;
    private bool gameIsPaused = false;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (gameIsPaused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }

        if (gameIsPaused) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                RestartLevel();  // Restarts the current level
            } else if (Input.GetKeyDown(KeyCode.M)) {
                LoadMainMenu();  // Loads the main menu scene
            }
        }
    }

    public void PauseGame() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;  // Freeze game time
        gameIsPaused = true;
    }

    public void ResumeGame() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;  // Resume game time
        gameIsPaused = false;
    }

    public void RestartLevel() {
        Time.timeScale = 1f;  // Ensure game time resumes
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Reload current scene
    }

    public void LoadMainMenu() {
        Time.timeScale = 1f;  // Ensure game time resumes
        SceneManager.LoadScene(0);  // Load main menu (Scene 0)
    }
}
