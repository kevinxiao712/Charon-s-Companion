using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonController : MonoBehaviour
{
    public int index;
    [SerializeField] private int maxIndex;
    public bool keyDown;
    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            if (!keyDown)
            {
                if (Input.GetAxis("Vertical") < 0)
                {
                    index++;
                    if (index > maxIndex)
                    {
                        index = 0;
                    }
                }
                else if (Input.GetAxis("Vertical") > 0)
                {
                    index--;
                    if (index < 0)
                    {
                        index = maxIndex;
                    }
                }

                keyDown = true;
            }
        }
        else
        {
            keyDown = false;
        }
    }
}
