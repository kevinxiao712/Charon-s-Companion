using Unity.Cinemachine;
using UnityEngine;

public class clone : MonoBehaviour
{
    [Header("References")]
    public Transform player;                  // The main player's Transform
    public PlayerMovement playerMovement;     // The main player's movement script
    public GameObject clonePrefab;           // Prefab for the clone
    public CinemachineCamera vcam;    // Cinemachine Virtual Camera in the scene
    public Transform playerOrientation;   // assign via Inspector
    public Transform playerModel;         

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.E;    // Press to spawn/switch control
    public KeyCode destroyKey = KeyCode.Q;

    public TestController testController;



    // Internal references
    private GameObject currentClone;         // The currently spawned clone (if any)
    private PlayerMovement cloneMovement;    // The clone's movement script (if any)
    private bool controllingClone;           // True if we're currently controlling the clone
    private bool cloneExists;                // True if a clone is currently spawned
    private int playerCullingMask;
    private int cloneCullingMask;



    void Start()
    {


    }
    void Awake()
    {
        playerCullingMask = LayerMask.GetMask("Default", "whatIsGround", "Player", "Clone");


        cloneCullingMask = LayerMask.GetMask("Default", "Clone", "Player", "whatIsGround", "CloneVisible");

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.cullingMask = playerCullingMask;
        }

    }
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            HandleToggle();
        }

        // Press Q to destroy the clone (if it exists)
        if (Input.GetKeyDown(destroyKey))
        {
            if (cloneExists)
            {
                DestroyClone();
            }
        }
    }
    private void HandleToggle()
    {
        // If no clone, spawn it and immediately control
        if (!cloneExists)
        {
            SpawnClone();
        }
        else
        {
            // We have a clone already.
            if (controllingClone)
            {
                // If we're controlling the clone, switch back to the player
                ReturnToPlayer();
            }
            else
            {
                // If we're controlling the player, switch to controlling the clone
                ControlClone();
            }
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
    private void ControlClone()
    {
        if (!cloneExists || currentClone == null) return;

        // Disable player movement, enable clone movement
        playerMovement.enabled = false;
        cloneMovement.enabled = true;

        controllingClone = true;
        if (testController != null)
        {
            Transform cloneOrientation = currentClone.transform.Find("CloneOrien");
            Transform cloneModel = currentClone.transform.Find("PlayerObj");

            testController.orientation = cloneOrientation;
            testController.player = currentClone.transform;
            testController.playerObj = cloneModel;
        }

        // Switch camera to clone
        if (vcam != null)
        {
            vcam.Follow = currentClone.transform;
            vcam.LookAt = currentClone.transform;
        }

        Camera.main.cullingMask = cloneCullingMask;
        Camera mainCamera = Camera.main;
    }


    private void SpawnClone()
    {
        currentClone = Instantiate(clonePrefab, player.position, player.rotation);
        cloneExists = true;

        cloneMovement = currentClone.GetComponent<PlayerMovement>();

        // Immediately control the clone
        playerMovement.enabled = false;
        cloneMovement.enabled = true;
        controllingClone = true;

        Transform cloneOrientation = currentClone.transform.Find("CloneOrien");
        Transform cloneModel = currentClone.transform.Find("PlayerObj");

        // Assign them to the TestController
        if (testController != null)
        {
            testController.orientation = cloneOrientation;
            testController.player = currentClone.transform;
            testController.playerObj = cloneModel;
        }

        // Switch camera to the clone
        if (vcam != null && currentClone != null)
        {
            vcam.Follow = currentClone.transform;
            vcam.LookAt = currentClone.transform;
        }
        Camera.main.cullingMask = cloneCullingMask;
        Camera mainCamera = Camera.main;
    }


    private void ReturnToPlayer()
    {
        // Disable clone movement, enable player
        if (cloneMovement != null)
            cloneMovement.enabled = false;
        playerMovement.enabled = true;

        controllingClone = false;
        if (testController != null && player != null)
        {
            testController.player = player;
            testController.playerObj = player;
        }

        testController.orientation = playerOrientation;
        testController.player = player;
        testController.playerObj = playerModel;


        // Switch camera to player
        if (vcam != null && player != null)
        {
            vcam.Follow = player;     // Or player.transform
            vcam.LookAt = player;     // Or player.transform
        }
        Camera mainCam = Camera.main;
        mainCam.cullingMask = playerCullingMask;
    }


    private void DestroyClone()
    {

        if (controllingClone)
        {
            ReturnToPlayer();
        }

        if (currentClone)
        {
            Destroy(currentClone);
        }

        // Clear references
        currentClone = null;
        cloneMovement = null;
        cloneExists = false;
        controllingClone = false;

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (vcam != null && player != null)
        {
            vcam.Follow = player;
            vcam.LookAt = player;
        }
    }

}
