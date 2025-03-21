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


    [Header("Air Speeds")]
    public float walkAirSpeed = 5f;     // Max speed in air if jumped while walking
    public float sprintAirSpeed = 10f;  // Max speed in air if jumped while sprinting


    [Header("Keys")]
    public KeyCode jumpKey = KeyCode.Space;
   public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Sprinting Jump")]
    public float sprintJumpForce = 12f;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;          // Duration to still allow jumping after stepping off
    private float coyoteTimeCounter;


    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    [Header("Charged Jump Settings")]
    public float maxHoldTime = 2f;
    public float maxJumpForce = 10f;
    public float horizontalBoost = 5f;
    public float holdTime = 0f;
    public bool isCharging = false;
    public float chargeTapThreshold = 0.2f;


    [Header("Jump Indicator")]
    public GameObject jumpIndicator;    
    public float indicatorMaxScale = 2f;


    [Header("Falling")]
    public float fallMultiplier = 5f;

    public float fallVelocityThreshold = -10f; // threshold to consider it a "big fall"
    public float stunDuration = 1f;            // how long to stun after a big fall

    private bool isStunned = false;
    private float maxFallSpeed = 0f;
    private bool wasGrounded = true;



    [Header("Slope Hnadling")]
    public float maxSlopeAngle;
    private RaycastHit slopHit;
    private bool exitingSlope;
    private bool isJumping;

    Rigidbody rb;
    public MovementState state;


    private MovementState lastGroundedState = MovementState.walking;




    public bool restricted;
    public bool freeze;
    public bool unlimited;


    public enum MovementState
    {
        freeze,
        unlimited,
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
        if (!grounded && !OnSlope() && rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.down * fallMultiplier, ForceMode.Acceleration);
        }
       // Debug.Log(OnSlope());

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

     //   Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.2f), Color.red);
    //   Debug.Log(grounded);




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
 
        }
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
     //   Debug.Log(readyToJump);
        bool isMovingInput = (Mathf.Abs(horizontalInput) > 0.01f || Mathf.Abs(verticalInput) > 0.01f);



        if (Input.GetKeyDown(jumpKey) && readyToJump && (grounded || coyoteTimeCounter > 0f))
        {
            // Use up coyote time
            coyoteTimeCounter = 0f;

            if (isMovingInput)
            {
                // If the player is moving, do a normal jump immediately
                readyToJump = false;
                NormalJump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }
            else
            {
                // If standing still begin charging
                isCharging = true;
                holdTime = 0f;
            }
        }
        if (Input.GetKeyUp(jumpKey) && isCharging)
        {
            isCharging = false;

            // If the hold time is below a small threshold, treat it as a tap => normal jump
            if (holdTime < chargeTapThreshold)
            {
                // Just do a normal jump with no forward velocity
                readyToJump = false;
                NormalJump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }
            else
            {
                // Else, it's a charged jump
                float chargeRatio = holdTime / maxHoldTime;
                float finalJumpForce = Mathf.Lerp(jumpforce, maxJumpForce, chargeRatio);

                PerformChargedJump(finalJumpForce);

                readyToJump = false;
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }
    }

    private void NormalJump()
    {


        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // The vertical jump always uses jumpforce
        rb.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);

        // If sprinting, add a horizontal boost
        if (state == MovementState.sprinting)
        {
            Vector3 forwardBoost = orientation.forward;
            forwardBoost.y = 0f;
            forwardBoost.Normalize();
            rb.AddForce(forwardBoost * sprintJumpForce, ForceMode.Impulse);
            Debug.Log("Velocity after jump: " + rb.linearVelocity);

        }

        isJumping = true;
        exitingSlope = true;
        Invoke(nameof(ResetJump), jumpCooldown);
    }
    private void PerformChargedJump(float jumpPower)
    {
        isJumping = true;
        exitingSlope = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        Vector3 jumpDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (jumpDirection.sqrMagnitude < 0.01f)
        {
            jumpDirection = orientation.forward;
        }
        jumpDirection.y = 0f;
        jumpDirection.Normalize();

        // Horizontal boost
        rb.linearVelocity += jumpDirection * horizontalBoost;

        // Upward impulse
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }


    public void MovePlayer()
    {

        if (restricted) return;
        if (isCharging)
        {

            Vector3 currentVel = rb.linearVelocity;
            rb.linearVelocity = new Vector3(0f, currentVel.y, 0f);
            return;  // End MovePlayer here so no further movement forces are applied
        }

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


        // Slope movement
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * MoveSpeed * 20f, ForceMode.Force);


            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * MoveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * MoveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        // Only disable gravity if fully on slope and not in coyote time, etc.
        if (OnSlope() && grounded && !exitingSlope && !isJumping && coyoteTimeCounter <= 0f)
        {
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }


    }

    private void SpeedControl()
    {
        if (grounded && !exitingSlope)
        {
            // Normal ground clamp
            if (OnSlope())
            {
                if (rb.linearVelocity.magnitude > MoveSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * MoveSpeed;
                }
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
        else
        {
            // We're in the air
            float currentMaxAirSpeed;

            // If we jumped while sprinting, use sprintAirSpeed. Otherwise use walkAirSpeed.
            if (lastGroundedState == MovementState.sprinting)
            {
                currentMaxAirSpeed = sprintAirSpeed;
            }
            else
            {
                currentMaxAirSpeed = walkAirSpeed;
            }

            // Now clamp horizontal speed to currentMaxAirSpeed
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVel.magnitude > currentMaxAirSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * currentMaxAirSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }





    private void ResetJump()
    {
        isJumping = false;
        exitingSlope = false;
        readyToJump = true;
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
        // If freeze
        if (freeze)
        {
            state = MovementState.freeze;
            rb.linearVelocity = Vector3.zero;
            return;
        }
        // If unlimited
        else if (unlimited)
        {
            state = MovementState.unlimited;
            MoveSpeed = 50f;
            return;
        }

        // Now handle normal states
        else if (grounded)
        {
            // Are we pressing the sprint key?
            if (Input.GetKey(sprintKey))
            {
                state = MovementState.sprinting;
                MoveSpeed = sprintSpeed;

                // Remember that we were sprinting on the ground
                lastGroundedState = MovementState.sprinting;
            }
            else
            {
                state = MovementState.walking;
                MoveSpeed = walkSpeed;

                // Remember that we were walking on the ground
                lastGroundedState = MovementState.walking;
            }
        }
        else
        {
            // We're in the air
            state = MovementState.air;

            // Check what we were doing last time we were on the ground
            if (lastGroundedState == MovementState.sprinting)
            {
                MoveSpeed = sprintSpeed;
            }
            else
            {
                MoveSpeed = walkSpeed;
            }
        }
    }
}
