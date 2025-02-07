using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThirdPersonInteractor : MonoBehaviour
{
    // https://www.youtube.com/watch?v=THmW4YolDok

    [SerializeField]
    public Transform interactorPoint;
    [SerializeField]
    public float interactorPointRadius = 0.5f;
    [SerializeField]
    private LayerMask interactableMask;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
