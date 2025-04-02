using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

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

    [Header("Tail Objects")]
    public GameObject tail;              // The normal tail (child of player)
    public GameObject tailPrefabObject;  // The alternate tail (child of player)

    // Internal references
    private GameObject currentClone;         // The currently spawned clone (if any)
    private PlayerMovement cloneMovement;    // The clone's movement script (if any)
    private bool controllingClone;           // True if we're currently controlling the clone
    private bool cloneExists;                // True if a clone is currently spawned
    private int playerCullingMask;
    private int cloneCullingMask;
    private CinemachineDeoccluder cinemachineCollider;

    public Transform playerCameraLookAt;  // Assign via Inspector (child of player)
    private Transform cloneCameraLookAt;  // Found at runtime in the clone prefab

    void Start()
    {
        cinemachineCollider = vcam.GetComponent<CinemachineDeoccluder>();

    }
    void Awake()
    {
        playerCullingMask = LayerMask.GetMask("Default", "whatIsGround", "Player", "Clone", "whatIsLedge");


        cloneCullingMask = LayerMask.GetMask("Default", "Clone", "Player", "whatIsGround", "CloneVisible", "whatIsLedge");
        if (tail != null) tail.SetActive(true);
        if (tailPrefabObject != null) tailPrefabObject.SetActive(false);
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
    private void LateUpdate()
    {
        // Only do this if a clone exists AND the tailPrefab is active
        if (cloneExists && currentClone != null && tailPrefabObject != null && tailPrefabObject.activeInHierarchy)
        {
            RotateTailTowardClone();
        }
    }
    private void RotateTailTowardClone()
    {
        Vector3 dir = currentClone.transform.position - tailPrefabObject.transform.position;
        dir = tailPrefabObject.transform.parent.InverseTransformDirection(dir);
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        // 2) Convert direction to angle around Y
        float angleY = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;



        Vector3 newLocalEuler = tailPrefabObject.transform.localEulerAngles;
        newLocalEuler.y = angleY;
        tailPrefabObject.transform.localEulerAngles = newLocalEuler;
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
            //vcam.Follow = currentClone.transform;
            //vcam.LookAt = currentClone.transform;
            StartCoroutine(SwitchCameraTarget(currentClone.transform));
        }

        Camera.main.cullingMask = cloneCullingMask;
        Camera mainCamera = Camera.main;
    }


    private void SpawnClone()
    {
        float behindDistance = 0.25f;
        Vector3 spawnPosition = player.position - player.forward * behindDistance;
        currentClone = Instantiate(clonePrefab, spawnPosition, player.rotation);
        cloneCameraLookAt = currentClone.transform.Find("Cameralookat");
        cloneExists = true;
        if (tail != null) tail.SetActive(false);
        if (tailPrefabObject != null) tailPrefabObject.SetActive(true);
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
            //vcam.Follow = currentClone.transform;
            //vcam.LookAt = currentClone.transform;
            StartCoroutine(SwitchCameraTarget(cloneCameraLookAt));
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
            //vcam.Follow = player;     
            //vcam.LookAt = player;    
            StartCoroutine(SwitchCameraTarget(playerCameraLookAt));

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
        if (tail != null) tail.SetActive(true);
        if (tailPrefabObject != null) tailPrefabObject.SetActive(false);


        if (playerMovement != null)
            playerMovement.enabled = true;

        if (vcam != null && player != null)
        {
            //vcam.Follow = player;
            //vcam.LookAt = player;
            StartCoroutine(SwitchCameraTarget(player));

        }
    }

    private IEnumerator SwitchCameraTarget(Transform newTarget)
    {
        if (cinemachineCollider != null)
        {
            // Disable Deoccluder temporarily
            cinemachineCollider.enabled = false;
        }

        // Set null first to avoid abrupt snapping
        vcam.Follow = null;
        vcam.LookAt = null;

        yield return new WaitForSeconds(0.1f); // Small delay before applying the new target

        vcam.Follow = newTarget;
        vcam.LookAt = newTarget;

        yield return new WaitForSeconds(0.3f); // Allow some time for the camera to reposition

        if (cinemachineCollider != null)
        {
            // Re-enable Deoccluder after transition
            cinemachineCollider.enabled = true;
        }
    }


}
