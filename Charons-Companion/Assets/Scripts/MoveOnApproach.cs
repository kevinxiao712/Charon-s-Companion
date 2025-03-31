using UnityEngine;

public class MoveOnApproach : MonoBehaviour
{
    [Header("Waypoints & Movement")]
    public Transform[] waypoints;        // Assign in inspector
    public float moveSpeed = 3f;         // Speed of movement to the next waypoint
    public float arrivalThreshold = 0.2f;// Distance at which we consider the object to have "arrived"

    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    [Header("Approach Settings")]
    public Transform player;             // Reference to the player's transform
    public float approachDistance = 5f;  // Distance at which the object begins its move


    private bool wasInRangeLastFrame = false;

    void Update()
    {
        if (!isMoving)
        {
            // Check distance to player
            float dist = Vector3.Distance(transform.position, player.position);
            bool isInRange = dist < approachDistance;

            // If the player *just* entered the range this frame
            if (isInRange && !wasInRangeLastFrame)
            {
                // Move to the next waypoint
                StartMovingToNextWaypoint();
            }

            wasInRangeLastFrame = isInRange;
        }
        else
        {
            // We are in the process of moving
            MoveTowardsWaypoint();
        }
    }

    private void StartMovingToNextWaypoint()
    {
        // Advance to the next waypoint
        currentWaypointIndex++;
        // If you want to loop back to the first waypoint:
        if (currentWaypointIndex >= waypoints.Length)
        {
            currentWaypointIndex = 0;
        }

        isMoving = true;
    }

    private void MoveTowardsWaypoint()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        // Move toward the target
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        float distanceToWaypoint = Vector3.Distance(transform.position, target.position);
        if (distanceToWaypoint <= arrivalThreshold)
        {
            // Stop moving
            isMoving = false;
        }
    }
}
