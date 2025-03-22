using UnityEngine;
using System.Collections;

public class DangerZone : MonoBehaviour
{
    [Header("Assign your Player in the Inspector")]
    public GameObject player;    // The Player root object
    [SerializeField] private int respawnIndex;
    [SerializeField] private float respawnDelay = 1f;   // How long to wait before respawning
    public RespawnManager respawnManager;

    [Header("Fade UI Reference")]
    [SerializeField] private Fadeout fadeUI;             // Drag in your FadeUI script from the scene/prefab

    private void Awake()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // Compare the root of the collided object to the assigned Player reference
        GameObject rootObj = other.transform.root.gameObject;
        if (rootObj == player)
        {
            // Start the death/respawn sequence
            if (respawnManager != null)
            {
                StartCoroutine(PlayerDeathAndRespawnRoutine(player));
            }
        }
    }

    private IEnumerator PlayerDeathAndRespawnRoutine(GameObject playerObj)
    {
        // 1) Disable player movement
        PlayerMovement playerMovement = playerObj.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // 2) Fade to black
        if (fadeUI != null)
        {
            yield return StartCoroutine(fadeUI.Fade(1f));  // alpha goes from current to 1.0
        }

        // 3) Optional: Wait additional delay if you want a pause while black
        yield return new WaitForSeconds(respawnDelay);

        // 4) Respawn the player
        respawnManager.RespawnPlayer(playerObj, respawnIndex);

        // 5) Fade from black
        if (fadeUI != null)
        {
            yield return StartCoroutine(fadeUI.Fade(0f));  // alpha from 1.0 back to 0
        }

        // 6) Re-enable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
}
