using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target; // The player's transform
    public Vector3 offset; // The offset distance between the player and camera
    public float smoothSpeed = 0.125f; // Adjusted smoothing speed
    private Vector3 velocity = Vector3.zero; // For SmoothDamp

    private void LateUpdate() {
        if (target == null) {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) {
                target = player.transform;  // Reassign the target if it was lost
            }
        }

        if (target != null) {
            Vector3 desiredPosition = target.position + offset;
            // SmoothDamp instead of Lerp for smoother transitions
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
