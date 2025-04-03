using UnityEngine;

public class OpenDoorScript : MonoBehaviour
{
    //[SerializeField] Animation thing;
    [SerializeField] GameObject door;
    [SerializeField] GameObject openPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //thing.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        door.transform.position = openPosition.transform.position;
    }

}
