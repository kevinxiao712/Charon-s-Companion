using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class thisDoor : MonoBehaviour
{
    [Header("Door Movement")]
    public float moveSpeed = 1f;
    public Transform openTransform;
    public Transform closedTransform;
    public float duration = 1.0f;

    [Header("Audio")]
    public AudioSource doorAudioSource;
    public AudioClip moveClip;

    private Rigidbody rb;
    private bool isOpen;
    private bool isMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void Activate()
    {
        if (!isMoving && !isOpen)
        {
            isOpen = true;
            StartMovingDoor(openTransform.position);
        }
    }

    public void Deactivate()
    {
        if (!isMoving && isOpen)
        {
            isOpen = false;
            StartMovingDoor(closedTransform.position);
        }
    }

    private void StartMovingDoor(Vector3 targetPos)
    {
        StopAllCoroutines();
        PlayDoorMoveSound();  
        StartCoroutine(MoveDoor(targetPos));
    }

    private System.Collections.IEnumerator MoveDoor(Vector3 targetPos)
    {
        isMoving = true;

        Vector3 startPos = rb.position;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            rb.MovePosition(newPos);
            yield return null;
        }

        rb.MovePosition(targetPos);
        isMoving = false;
    }

    private void PlayDoorMoveSound()
    {
        if (doorAudioSource != null && moveClip != null && !doorAudioSource.isPlaying)
        {
            doorAudioSource.PlayOneShot(moveClip);
        }
    }
}
