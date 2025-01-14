using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    #region values and references!

    internal Rigidbody rb;
    internal bool disableMovement;
    private CapsuleCollider playerCollider;

    [Header("values")]
    internal float currentVelocity;
    internal bool isCrouching; 
    internal bool sprintingState;
    internal bool isWalking;
    internal bool isRunning;
    private Vector3 movingDirection;
    internal Vector3 rawMovingDirection;
    

    [Header("Physics Values")]
    internal float speedIncrease = 1;
    public float walkingSpeed;
    public float sprintingSpeedIncrease;
    public float crouchSpeed;

    public float jumpForce;


    [Header("Reference Transforms")]

    public Transform feetCheck;
    public Transform headCheck;
    public Transform playerCamera;
    public PlayerLook look;


    [Header("Raycast values")]
    public float feetSize;
    public float headSize;

    internal bool isGrounded;
    internal bool isHeaded;
    internal bool canClimb;

    private RaycastHit slopeRaycast;
    public LayerMask jumpableLayers;
    public LayerMask ledgeLayers;

    [Header("Player State")]
    public PlayerStates state;
    public enum PlayerStates { idle, walking, running, onAir, crouching }

    #endregion
    
    #region Main methods

    public void Start()
    {
        playerCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        currentVelocity = walkingSpeed;

        PlayerManager.instance.localPlayer = gameObject;
    }

    private void LateUpdate()
    {
        HandleInputs();
    }

    private void Update()
    {
        HandleStates();
    }

    private void FixedUpdate()
    {
        HandleRaycasts();
        HandleMovement();
    }

    #endregion

    #region Inputs and Movement

    void HandleInputs()
    {
        // Jumping
        if (Input.GetButton("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce * Time.fixedDeltaTime, rb.linearVelocity.z);
        }

        // Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isCrouching)
            {
                if (!isHeaded) UnCrouch();
            }
            else
            {
                Crouch();
            }
            isCrouching = !isCrouching;
        }

        // Movement
        rawMovingDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // Movement direction while on ground or in the air
        if (!IsOnSlope())
            movingDirection = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
        else
            movingDirection = Vector3.ProjectOnPlane(transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical"), slopeRaycast.normal);

        // Sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isCrouching)
        {
            isRunning = true;
            Sprint(sprintingSpeedIncrease, true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && !isCrouching)
        {
            isRunning = false;
            Sprint(sprintingSpeedIncrease, false);
        }
    }

    void HandleStates()
    {
        if (!isGrounded)
        {
            state = PlayerStates.onAir;
        }
        else if (isCrouching)
        {
            state = PlayerStates.crouching;
        }
        else if (rb.linearVelocity.magnitude > 0)
        {
            if (isRunning)
                state = PlayerStates.running;
            else
            {
                state = PlayerStates.walking;
                isWalking = true;
            }
        }
        else
        {
            if (!isGrounded) return;
            isWalking = false;
            state = PlayerStates.idle;
        }
    }

    void UnCrouch()
    {
        playerCollider.height = 2;
        ChangeSpeed(walkingSpeed);
    }

    void Crouch()
    {
        playerCollider.height = 1;
        rb.AddForce(-transform.up * 5, ForceMode.Impulse);
        ChangeSpeed(crouchSpeed);
    }

    void HandleMovement()
    {
        // Allowing movement in the air by applying horizontal forces

        if(isGrounded)
        rb.linearVelocity = new Vector3(movingDirection.x * currentVelocity, rb.linearVelocity.y, movingDirection.z * currentVelocity);

        else if(!isGrounded)
        rb.linearVelocity = new Vector3(movingDirection.x * currentVelocity * 1.75f, rb.linearVelocity.y, movingDirection.z * currentVelocity * 1.75f);
    }

    #endregion

    #region Misc methods

    void Sprint(float newSpeed, bool run)
    {
        if (run)
            currentVelocity = walkingSpeed + newSpeed;
        else
            currentVelocity = walkingSpeed * speedIncrease;
    }

    void ChangeSpeed(float newSpeed)
    {
        currentVelocity = newSpeed * speedIncrease;
    }

    #endregion

    #region Raycasts, etc

    void HandleRaycasts()
    {
        isGrounded = Physics.CheckSphere(feetCheck.position, feetSize, jumpableLayers);
        isHeaded = Physics.CheckSphere(headCheck.position, headSize, jumpableLayers);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(feetCheck.position, feetSize);
        Gizmos.DrawWireSphere(headCheck.position, headSize);
    }

    bool IsOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeRaycast, transform.localScale.y))
        {
            if (slopeRaycast.normal != Vector3.up)
                return true;
            return false;
        }
        return false;
    }

    #endregion
}
