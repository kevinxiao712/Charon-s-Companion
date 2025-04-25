using UnityEngine;
using System.Collections;
public class PlayerMovement : MonoBehaviour
{

    public Animator playerAnimator;
    [Header("Movement")]
    private float MoveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    public float jumpforce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    private bool jumpKeyReleased = true;

    [Header("Air Speeds")]
    public float walkAirSpeed = 5f;     // Max speed in air if jumped while walking
    public float sprintAirSpeed = 10f;  // Max speed in air if jumped while sprinting

    [Header("Audio Clips")]
    public AudioSource audioSource;        // Reference to the AudioSource component
    public AudioClip[] outOfJumpsClips;
    public AudioClip doubleJumpClip;


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
    //   public bool isCharging = false;
    public float chargeTapThreshold = 0.2f;


    [Header("Jump Indicator")]
    public float indicatorMaxScale = 2f;


    [Header("Falling")]
    public float fallMultiplier = 5f;

    public float fallVelocityThreshold = -10f; // threshold to consider it a "big fall"
    public float stunDuration = 1f;            // how long to stun after a big fall

    private bool isStunned = false;
    private float maxFallSpeed = 0f;
    private bool wasGrounded = true;

    [Header("Jump Forces")]
    public float groundJumpForce = 10f;
    public float airJumpForce = 7f;

    [Header("Slope Hnadling")]
    public float maxSlopeAngle;
    private RaycastHit slopHit;
    private bool exitingSlope;
    private bool isJumping;
    [SerializeField] private float ropeSnapVerticalOffset = 20f;
    Rigidbody rb;
    public MovementState state;

    private MovementState lastGroundedState = MovementState.walking;
    [HideInInspector] public bool onRail = false;
    private Vector3 railDir = Vector3.forward;

    [Header("Jump Count (Double Jump)")]
    public int maxJumpCount = 2;   // Allow 2 jumps: initial jump and one mid-air jump.
    private int jumpCount = 0;

    public bool restricted;
    public bool freeze;
    public bool unlimited;


    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        sprinting,
        air,
        onRope
    }
    private void FixedUpdate()
    {

        MovePlayer();
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

        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayRandomClip(outOfJumpsClips);
        }

        if (grounded)
        {
            // Reset coyote time
            coyoteTimeCounter = coyoteTime;
            jumpCount = 0;
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


        bool isActuallyMoving = (Mathf.Abs(horizontalInput) > 0.01f || Mathf.Abs(verticalInput) > 0.01f);
        bool isWalking = (state == MovementState.walking && isActuallyMoving && grounded);
        bool isRunning = (state == MovementState.sprinting && grounded);

        // Set walk/run bools:
        playerAnimator.SetBool("isWalking", isWalking);
        playerAnimator.SetBool("isRunning", isRunning);



        if (Input.GetKeyUp(jumpKey))
        {
            jumpKeyReleased = true;
            if (jumpCount < maxJumpCount)
                readyToJump = true;
        }

        if (grounded && jumpKeyReleased)
            readyToJump = true;
        // chargeJump();
    }


    private void MyInput()
    {

        if (onRail && Input.GetKeyDown(jumpKey))
        {
            ExitRail();          // unlock
            readyToJump = true;  
                                 
        }
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        bool isMovingInput = (Mathf.Abs(horizontalInput) > 0.01f || Mathf.Abs(verticalInput) > 0.01f);

        // Check for jump input when ready to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump)
        {
            jumpKeyReleased = false;   // lock until it¡¯s released again
            readyToJump = false;
            playerAnimator.SetTrigger("Jumping");
            bool canJumpNormally = grounded || coyoteTimeCounter > 0f;
            // If on ground (or within coyote time) and still have jumps left:
            if (canJumpNormally && jumpCount < maxJumpCount)
            {
                jumpCount++;            // Count this as the first jump
                if (maxJumpCount == jumpCount)
                coyoteTimeCounter = 0f;   // Use up coyote time

                playerAnimator.SetTrigger("Jumping");
                readyToJump = false;
                NormalJump();
                Invoke(nameof(ResetJump), jumpCooldown);

            }
            // Else if already in the air (and not within coyote time) but have not used the extra jump yet
            else if (!canJumpNormally && jumpCount < maxJumpCount)
            {
                jumpCount++;  // This is the double jump.
                readyToJump = false;
                NormalJump();
                Invoke(nameof(ResetJump), jumpCooldown);
                playerAnimator.ResetTrigger("Jumping");
                playerAnimator.Play("Jump", 0, 0f);
                if (doubleJumpClip != null && audioSource != null)
                    audioSource.PlayOneShot(doubleJumpClip);
            }
        }

    }

    private void NormalJump()
    {


        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        float verticalForce = grounded ? groundJumpForce : airJumpForce;
        rb.AddForce(Vector3.up * verticalForce, ForceMode.Impulse);

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


    public void MovePlayer()
    {

        if (restricted) return;


        if (onRail)
        {

            float fwd = Input.GetAxisRaw("Vertical");
            moveDirection = railDir * fwd;

        }
        else
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        }


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

            // Check what we were doing last time we were on the gruound
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
    public void PlayRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return; // no clips assigned
        Debug.Log("meow");
        int index = Random.Range(0, clips.Length);
        audioSource.clip = clips[index];
        audioSource.Play();
    }
    private void Jump()
    {
        readyToJump = false;
        coyoteTimeCounter = 0f;
        jumpCount++;

        NormalJump();                  // vertical impulse (and sprint boost if needed)
        Invoke(nameof(ResetJump), jumpCooldown);
    }


    public void EnterRail(Vector3 direction)
    {
        onRail = true;
        railDir = direction.normalized;

        // kill sideways velocity so the player doesn¡¯t ¡°slide off¡±
        rb.linearVelocity = Vector3.Project(rb.linearVelocity, railDir);
    }

    public void ExitRail() => onRail = false;


}
