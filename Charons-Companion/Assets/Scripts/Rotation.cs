using UnityEngine;
using System.Collections;


public class Rotation : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform door;              // The door's Transform
    public float rotationDuration = 1f; // How many seconds the door takes to rotate
    public float openRotationAmount = -25f; // Degrees to rotate around Y axis

    [Header("Player Settings")]
    public string playerTag = "Player"; // The tag on your Player object

    private bool isRotating = false;    // True while the door is rotating
    private bool doorIsOpen = false;    // Once door is opened, we won't rotate again

    private void OnTriggerEnter(Collider other)
    {
        // If the object is the player, the door is not open yet, and not currently rotating
        if (other.CompareTag(playerTag) && !doorIsOpen && !isRotating)
        {
            // Start the smooth open
            StartCoroutine(SmoothOpenDoor(openRotationAmount, rotationDuration));
        }
    }
    private IEnumerator SmoothOpenDoor(float angleDelta, float duration)
    {
        isRotating = true;

        // Current Y angle
        float startY = door.eulerAngles.y;
        // Target Y angle
        float endY = startY + angleDelta;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;


            float currentY = Mathf.Lerp(startY, endY, t);

            Vector3 eulers = door.eulerAngles;
            eulers.y = currentY;
            door.eulerAngles = eulers;

            yield return null;
        }

        // Snap to final angle
        Vector3 finalEulers = door.eulerAngles;
        finalEulers.y = endY;
        door.eulerAngles = finalEulers;

        isRotating = false;
        doorIsOpen = true;  // The door is now permanently open
    }
}