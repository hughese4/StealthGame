using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;

    // Start is called before the first frame update
    void Start() {
        Guard.OnGuardHasSpottedPlayer += ShowGameLoseUI;
        FindObjectOfType<Player>().OnReachedEndOfLevel += ShowGameWinUI;
    }

    // Update is called once per frame
    void Update() {
        if (gameIsOver) {
            if(Input.GetKeyDown(KeyCode.Space)) {
                SceneManager.LoadScene(1);
            }
        }
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
        FindObjectOfType<Player>().OnReachedEndOfLevel -= ShowGameWinUI;
    }
}
