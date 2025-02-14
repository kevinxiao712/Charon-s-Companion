using System;
using Unity.VisualScripting;
using UnityEngine;

public class tugObject : MonoBehaviour
{
    [SerializeField] Transform target;

    GameObject origin;
    bool isPulled;

    //Sets origin to start position. This is used to return the object to that point.
    void Start()
    {
        isPulled = false;
        origin = new GameObject();
        origin.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }



    //moves the object to the target Position.
    public void Pull()
    {
        Debug.Log(origin);
        if (isPulled == false)
        {
            this.transform.position = target.transform.position;
            isPulled = true;
        }
        else
        {
            this.transform.position = origin.transform.position;
            isPulled = false;
        }
    }

    private void MoveTo(Vector3 begin, Vector3 end, float speed)
    {
        this.transform.position = Vector3.MoveTowards(begin, end, speed);
    }
}
