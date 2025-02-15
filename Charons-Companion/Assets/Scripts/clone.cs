using Unity.Cinemachine;
using UnityEngine;

public class clone : MonoBehaviour
{
    [Header("References")]
    public Transform player;                    // The main player transform
    public PlayerMovement playerMovement;       // The main player's movement script
    public GameObject clonePrefab;             // Prefab of the clone
    public KeyCode cloneKey = KeyCode.E;       // The key to trigger clone logic

    private GameObject currentClone;           // Reference to the spawned clone
    private PlayerMovement cloneMovement;      // Reference to the clone's movement script

    // Possible states
    private bool controllingClone = false;     // Are we currently controlling the clone?
    private bool cloneExists = false;     // Does a clone exist in the world?
    [Header("Cinemachine")]
    public CinemachineCamera vcam;
    void Update()
    {
        // Check if the user pressed our clone key
        if (Input.GetKeyDown(cloneKey))
        {
            HandleCloneLogic();
        }
    }

    private void HandleCloneLogic()
    {
        // CASE 1: No clone in the world => Spawn one & control it
        if (!cloneExists && !controllingClone)
        {
            SpawnClone();
            return;
        }

        // CASE 2: A clone exists, and we're currently controlling it => Return to player, keep clone
        if (cloneExists && controllingClone)
        {
            ReturnToPlayer();
            return;
        }

        // CASE 3: A clone exists, and we're controlling the player => destroy the clone
        if (cloneExists && !controllingClone)
        {
            DestroyClone();
            return;
        }


    }

    // -------------------------------------------------------------------
    // 1) Spawn a clone, switch control to the clone
    private void SpawnClone()
    {
        // Instantiate clone at player position/rotation
        currentClone = Instantiate(clonePrefab, player.position, player.rotation);
        cloneExists = true;
        if (vcam != null)
        {
            vcam.Follow = currentClone.transform;
            vcam.LookAt = currentClone.transform;
        }
        // Get the clone's movement script
        cloneMovement = currentClone.GetComponent<PlayerMovement>();

        // Switch control: disable player's movement, enable clone's
        playerMovement.enabled = false;
        cloneMovement.enabled = true;

        controllingClone = true;
    }

    // -------------------------------------------------------------------
    // 2) Return to the player, but leave the clone in the scene
    private void ReturnToPlayer()
    {
        // Disable clone's movement
        if (cloneMovement != null)
            cloneMovement.enabled = false;

        // Enable player's movement
        playerMovement.enabled = true;
        if (vcam != null)
        {
            vcam.Follow = player;  // or player.transform
            vcam.LookAt = player;  // or player.transform
        }
        controllingClone = false;
    }

    // -------------------------------------------------------------------
    // 3) Destroy the clone, free references
    private void DestroyClone()
    {
        if (currentClone != null)
            Destroy(currentClone);

        currentClone = null;
        cloneMovement = null;
        cloneExists = false;
        controllingClone = false;

        // Ensure player is still enabled
        playerMovement.enabled = true;
    }
}
