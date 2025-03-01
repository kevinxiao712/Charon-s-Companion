using UnityEngine;

public class SpotTrigger : MonoBehaviour
{
    public PlatformSpawner manager;  // Assign in the Inspector
    public GameObject instructionText; // Drag the instruction text here in the Inspector

    private Collider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        myCollider.isTrigger = true;

        if (instructionText != null)
        {
            instructionText.SetActive(false); // Make sure it's off by default
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            manager.SetOccupant(myCollider, other.CompareTag("Player") ? "Player" : "Clone");
            if (instructionText != null)
            {
                instructionText.SetActive(true); // Enable the text when player is inside
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            if (instructionText != null)
            {
                instructionText.SetActive(false); // Disable the text when player leaves
            }
        }
    }
}
