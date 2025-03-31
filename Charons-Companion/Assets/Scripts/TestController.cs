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

    [Header("Fade-Out Settings")]
    public SkinnedMeshRenderer[] targetRenderers;
    public float fadeStartDistance = 1.5f;
    public float fadeEndDistance = 0.5f;

    private void Start()
    {
        // If you prefer to auto-grab them from children, you could do:
        // targetRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (player == null || orientation == null)
            return;

        // -- existing orientation logic
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        Vector3 camForward = transform.forward;
        camForward.y = 0f;
        camForward.Normalize();
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

        // -- existing zoom logic
        if (vcam != null)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                float currentFOV = vcam.Lens.FieldOfView;
                float newFOV = currentFOV - scrollInput * zoomSpeed;
                newFOV = Mathf.Clamp(newFOV, minFOV, maxFOV);
                vcam.Lens.FieldOfView = newFOV;
            }
        }

        // -- NEW: Fade-out logic
        if (targetRenderers != null && targetRenderers.Length > 0)
        {
            FadeTargetIfTooClose();
        }
    }

    private void FadeTargetIfTooClose()
    {
        float distanceToTarget = Vector3.Distance(transform.position, player.position);

        // If distance < fadeStartDistance, begin to fade
        if (distanceToTarget < fadeStartDistance)
        {
            // Map distance to alpha [0..1]
            float t = Mathf.InverseLerp(fadeEndDistance, fadeStartDistance, distanceToTarget);
            SetAlphaOnAllRenderers(t);
        }
        else
        {
            // Fully opaque otherwise
            SetAlphaOnAllRenderers(1f);
        }
    }

    private void SetAlphaOnAllRenderers(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);

        // Loop over each SkinnedMeshRenderer in the array
        foreach (SkinnedMeshRenderer rend in targetRenderers)
        {
            if (rend == null) continue;

            // Update alpha on each material
            foreach (Material mat in rend.materials)
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;

            }
        }
    }
}
