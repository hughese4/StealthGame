using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    public GameObject mainMenuUI;

    void Start() {
        if (!GameController.isPersistentSceneLoaded) {
            SceneManager.LoadScene("GlobalObjects", LoadSceneMode.Additive);
            GameController.isPersistentSceneLoaded = true;
        }

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
        Time.timeScale = 1f;  // Unpause the game
        StartCoroutine(LoadLevelAndResetPlayer("Level_1"));
    }

    IEnumerator LoadLevelAndResetPlayer(string levelName) {
        // Check if Level_1 is already loaded
        bool isLevelLoaded = false;
        Scene levelScene = default;

        for (int i = 0; i < SceneManager.sceneCount; ++i) {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == levelName) {
                isLevelLoaded = true;
                levelScene = scene;
                break;
            }
        }

        // If Level_1 isn't loaded, load it
        if (!isLevelLoaded) {
            // Load the level and get its reference
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            // Wait until the level has finished loading
            yield return new WaitUntil(() => asyncLoad.isDone);          
            
            levelScene = SceneManager.GetSceneByName(levelName);  // Get the loaded level scene
        }

        // Set the level as the active scene
        SceneManager.SetActiveScene(levelScene);

        // Find the Player object in the now active 'Level_1' and reset to spawn        
        if (Player.instance != null) {
            Player.instance.ResetToSpawn();
        } else {
            Debug.LogError("Player object not found in level scene.");
        }

        // Unload the MainMenu
        yield return SceneManager.UnloadSceneAsync("MainMenu");
        
    }
}