using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("Door Positions")]
    public Transform openPositionTransform; // Empty GameObject that marks the 'open' position
    private Vector3 closedPosition;         

    private bool isOpen = false;

    private void Start()
    {
        // Remember the door's position when the game starts as the closed position
        closedPosition = transform.position;
    }

    public void Activate()
    {
        if (!isOpen)
        {
            isOpen = true;
            StopAllCoroutines();
            StartCoroutine(MoveDoor(openPositionTransform.position));
        }
    }

    public void Deactivate()
    {
        if (isOpen)
        {
            isOpen = false;
            StopAllCoroutines();

            StartCoroutine(MoveDoor(closedPosition));
        }
    }


    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        float time = 0f;
        Vector3 startPos = transform.position;
        float duration = 1f;  

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Smoothly interpolate between current and target
            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }
        transform.position = targetPosition;
    }
}
