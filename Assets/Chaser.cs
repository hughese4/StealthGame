using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : Guard {
    private bool isChasing = false;

    protected override void HandlePlayerSpotted() {
        // Override to stop the player from losing on spotting and change spotlight to blue
        spotlight.color = Color.blue;

        if (!isChasing) {
            // Start chasing if we are not already
            isChasing = true;
            StopAllCoroutines();  // Stop the waypoint patrol
            StartCoroutine(ChasePlayer());
        }
    }

    protected override void Update() {
        if (player == null) return;

        // Update spotlight color based on whether the guard is chasing or not
        UpdateSpotlightColor();

        // Once the Chaser is chasing, we only stop if the player breaks line of sight (behind obstacles)
        if (isChasing && !HasLineOfSight()) {
            playerVisibleTimer -= Time.deltaTime;

            if (playerVisibleTimer <= 0) {
                // Stop chasing and return to patrol if the player breaks line of sight
                isChasing = false;
                spotlight.color = originalSpotlightColor;  // Reset spotlight color
                StartCoroutine(FollowPath(GetWaypoints(), GetNearestWaypointIndex()));
            }
        }

        // If not chasing, detect the player to start chasing
        if (!isChasing && CanSeePlayer()) {
            HandlePlayerSpotted();  // Start chasing if spotted
        }
    }

    // Check if the Chaser has line of sight on the player
    private bool HasLineOfSight() {
        if (Physics.Linecast(transform.position, player.position, viewMask)) {
            // If there's an obstacle between the chaser and player, line of sight is broken
            return false;
        }
        return true;
    }

    // Coroutine for chasing the player
    private IEnumerator ChasePlayer() {
        while (isChasing) {
            if (player != null) {
                // Move towards the player's position
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

                // Rotate to face the player
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                float targetAngle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;
                transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            }
            yield return null;
        }
    }

    // Helper function to get waypoints for returning to the patrol
    private Vector3[] GetWaypoints() {
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++) {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);  // Keep y-axis constant
        }
        return waypoints;
    }

    // Helper function to find the nearest waypoint index when returning to the patrol
    private int GetNearestWaypointIndex() {
        Vector3[] waypoints = GetWaypoints();
        int nearestIndex = 0;
        float nearestDistance = Vector3.Distance(transform.position, waypoints[0]);

        for (int i = 1; i < waypoints.Length; i++) {
            float distance = Vector3.Distance(transform.position, waypoints[i]);
            if (distance < nearestDistance) {
                nearestIndex = i;
                nearestDistance = distance;
            }
        }
        return nearestIndex;
    }

    // Fix spotlight behavior by overriding this method, so the color remains blue while chasing
    private void UpdateSpotlightColor() {
        if (isChasing) {
            spotlight.color = Color.blue;  // Keep spotlight blue while chasing
        } else {
            spotlight.color = originalSpotlightColor;  // Reset when not chasing
        }
    }
}
