using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighAlertGuard : Guard {

    protected override void Start() {
        base.Start();
    }

    protected override void HandlePlayerSpotted() {
        base.HandlePlayerSpotted();

        // Turn to face the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
    }
}
