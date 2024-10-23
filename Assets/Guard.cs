using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour {

    public static event System.Action OnGuardHasSpottedPlayer;

    public float waitTime = .3f;
    public float speed = 5;
    public float turnSpeed = 90;
    public float timeToSpotPlayer = .5f;

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;

    protected Transform player;
    protected float playerVisibleTimer;
    protected Color originalSpotlightColor;
    protected float viewAngle;

    public Transform pathHolder;

    protected virtual void Start() {
        StartCoroutine(FindPlayerAfterDelay());
        viewAngle = spotlight.spotAngle;
        originalSpotlightColor = spotlight.color;

        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++) {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        // Select a random waypoint index as the starting point
        int randomWaypointIndex = Random.Range(0, waypoints.Length);
        transform.position = waypoints[randomWaypointIndex];

        // Start following the path from the next waypoint
        StartCoroutine(FollowPath(waypoints, (randomWaypointIndex + 1) % waypoints.Length));
    }

    protected virtual void Update() {
        if (player == null) return;

        if (CanSeePlayer()) {
            HandlePlayerSpotted();
        } else {
            playerVisibleTimer -= Time.deltaTime;
        }

        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotlight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer) {
            OnGuardHasSpottedPlayer?.Invoke();
        }
    }

    IEnumerator FindPlayerAfterDelay() {
        yield return new WaitForSeconds(0.1f);  // Short delay
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null) {
            Debug.LogError("Player not found!");
        }
    }

    protected virtual void HandlePlayerSpotted() {
        // Calculate the distance between the guard and the player as a percentage of viewDistance
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distancePercentage = distanceToPlayer / viewDistance;

        // If the player is within the closest 25 % of the view distance, reduce the time to spot
        if (distancePercentage <= 0.25f) {
            timeToSpotPlayer = 0.1f;  // Set spotting time to 0.1 seconds if very close
        } else if(distancePercentage > 0.25f && distancePercentage <= 0.75) {
            timeToSpotPlayer = 0.4f;
        } else {
            timeToSpotPlayer = 1.0f;
        }

        playerVisibleTimer += Time.deltaTime;
    
    }

    protected bool CanSeePlayer() {
        if (Vector3.Distance(transform.position, player.position) < viewDistance) {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f) {
                if (!Physics.Linecast(transform.position, player.position, viewMask)) {
                    return true;
                }
            }
        }
        return false;
    }

    protected IEnumerator FollowPath(Vector3[] waypoints, int startingWaypointIndex) {
        int targetWaypointIndex = startingWaypointIndex;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true) {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint) {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }

    protected IEnumerator TurnToFace(Vector3 lookTarget) {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05) {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    void OnDrawGizmos() {
        // Ensure that pathHolder is not null before trying to draw waypoints
        if (pathHolder == null) return;

        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach (Transform waypoint in pathHolder) {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }

        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}