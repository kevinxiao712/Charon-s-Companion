using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Door : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Transform openTransform;  // empty object defining open position
    public Transform closedTransform;

    private Rigidbody rb;
    private bool isOpen;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;  // So it doesn't get forced around by physics
    }

    public void Activate()
    {
        isOpen = true;
        StopAllCoroutines();
        StartCoroutine(MoveDoor(openTransform.position));
    }

    public void Deactivate()
    {
        isOpen = false;
        StopAllCoroutines();
        StartCoroutine(MoveDoor(closedTransform.position));
    }

    private System.Collections.IEnumerator MoveDoor(Vector3 targetPos)
    {
        Vector3 startPos = rb.position;
        float time = 0f;
        float duration = 1f;  // adjust as needed

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);

            // Use MovePosition in FixedUpdate or mimic it properly:
            rb.MovePosition(newPos);

            yield return null;
        }

        // Ensure final position is exact:
        rb.MovePosition(targetPos);
    }
}
