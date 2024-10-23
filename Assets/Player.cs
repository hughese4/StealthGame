using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public static Player instance;

    public GameObject levelEndUI;
    bool beatLevel;

    public float moveSpeed = 7;
    public float smoothMoveTime = .05f;
    public float turnSpeed = 8;

    public float levelTime;
    private bool levelStarted;
    public TMPro.TextMeshProUGUI timerText;

    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;

    new Rigidbody rigidbody;
    bool disabled;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        //rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        ResetToSpawn();

        Guard.OnGuardHasSpottedPlayer += Disable;
    }

    public void ResetToSpawn() {
        GameObject spawnPoint = GameObject.FindWithTag("Spawn");
        if (spawnPoint != null) {
            transform.position = spawnPoint.transform.position;
            Enable();
            levelTime = 0f; // Reset timer at the start
            levelStarted = true; // Start the timer
        }
    }

    // Update is called once per frame
    void Update() {
        if (levelStarted && !beatLevel) {
            levelTime += Time.deltaTime; // Increment time
        }

        // for end of level
        if (beatLevel) {
            if (Input.GetKeyDown(KeyCode.N)) {
                LoadNextLevel();
            }
        }

        
    }

    void OnTriggerEnter(Collider hitCollider) {
        if (hitCollider.tag == "Finish") {
            Disable();

            beatLevel = true;
            // show level end UI and wait for input
            levelEndUI.SetActive(true);
            timerText.text = "Time: " + levelTime.ToString("F2") + " seconds";
        } else if (hitCollider.tag == "Guard") {
            Disable();
            FindObjectOfType<GameController>().ShowGameLoseUI();
        }
    }

    void Disable() {
        disabled = true;
        //GetComponent<Collider>().enabled = false; // Disable player collider
    }

    void Enable() {
        disabled = false;
    }

    void FixedUpdate() {
        Vector3 inputDirection = Vector3.zero;
        if (!disabled) {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }

        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;

        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        // Prevent movement along the y-axis by setting a fixed y-position

        // Perform a raycast to check for walls or obstacles in front
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Check if the ray hits an object within 1 unit
        if (Physics.Raycast(ray, out hit, 0.8f)) {
            // If the object hit is tagged as a wall, prevent forward movement
            if (hit.collider.tag == "Obstacle") {
                // Stop the forward velocity to prevent phasing through the wall
                velocity = Vector3.zero;
            }
        }

        Vector3 newPosition = rigidbody.position + velocity * Time.fixedDeltaTime;
        newPosition.y = 0.5f;  // Keep the y-axis locked to the ground level (adjust if needed)

        rigidbody.MovePosition(newPosition);
    }

    void OnDestroy() {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }

    void LoadNextLevel() {
        // turn off UI
        levelEndUI.SetActive(false);
        string currentLevelName = SceneManager.GetActiveScene().name;
        Debug.Log("Current level name: " + currentLevelName);

        // get current level
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        StartCoroutine(UnloadCurrentLoadNext(currentSceneIndex));

    }

    IEnumerator UnloadCurrentLoadNext(int current) {
        int nextSceneIndex = current + 1;
        Debug.Log("Next scene: " + nextSceneIndex);
        // Check if there is a next scene or if it was the last one
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) {
            // unload current level
            yield return SceneManager.UnloadSceneAsync(current);
            // load next level and set as active scene
            yield return SceneManager.LoadSceneAsync(nextSceneIndex, LoadSceneMode.Additive);
            Scene nextScene = SceneManager.GetSceneByBuildIndex(nextSceneIndex);
            SceneManager.SetActiveScene(nextScene);

            // Reset player position to spawn after scene is fully loaded
            if (instance != null) {
                instance.ResetToSpawn();
            }
        } else {
            // If there is no next scene
            yield return SceneManager.UnloadSceneAsync(current);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
    }

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Keep this player alive
        } else {
            Destroy(gameObject);  // Destroy the new player instance
        }
    }
}
