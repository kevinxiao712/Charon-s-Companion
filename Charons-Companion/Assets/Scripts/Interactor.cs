using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// https://www.youtube.com/watch?v=K06lVKiY-sY
//The current problem is the third-person perspective.
//The starting point of the ray is the camera, and the direction will point to the player obj,
//so objects between the camera and the player can interact.

// This code can be placed on any object (player), but the InteractorSource is the camera

interface IInteractable
{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    
    public Transform InteractorSource;
    public float InteractorRange;

    public Transform orienTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (InteractorSource == null)
        {
            Debug.LogWarning("InteractorSource has not been assigned. Please assign it in the inspector.");
        }
    }

    //void FixedUpdate()
    //{
    //    //Vector3 rayOrigin = transform.position + Vector3.up * 1.0f;
    //    Vector3 rayOrigin = transform.position;
    //    float sphereRadius = 0.5f;
    //    Vector3 direction = transform.forward;

    //    Debug.DrawRay(rayOrigin, direction * InteractorRange, Color.red, 2.0f);

    //    if (Physics.SphereCast(rayOrigin, sphereRadius, direction, out RaycastHit hitInfo, InteractorRange))
    //    {
    //        Debug.Log($"Sphere Hit: {hitInfo.collider.gameObject.name}");

    //        if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
    //        {
    //            Debug.Log("Interacting with object");
    //            interactObj.Interact();
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Hit object is not interactable!");
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("No object hit");
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3 rayOrigin = transform.position;
            Vector3 direction = orienTransform.forward;

            Debug.DrawRay(rayOrigin, direction * InteractorRange, Color.red);

            if (Physics.Raycast(rayOrigin, direction, out RaycastHit hitInfo, InteractorRange))
            {
                Debug.Log($"Hit: {hitInfo.collider.gameObject.name}");

                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    Debug.Log("Interacting with object");
                    interactObj.Interact();
                }
                else
                {
                    Debug.LogWarning("Hit object is not interactable!");
                }
            }
            else
            {
                Debug.Log("No object hit");
            }
        }
    }
}
