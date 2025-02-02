using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed;
    public float groundDrag;
    public float jumpforce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;


    [Header("Keys")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode chargeJumpKey = KeyCode.LeftShift;


    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;



    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    [Header("Charged Jump Settings")]
    public float maxHoldTime = 2f;       
    public float maxJumpForce = 20f;     

    private float holdTime = 0f;         
    private bool isCharging = false;    

    [Header("Jump Indicator")]
    public GameObject jumpIndicator;    
    public float indicatorMaxScale = 2f; 




    Rigidbody rb;
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

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MyInput();
        SpeedControl();
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

        if (Input.GetKey(jumpKey) && !Input.GetKey(chargeJumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);

        }
        if (Input.GetKeyDown(jumpKey) && Input.GetKey(chargeJumpKey) && readyToJump && grounded)
        {
            isCharging = true;
            holdTime = 0f;
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


        rb.AddForce(transform.up * finalJumpForce, ForceMode.Impulse);


        readyToJump = false;
        Invoke(nameof(ResetJump), jumpCooldown);

        if (jumpIndicator != null)
            jumpIndicator.SetActive(false);
    }

    public void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (grounded)
            rb.AddForce(moveDirection.normalized * MoveSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * MoveSpeed * 10f * airMultiplier, ForceMode.Force);

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > MoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * MoveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpforce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

    }
}
