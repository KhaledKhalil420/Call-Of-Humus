using UnityEngine;
using System.Collections;

[System.Serializable]
public struct MovementValues
{
    [HideInInspector] public Vector3 movingDirection, movementDirection;
    [HideInInspector] public float currentSpeed; 
    
    public int SpeedMultiplier;

    [Header("Walking / Running")] //Ground speeds 
    public float groundSpeed;
    public float sprintSpeed;

    [Header("Sliding & Crouching")] //Sliding & Crouching
    public float crouchingSpeed;
    public float SlideForce;
    public float CrouchingHeight;

    [Header("Air & Jumping")] //Air & Jumping
    public float jumpForce;
    public float airSpeed;

    [Header("Dashing")] //Dashing
    public float DashingForce;
    public float DashTime;
    
    [Header("Speed control")] //Speed Control
    public float GroundDrag;
    public float SlidingDrag;
    public float onAirMaxSpeed;
    public float currentMaxSpeed;
    public float airAcceleration;
    public float slideJumpForce;

    [Header("Raycasts")] // Raycasts
    public float groundCheckRadius;
    public float headCheckRadius;
    
    public Transform Head, Feet;
    public LayerMask GroundLayer;
    
    [HideInInspector]
    public bool HasDash;
}

public class Movement : MonoBehaviour
{
    //References
    public Rigidbody rb;
    public AudioManager audioManager;
    public MovementValues v;
    private PlayerLook playerLook;

    //Moving States
    public MovingStates MoveState;
    public enum MovingStates { Idling, Walking, Sprinting, Sliding, OnAir, IsDashing }

    //Coyote Time Variables
    public float coyoteTimeDuration = 0.5f; // Duration for coyote jump window
    private float coyoteTimeCounter; // Counter for coyote time

    //Inputs & Conditions
    private bool isDashing;
    public bool isSliding;
    private bool isSprinting;
    private bool canJump = true, canSlideForce;
    public bool canDash;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        audioManager = AudioManager.instance;
        playerLook = GameObject.FindWithTag("CameraHolder").GetComponent<PlayerLook>();
    }

    private void Update()
    {
        ControllSpeed();
        ControllMovingStates();
        UpdateCoyoteTime(); // Update coyote time counter
    }

    private void LateUpdate()
    {
        ControllDrag();
        MyInputs();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #region Inputs

    private void MyInputs()
    {
        // Movement Direction
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        v.movingDirection = new Vector3(x, 0, z);
        v.movementDirection = transform.right * x + transform.forward * z;

        // Sprinting
        isSprinting = !Input.GetButton("Sprint");

        // Dashing
        if (Input.GetButtonDown("Dash") && canDash && !IsGrounded() && v.HasDash)
        {
            StartCoroutine(nameof(StartDashing));
        }

        // Sliding
        if (Input.GetButtonDown("Slide") && !IsHeaded())
        {
            StartSliding();
        }

        else if (!Input.GetButton("Slide") && isSliding && !IsHeaded())
        {
            StopSliding();
        }
        
        if (Input.GetButton("Slide") && IsGrounded() && canSlideForce)
        {
            rb.AddForce(v.SlideForce * transform.forward, ForceMode.VelocityChange);
            canSlideForce = false;
        }

        // Jumping with Coyote Time
        if (Input.GetButtonDown("Jump") && (IsGrounded() || coyoteTimeCounter > 0f) && canJump)
        {
            Jump(v.jumpForce);  // Regular jump
            StartCoroutine(nameof(GetReadyToJump));
            return;
        }
    }

    #endregion

    #region Sliding

    private void StartSliding()
    {
        rb.AddForce(Vector3.down * 15f, ForceMode.Impulse);
        transform.localScale = new Vector3(1, v.CrouchingHeight, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);

        isSliding = true;
    }

    private void StopSliding()
    {
        transform.localScale = new Vector3(1, 1.5f, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        isSliding = false;
        canSlideForce = true;
    }

    #endregion

    #region Coyote Time Management

    private void UpdateCoyoteTime()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTimeDuration; // Reset coyote time when grounded
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Decrease counter when in air
        }
    }

    #endregion

    #region Moving States & Speed

    private void ControllMovingStates()
    {
        if (isDashing)
        {
            MoveState = MovingStates.IsDashing;
            return;
        }

        if (IsGrounded())
        {
            if (v.movingDirection.magnitude != 0 && !isSliding)
                MoveState = isSprinting ? MovingStates.Sprinting : MovingStates.Walking;
            else
                MoveState = isSliding ? MovingStates.Sliding : MovingStates.Idling;

            canDash = true;
        }
        else if (!IsGrounded() && !IsHeaded())
        {
            MoveState = MovingStates.OnAir;
        }
    }

    private void ControllDrag()
    {
        if (IsGrounded())
            rb.linearDamping = !isSliding ? v.GroundDrag : v.SlidingDrag;
        else if (MoveState == MovingStates.OnAir)
            rb.linearDamping = 0;
    }

    private void ControllSpeed()
    {
        switch (MoveState)
        {
            case MovingStates.Walking:
                v.currentSpeed = isSprinting ? v.sprintSpeed : v.groundSpeed;
                v.currentMaxSpeed = v.onAirMaxSpeed;
                break;

            case MovingStates.Sprinting:
                v.currentSpeed = isSprinting ? v.sprintSpeed : v.groundSpeed;
                v.currentMaxSpeed = v.onAirMaxSpeed;
                break;

            case MovingStates.Sliding:
                v.currentSpeed = v.crouchingSpeed;
                v.currentMaxSpeed = v.slideJumpForce;
                break;

            case MovingStates.OnAir:
                v.currentSpeed = v.airSpeed;
                v.currentMaxSpeed -= Time.deltaTime * v.airAcceleration;
                v.currentMaxSpeed = Mathf.Clamp(v.currentMaxSpeed, v.onAirMaxSpeed, v.onAirMaxSpeed * 2);
                LimitSpeed(v.currentMaxSpeed);
                break;
        }
    }

    private void LimitSpeed(float maxSpeed)
    {
        Vector3 currentVelocity = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (currentVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = currentVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    #endregion

    #region Dashing & DoubleJumping

    private IEnumerator GetReadyToJump()
    {
        canJump = false;
        yield return new WaitForSeconds(0.1f);
        canJump = true;
    }
    
    private IEnumerator StartDashing()
    {
        canDash = false;
        isDashing = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        audioManager.PlaySound("Dash", 1, 1.25f);
        yield return new WaitForSeconds(v.DashTime);
        isDashing = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    #endregion

    #region Moving 

    private void Move()
    {
        if (MoveState == MovingStates.IsDashing)
        {
            rb.AddForce(v.DashingForce * playerLook.ForwardDirection(), ForceMode.Impulse);
            return;
        }

        if (MoveState == MovingStates.Walking || MoveState == MovingStates.Sprinting || MoveState == MovingStates.OnAir)
            rb.AddForce(v.currentSpeed * v.SpeedMultiplier * v.movementDirection.normalized, ForceMode.Force);

        if (MoveState == MovingStates.Sliding)
            rb.AddForce(v.currentSpeed * v.SpeedMultiplier * v.movementDirection.normalized, ForceMode.Force);
    }

    private void Jump(float force)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(force * transform.up, ForceMode.Impulse);

        audioManager.PlaySound("Jump"); 
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(v.Head.position, v.headCheckRadius);
        Gizmos.DrawWireSphere(v.Feet.position, v.groundCheckRadius);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(v.Feet.position, v.groundCheckRadius, v.GroundLayer);
    }

    private bool IsHeaded() 
    {
        return Physics.CheckSphere(v.Head.position, v.headCheckRadius, v.GroundLayer);
    }
}
