using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Rope : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var pm = other.GetComponentInParent<PlayerMovement>();
        if (pm == null) return;

        // Tell the player he has latched on and hand him the rail axis
        pm.EnterRail(transform.position, transform.right.normalized);
    }

    private void OnTriggerExit(Collider other)
    {
        var pm = other.GetComponentInParent<PlayerMovement>();
        if (pm == null) return;

        pm.ExitRail();
    }
}
