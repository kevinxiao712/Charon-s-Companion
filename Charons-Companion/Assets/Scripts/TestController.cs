using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{


    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;


    public void Start()
    {
        
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
    }

}