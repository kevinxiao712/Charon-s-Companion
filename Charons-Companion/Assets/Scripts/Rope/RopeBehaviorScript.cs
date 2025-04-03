using UnityEngine;

public class RopeBehaviorScript : MonoBehaviour
{
    public Transform startPoint;  // e.g. Player
    public Transform endPoint;    // e.g. Pull object

    // Store the original scale.x and scale.y so we only stretch along Z
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        // Make sure we have valid references
        if (startPoint == null || endPoint == null)
            return;

        // Position rope at the startPoint
        transform.position = startPoint.position;

        // Rotate it so it faces the endPoint
        transform.LookAt(endPoint.position);

        // Calculate the distance between start and end
        float distance = Vector3.Distance(startPoint.position, endPoint.position);

        transform.localScale = new Vector3(initialScale.x,initialScale.y,distance * 22);
    }
}
