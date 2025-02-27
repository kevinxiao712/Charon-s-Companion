using System;
using Unity.VisualScripting;
using UnityEngine;

public class tugObject : MonoBehaviour
{
    [Tooltip("This is the GameObject Position the Object will go to. to change the position of the object, just change the position of this gameobject.")]
    [SerializeField] Transform target;
    [Tooltip("Area that Shadow needs to be in to pull the object.")]
    [SerializeField] Collider pullArea;
    GameObject origin;
    //Keeps track of whether the object can be pulled
    bool isPullable;
    //Keeps track of 
    bool isPulled;

    /// <summary>
    /// Sets origin to start position. This is used to return the object to that point.
    /// </summary>
    void Start()
    {
        isPullable = false;
        isPulled = false;
        origin = new GameObject();
        origin.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }

    /// <summary>
    /// The PullArea will utilize this method. This will set isPullable to true or false.
    /// </summary>
    /// <param name="value"></param>
    public void setPullable(bool value)
    {
        isPullable = value;
    }

    /// <summary>
    /// moves the object to the target Position IF isPullable is true. If the object is already at the target Position, it will return to the origin position.
    /// </summary>
    public void Pull()
    {
        Debug.Log(origin);
        if (isPullable == true)
        {
            if (isPulled == false)
            {
                this.transform.position = target.transform.position;
                isPulled = true;
            }
            //remove this if we do not want the player to move the object back.
            else
            {
                this.transform.position = origin.transform.position;
                isPulled = false;
            }
        }
    }

    /// <summary>
    /// CURRENTLY UNUSED - hopefully moves object from one position to another.
    /// </summary>
    /// <param name="begin"></param>
    /// <param name="end"></param>
    /// <param name="speed"></param>
    private void MoveTo(Vector3 begin, Vector3 end, float speed)
    {
        this.transform.position = Vector3.MoveTowards(begin, end, speed);
    }
}
