using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GameObject pauseMenuUI;
    public GameObject gameLoseUI;
    public GameObject gameWinUI;

    private bool gameIsPaused = false;
    bool gameIsOver;

    private Player playerInstance;
    private static GameController instance;


    public static bool isPersistentSceneLoaded = false;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Keep this controller alive
        }        
    }


    void Start() {

        SetupEventListeners();

        // Check if persistent scene is already loaded
        if (!isPersistentSceneLoaded) {
            SceneManager.LoadScene("GlobalObjects", LoadSceneMode.Additive);
            isPersistentSceneLoaded = true;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != null) {
            mainCamera.gameObject.SetActive(true);  // Ensure the camera is active
        } else {
            Debug.LogError("No camera found!");
        }
    
        playerInstance = FindObjectOfType<Player>();
        
    }

    void Update() {
        if (gameIsOver) {
            // Check if the loss UI is active
            if (gameLoseUI.activeSelf) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    RestartLevel();  // Restart the level
                    gameLoseUI.SetActive(false);  // Hide the game lose UI
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (gameIsPaused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }

        if (gameIsPaused && pauseMenuUI.activeSelf) {
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
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        gameIsPaused = false;
        gameIsOver = false;

        string currentLevelName = SceneManager.GetActiveScene().name;  // Get current level
        Debug.Log("Active scene on restart: " + currentLevelName);
        StartCoroutine(ReloadLevelAndResetPlayer(currentLevelName));
    }

    IEnumerator ReloadLevelAndResetPlayer(string levelName) {
        
        // if it's not the persistent scene
        if (SceneManager.GetSceneByName(levelName).isLoaded && SceneManager.GetActiveScene().name != "GlobalObjects") {
            yield return SceneManager.UnloadSceneAsync(levelName);
            // Reload the level
            yield return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            // Set the level as the active scene
            Scene levelScene = SceneManager.GetSceneByName(levelName);
            SceneManager.SetActiveScene(levelScene);
        }

        SetupEventListeners();  // Re-subscribe to events after reload

        // Reset player position to spawn after scene is fully loaded
        if (playerInstance != null) {
            playerInstance.ResetToSpawn();
        }
    }

    // Helper method to set up event listeners
    private void SetupEventListeners() {
        Guard.OnGuardHasSpottedPlayer += ShowGameLoseUI;
    }

    public void LoadMainMenu() {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(false);
        gameIsPaused = false;
        string currentLevelName = SceneManager.GetActiveScene().name;

        // Unload the current level
        StartCoroutine(UnloadLevelAndLoadMainMenu(currentLevelName));
    }

    private IEnumerator UnloadLevelAndLoadMainMenu(string currentLevelName) {
        if (currentLevelName != "GlobalObjects") {
            // Unload the current level
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentLevelName);

            // Wait until the level is fully unloaded
            yield return unloadOperation;
        }

        // Now load the main menu scene
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);  // Load the main menu 
    }

    void ShowGameWinUI() {
        OnGameOver(gameWinUI);
    }

    public void ShowGameLoseUI() {
        OnGameOver(gameLoseUI);
    }

    void OnGameOver(GameObject gameOverUI) {
        if (gameOverUI != null && !gameIsOver) {
            gameOverUI.SetActive(true);
            gameIsOver = true;
        }
        Guard.OnGuardHasSpottedPlayer -= ShowGameLoseUI;        
    }
}
