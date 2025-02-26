using UnityEngine;

public class SpotTrigger : MonoBehaviour
{

    public PlatformSpawner manager;  // Assign in the Inspector

    private Collider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        myCollider.isTrigger = true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //private void OnTriggerEnter(Collider other)
    //{
    //    // If a Player or Clone enters, notify the manager
    //    if (other.CompareTag("Player"))
    //    {
    //        manager.SetOccupant(myCollider, "Player");
    //    }
    //    else if (other.CompareTag("Clone"))
    //    {
    //        manager.SetOccupant(myCollider, "Clone");
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    // If a Player or Clone exits, clear occupant
    //    if (other.CompareTag("Player") || other.CompareTag("Clone"))
    //    {
    //        manager.ClearOccupant(myCollider);
    //    }
    //}
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.SetOccupant(myCollider, "Player");
        }
        else if (other.CompareTag("Clone"))
        {
            manager.SetOccupant(myCollider, "Clone");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
