using UnityEngine;
using System.Collections;
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float MoveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    public float jumpforce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;


    [Header("Keys")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode chargeJumpKey = KeyCode.LeftAlt;
   public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;



    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;          // Duration to still allow jumping after stepping off
    private float coyoteTimeCounter;


    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    [Header("Charged Jump Settings")]
    public float maxHoldTime = 2f;       
    public float maxJumpForce = 20f;
    public float horizontalBoost = 5f;
    private float holdTime = 0f;         
    private bool isCharging = false;    

    [Header("Jump Indicator")]
    public GameObject jumpIndicator;    
    public float indicatorMaxScale = 2f;




    public float fallVelocityThreshold = -10f; // threshold to consider it a "big fall"
    public float stunDuration = 1f;            // how long to stun after a big fall

    private bool isStunned = false;
    private float maxFallSpeed = 0f;
    private bool wasGrounded = true;



    [Header("Slope Hnadling")]
    public float maxSlopeAngle;
    private RaycastHit slopHit;
    private bool exitingSlope;

    Rigidbody rb;
    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        air
    }
    private void FixedUpdate()
    {
        if (!isCharging)
        {
            MovePlayer();
        }
    }
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        
    }

    public void Update()
    {

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, playerHeight * 0.5f + 0.2f, whatIsGround))
        {
            float angle = Vector3.Angle(Vector3.up, groundHit.normal);
            if (angle < maxSlopeAngle)
            {
                grounded = true;
            }
            else
            {
                grounded = false;
            }
        }
        else
        {
            grounded = false;
        }

        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.2f), Color.red);
        Debug.Log(grounded);




        if (grounded)
        {
            // Reset coyote time
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            // Decrease coyote time if not grounded
            coyoteTimeCounter -= Time.deltaTime;
        }


        if (!grounded)
        {
            // Record the worst (most negative) Y velocity while falling
            if (rb.linearVelocity.y < maxFallSpeed)
            {
                maxFallSpeed = rb.linearVelocity.y;
            }
        }
        else
        {

            if (!wasGrounded)
            {
                // Check if we exceeded the fall velocity threshold
                if (maxFallSpeed < fallVelocityThreshold)
                {
                    StartCoroutine(ApplyFallStun(stunDuration));
                }
                // Reset for next fall
                maxFallSpeed = 0f;
            }
        }
        wasGrounded = grounded;
        MyInput();
        SpeedControl();
        StateHandler();
        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }

        else
            rb.linearDamping = 0;

        chargeJump();
    }

    public void chargeJump()
    {
        if (isCharging)
        {
            holdTime += Time.deltaTime;
            holdTime = Mathf.Clamp(holdTime, 0f, maxHoldTime);

            if (jumpIndicator != null)
            {
                jumpIndicator.SetActive(true);
                float chargeRatio = holdTime / maxHoldTime;
                float scaleValue = Mathf.Lerp(1f, indicatorMaxScale, chargeRatio);
                jumpIndicator.transform.localScale = Vector3.one * scaleValue;
            }
        }
        else
        {
            if (jumpIndicator != null)
                jumpIndicator.SetActive(false);
        }
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        Debug.Log(readyToJump);

        if (Input.GetKeyDown(jumpKey) && !Input.GetKey(chargeJumpKey) && readyToJump && (grounded || coyoteTimeCounter > 0f))
        {
            readyToJump = false;
            Jump();
            coyoteTimeCounter = 0f;
            Invoke(nameof(ResetJump), jumpCooldown);

        }
        if (Input.GetKeyDown(jumpKey) && Input.GetKey(chargeJumpKey) && readyToJump && (grounded || coyoteTimeCounter > 0f))
        {
            isCharging = true;
            holdTime = 0f;
            coyoteTimeCounter = 0f;
        }


        if (Input.GetKeyUp(jumpKey) && isCharging)
        {
            PerformChargedJump();
        }
    }
    private void PerformChargedJump()
    {
        isCharging = false;


        float chargeRatio = holdTime / maxHoldTime;
        float finalJumpForce = Mathf.Lerp(jumpforce, maxJumpForce, chargeRatio);


        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);


        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");


        Vector3 jumpDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


        if (jumpDirection.sqrMagnitude < 0.01f)
        {
            jumpDirection = orientation.forward;
        }


        jumpDirection.y = 0f;
        jumpDirection.Normalize();

        float horizontalBoost = 5f; // adjust as needed
        rb.linearVelocity += jumpDirection * horizontalBoost;


        rb.AddForce(Vector3.up * finalJumpForce, ForceMode.Impulse);

        readyToJump = false;
        Invoke(nameof(ResetJump), jumpCooldown);

        if (jumpIndicator != null)
            jumpIndicator.SetActive(false);
    }

    public void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * MoveSpeed * 20f, ForceMode.Force);
            if(rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f , ForceMode.Force);
        }
        if (grounded)
            rb.AddForce(moveDirection.normalized * MoveSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * MoveSpeed * 10f * airMultiplier, ForceMode.Force);

       // rb.useGravity = !OnSlope();

    }

    private void SpeedControl()
    {

        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > MoveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * MoveSpeed;
        }
        else
        {

            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > MoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * MoveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpforce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;

    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopHit.normal).normalized;
    }

    private IEnumerator ApplyFallStun(float duration)
    {
        isStunned = true;
        // Optionally you can do some animation or effects here
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }
    private void StateHandler()
    {
        if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            MoveSpeed = sprintSpeed;
        }
        else if(grounded)
        {
            state = MovementState.walking;
            MoveSpeed= walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }
}
