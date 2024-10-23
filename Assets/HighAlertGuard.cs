using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighAlertGuard : Guard {

    // Ensure this method is called when the player is spotted
    protected override void HandlePlayerSpotted() {
        // Call the base class method to handle default behavior
        base.HandlePlayerSpotted();

        // Turn to face the player when spotted
        TurnToFacePlayer();
    }

    private void TurnToFacePlayer() {
        if (player != null) { // Ensure the player reference is not null
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
        }
    }
}
