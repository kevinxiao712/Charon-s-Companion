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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (InteractorSource == null)
        {
            Debug.LogWarning("InteractorSource has not been assigned. Please assign it in the inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
            Debug.DrawLine(InteractorSource.position, InteractorSource.forward,Color.white);
            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractorRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }
    }
}
