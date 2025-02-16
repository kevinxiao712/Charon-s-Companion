using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static Unity.VisualScripting.Metadata;

public class tugObjectManager : MonoBehaviour
{

    public KeyCode tugKey = KeyCode.F;
    private tugObject[] tugList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tugList = GetComponentsInChildren<tugObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(tugKey))
        {
            foreach (tugObject tugs in tugList)
            {
                tugs.Pull();
            }
        }
    }

    /// <summary>
    /// Pull the closest object.
    /// DISCLAIMER: THIS MECHANIC IS IN TESTING, THIS SHOULD PULL THE CLOSEST OBJECT NOT ALL OBJECTS.
    /// </summary>
    void tugThis()
    {

    }
}
