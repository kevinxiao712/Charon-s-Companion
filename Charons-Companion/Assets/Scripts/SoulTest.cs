using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoulTest : MonoBehaviour, IInteractable
{
    public Dialogue dialogue;
    private bool hasTriggeredDialogue = false; 

    public void Interact()
    {
        Debug.Log("Interacting with Soul");
        if (dialogue != null)
        {
            if (!dialogue.gameObject.activeSelf)
            {
                dialogue.StartDialogue();
            }
        }
        else
        {
            Debug.LogWarning("Dialogue component is not assigned!");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggeredDialogue) 
        {
            Debug.Log("Player entered Soul trigger");
            hasTriggeredDialogue = true; 
            Interact();
        }
    }
}
