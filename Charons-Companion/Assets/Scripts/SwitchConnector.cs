using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Door : MonoBehaviour
{
    [Header("Door Movement")]
    public float moveSpeed = 1f;
    public Transform openTransform;
    public Transform closedTransform;

    [Header("Audio")]
    public AudioSource doorAudioSource;
    public AudioClip moveClip;

    private Rigidbody rb;
    private bool isOpen;
    private bool isMoving = false;

    void Awake()
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



    void StartMovingDoor(Vector3 targetPos)
    {
        StopAllCoroutines();                 // cancel any previous move
        StartCoroutine(MoveDoor(targetPos)); // one place controls motion & audio
    }

    System.Collections.IEnumerator MoveDoor(Vector3 targetPos)
    {
        isMoving = true;

        if (doorAudioSource && moveClip && !doorAudioSource.isPlaying)
        {
            doorAudioSource.clip = moveClip;
            doorAudioSource.loop = true;     // seamless for any duration
            doorAudioSource.Play();
        }

        Vector3 startPos = rb.position;
        float distance = Vector3.Distance(startPos, targetPos);
        float duration = distance / Mathf.Max(moveSpeed, 0.0001f); // protect div-by-zero
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, t));
            yield return null;
        }

        rb.MovePosition(targetPos);        


        if (doorAudioSource && doorAudioSource.isPlaying)
            doorAudioSource.Stop();

        isMoving = false;
    }
}
