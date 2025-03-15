using UnityEngine;
using System.Collections;

public class LedgeGrabber : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement pm;
    public Transform orientation;
    public Transform cam;
    public Rigidbody rb;

    [Header("Ledge Grabbing")]
    public float moveToLedgeSpeed;
    public float maxLedgeGrabDistance;

    public float minTimeonLedge;
    private float timeOnLedge;
    public bool holding;



    [Header("Ledge Detection")]
    public float ledgeDetectionLength;
    public float ledgeSphereCastRadius;
    public LayerMask whatIsLedge;

    private Transform lastLedge;
    private Transform currLedge;

    private RaycastHit ledgeHit;



    public void upodate()
    {
        ledgeDetection();
    }



    private void ledgeDetection()
    {
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit, ledgeDetectionLength,whatIsLedge);


        if (!ledgeDetected) return;

        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);

        if (ledgeHit.transform == lastLedge) return;

        if(distanceToLedge < maxLedgeGrabDistance  && !holding) EnterLedgeHold();

    }


    private void EnterLedgeHold()
    {

    }

    private void FreezeRigidBodyOnLedge()
    {

    }
}
