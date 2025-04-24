using UnityEngine;
using System.Collections;

public class PullTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject ropePrefab;  // Rope prefab w/ rope controller script
    public Transform pullObject;   // Object to be pulled
    public Transform pullTarget;   // Destination for the pullObject
    public string playerTag = "Player";

    [Header("Pull Settings")]
    public KeyCode pullKey = KeyCode.F;
    public float pullDuration = 1.5f;     // How long the pull takes (seconds)

    private GameObject currentRope;
    private Transform currentPlayer;
    private bool playerInside = false;
    private bool isPulling = false;
    public AudioSource audioSource;
    public AudioClip pullSound;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = true;
            currentPlayer = other.transform;

            // Spawn rope if we have a ropePrefab
            if (ropePrefab != null)
            {
                currentRope = Instantiate(ropePrefab);

                // Assign references on the rope's controller script
                RopeBehaviorScript ropeCtrl = currentRope.GetComponent<RopeBehaviorScript>();
                if (ropeCtrl != null)
                {
                    ropeCtrl.startPoint = currentPlayer; // The player
                    ropeCtrl.endPoint = pullObject;    // The object to pull
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            currentPlayer = null;

            // Destroy rope if it was spawned
            if (currentRope != null)
            {
                Destroy(currentRope);
                currentRope = null;
            }
        }
    }

    private void Update()
    {
        if (playerInside && !isPulling && Input.GetKeyDown(pullKey))
        {
            if (pullObject != null && pullTarget != null)
            {


                StartCoroutine(SmoothPullRoutine(pullDuration));
            }
        }
    }

    private IEnumerator SmoothPullRoutine(float duration)
    {
        isPulling = true;

        if (audioSource != null && pullSound != null)
        {
            audioSource.clip = pullSound;
            audioSource.loop = true;   // keep it seamlessly looping
            audioSource.Play();
        }

        Vector3 startPos = pullObject.position;
        Vector3 endPos = pullTarget.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            pullObject.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;   // wait one frame
        }

        pullObject.position = endPos;


        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        isPulling = false;
    }
}
