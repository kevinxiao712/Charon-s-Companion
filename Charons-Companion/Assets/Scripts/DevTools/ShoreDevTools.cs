using NUnit.Framework;
using UnityEngine;
using System.Collections;

public class ShoreDevTools : MonoBehaviour
{
    [SerializeField] private GameObject[] sections = new GameObject[7];
    [SerializeField] private GameObject player;
    /// <summary>
    /// Whenever a key is pressed, the player will be moved to the beginning of that section.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.transform.position = sections[0].transform.position;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            player.transform.position = sections[1].transform.position;
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            player.transform.position = sections[2].transform.position;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            player.transform.position = sections[3].transform.position;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            player.transform.position = sections[4].transform.position;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            player.transform.position = sections[5].transform.position;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            player.transform.position = sections[6].transform.position;
        }
    }
}
