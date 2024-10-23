using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PersistentEventSystem : MonoBehaviour {
    private static PersistentEventSystem instance;
    private static bool isGlobalSceneLoaded = false;  // Flag to ensure persistent scene isn't loaded multiple times

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Keep this EventSystem alive
        } 
    }

    // Static method to ensure the persistent scene is only loaded once
    public static void EnsureGlobalObjectsLoaded() {
        if (!isGlobalSceneLoaded) {
            SceneManager.LoadScene("GlobalObjects", LoadSceneMode.Additive);  // Load persistent objects scene
            isGlobalSceneLoaded = true;  // Set flag to true so it's not loaded again
        }
    }
}
