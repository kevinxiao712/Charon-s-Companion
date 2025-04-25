using UnityEngine;
[RequireComponent(typeof(Collider))]
public class Rope : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        var pm = other.GetComponentInParent<PlayerMovement>();
        if (pm == null) return;                    

        pm.EnterRail(transform.right);                   
        SnapSideways(other.transform);               
    }

    private void OnTriggerExit(Collider other)
    {
        var pm = other.GetComponentInParent<PlayerMovement>();
        if (pm == null) return;

        pm.ExitRail();
    }
    private void SnapSideways(Transform t)
    {
        Vector3 railDir = transform.right;               // world-space X axis
        Vector3 toPlayer = t.position - transform.position;

        float xAlongRail = Vector3.Dot(toPlayer, railDir);   // signed distance
        Vector3 lockedPos = transform.position + railDir * xAlongRail;

        t.position = new Vector3(
            lockedPos.x,
            t.position.y,                                // keep current height
            lockedPos.z);
    }
}