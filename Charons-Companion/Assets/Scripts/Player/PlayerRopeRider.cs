using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRopeRider : MonoBehaviour
{
    [Header("Keys & Speed")]
    public KeyCode detachKey = KeyCode.Space;
    public float climbSpeed = 4f;

    Rigidbody rb;
    PlayerMovement walkCtrl;                 // ↘ your existing controller
    Rope rope;                     // current rope
    RigidbodyConstraints originalConstraints;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        walkCtrl = GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (rope) return;                    // already attached
        Rope r = c.GetComponent<Rope>();
        if (r) Attach(r);
    }

    void OnTriggerExit(Collider c)
    {
        Rope r = c.GetComponent<Rope>();
        if (r && r == rope)
            Detach(false);
    }

    void FixedUpdate()
    {
        if (!rope) return;

        float v = Input.GetAxisRaw("Vertical");               // W / S
        Vector3 step = rope.SlideDir.normalized * v * climbSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + step);

        // face along rope
        if (Mathf.Abs(v) > 0.01f)
            transform.rotation = Quaternion.LookRotation(v > 0 ? rope.SlideDir : -rope.SlideDir);

        // hop off
        if (Input.GetKey(detachKey))
            Detach(true);
    }

    void Attach(Rope r)
    {
        rope = r;

        // 1 每 pause walking script
        walkCtrl.enabled = false;

        // 2 每 store & replace Rigidbody constraints
        originalConstraints = rb.constraints;
        rb.constraints = FreezeExcept(rope.AxisIndex);

        // 3 每 zero velocity & turn off gravity
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;

        // 4 每 snap onto surface (very light projection)
        Vector3 snap = ProjectOnRopeSurface(rb.position, r);
        rb.MovePosition(snap);
    }

    void Detach(bool withJump)
    {
        if (!rope) return;

        // restore everything
        rb.constraints = originalConstraints;
        rb.useGravity = true;
        walkCtrl.enabled = true;
        rope = null;

        if (withJump)
            rb.AddForce(Vector3.up * walkCtrl.jumpforce, ForceMode.Impulse);
    }

    static RigidbodyConstraints FreezeExcept(int axisIdx)
    {
        var c = RigidbodyConstraints.FreezeRotation;
        if (axisIdx != 0) c |= RigidbodyConstraints.FreezePositionX;
        if (axisIdx != 1) c |= RigidbodyConstraints.FreezePositionY;
        if (axisIdx != 2) c |= RigidbodyConstraints.FreezePositionZ;
        return c;
    }


    static Vector3 ProjectOnRopeSurface(Vector3 world, Rope r, float skin = 0.05f)
    {
        Vector3 local = r.transform.InverseTransformPoint(world);
        Vector3 half = r.GetComponent<Collider>().bounds.extents;

        // clamp along rope
        int idx = r.AxisIndex;
        local[idx] = Mathf.Clamp(local[idx], -half[idx], half[idx]);

        // snap to nearest face on the other two axes
        for (int i = 0; i < 3; i++)
            if (i != idx)
                local[i] = Mathf.Sign(local[i]) * (half[i] + skin);

        return r.transform.TransformPoint(local);
    }
}
