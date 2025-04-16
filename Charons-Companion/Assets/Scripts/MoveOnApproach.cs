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
    public string[] targetTags = { "Player", "Clone" }; 
    public float approachDistance = 5f;  // Distance at which the object begins its move


    private bool wasInRangeLastFrame = false;

    void Update()
    {
        if (!isMoving)
        {
            float nearestDistance = GetNearestTargetDistance();
            bool isInRange = nearestDistance >= 0f   
                                   && nearestDistance < approachDistance;

            if (isInRange && !wasInRangeLastFrame)
            {
                StartMovingToNextWaypoint();
            }

            wasInRangeLastFrame = isInRange;
        }
        else
        {
            MoveTowardsWaypoint();
        }
    }
    private float GetNearestTargetDistance()
    {
        float minDist = -1f;          
        foreach (string tag in targetTags)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objs)
            {
                float d = Vector3.Distance(transform.position, obj.transform.position);
                if (minDist < 0f || d < minDist)
                    minDist = d;
            }
        }
        return minDist;                         
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
        transform.position = Vector3.MoveTowards(transform.position,
                                                 target.position,
                                                 moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) <= arrivalThreshold)
        {
            isMoving = false;
            wasInRangeLastFrame = false;  
        }
    }

}
