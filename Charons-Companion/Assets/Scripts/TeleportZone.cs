using UnityEngine;

public class TeleportZone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform teleportTarget; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Debug.Log("Player entered teleport zone!");

            if (teleportTarget != null)
            {
                other.transform.root.position = teleportTarget.position;
                Debug.Log("Player teleported to " + teleportTarget.position);
            }
            else
            {
                Debug.LogWarning("Teleport target is not assigned!");
            }
        }
    }
}
