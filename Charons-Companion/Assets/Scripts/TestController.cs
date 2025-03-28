using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class TestController : MonoBehaviour
{


    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;

    [Header("Camera Zoom (FOV)")]
    public CinemachineCamera vcam;
    public float zoomSpeed = 2f;
    public float minFOV = 15f;
    public float maxFOV = 70f;


    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    public void LateUpdate()
    {

        if (player == null || orientation == null)
            return;
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;
        Vector3 camForward = transform.forward;
        camForward.y = 0f;
        camForward.Normalize();
        // Pre-calculate the bitmask for each "view mode"
        int playerViewMask = LayerMask.GetMask("Default", "Player");
        int cloneViewMask = LayerMask.GetMask("Default", "Clone");

        orientation.forward = camForward;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero)
        {
            playerObj.forward = Vector3.Slerp(
                playerObj.forward,
                inputDir.normalized,
                Time.deltaTime * rotationSpeed
            );
        }
        if (vcam != null)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                Debug.Log(scrollInput);
                float currentFOV = vcam.Lens.FieldOfView;
                float newFOV = currentFOV - scrollInput * zoomSpeed;
                newFOV = Mathf.Clamp(newFOV, minFOV, maxFOV);
                vcam.Lens.FieldOfView = newFOV;
            }
        }

    }

}