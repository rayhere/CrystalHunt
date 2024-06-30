using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class WASDController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Tooltip("Player speed multiplier.")]
    private float playerSpeed = 2.0f;
    private float moveSpeed;

    [SerializeField, Tooltip("Downwards force on the player.")]
    private float gravityValue = -9.81f;
    [SerializeField, Tooltip("Rotation Speed multiplier.")]
    private float rotationSpeed = .4f;
    [SerializeField, Tooltip("Player speed multiplier.")]
    private float animationBlendDamp = .3f;
    [SerializeField, Tooltip("Input smooth damp speed.")]
    private float inputSmoothDamp = .3f;

    
    public float walkSpeed = 3f;
    public float sprintSpeed = 10f;
    public float wallrunSpeed = 8.5f; 
    public float climbSpeed = 3f;
    public float slideSpeed = 30f;
    public float vaultSpeed = 15f;
    public float airMinSpeed = 7f;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier = 1.5f;
    public float slopeIncreaseMultiplier = 2.5f;
    public float groundDrag = 4f;

    [Header("Jumping")]
    [SerializeField, Tooltip("How the player should jump.")]
    private float jumpHeight = 1.0f;
    public float jumpForce = 6f;
    public float jumpCooldown = 0.55f;
    public float airMultiplier = 0.4f;
    bool readyToJump;
    bool jumpStarted;
    // public float jumpUpwardThreshold = 0.1f; // Adjust as needed
    [SerializeField, Tooltip("raycastLandingDistance for play landing animation")]
    public float raycastLandingDistance = 2.0f; // Adjust as needed
    private bool landedOnGroundInvokeScheduled = false;

    [Header("Crouching")]
    public float crouchSpeed = 3.5f;
    public float crouchYScale = 0.5f;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    //public KeyCode crouchKey = KeyCode.Z;

    [Header("Ground Check")]
    [SerializeField, Tooltip("SphereCast Ground Check")]
    public LayerMask whatIsGround;
    public bool grounded;
    [SerializeField] private float _groundCheckOffset = 0.35f;
    [SerializeField] private float _groundCheckDistance = 0.35f;
    [SerializeField] private float _groundCheckRadius = 0.3f;
    private Vector3 _groundNormal;

    [Header("Slope Handling")]
    public float playerHeight = 2f; // For Slope Handling
    public float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    
    [Header("References")]
    public Climbing climbingScript;
    public Transform orientation;
    public Animator myAnim;
    public PlayerStatsSO playerStats; // Reference to the ScriptableObject
    // Declare a variable to hold the CapsuleCollider component
    private CapsuleCollider capsuleCollider;
    private Vector3 originalCenter;
    private float originalHeight;
    private CursorLock cursorLock;

    
    public bool isActive = true; // Flag to control whether script is active

    private bool cursorLocked = true; // cursor locked only for wasd mode while controlling character

    public float horizontalInput;
    public float verticalInput;

    private Vector3 moveDirection;

    public MovementState movementState;
    public enum MovementState
    {
        freeze,
        unlimited,
        jumping,
        falling,
        aboutlanding,
        landedonground,
        walking,
        standingidle,
        sprinting,
        wallrunning,
        climbing,
        vaulting,
        crouching,
        sliding,
        air
    }

    public PerformState performState;
    public enum PerformState
    {
        isStandingIdle,
        isWalking,
        isSprinting,
        isCrouching,
        isJumping,
        isFalling,
        isAboutLanding,
        isLandedOnGround,
        isGrounded,
        isCrouchedWalking,
        isCrouchingIdle,
        isSliding
    }

    // For StateHandler
    public bool freeze;
    public bool unlimited;
    public bool restricted;
    public bool jumping;
    public bool falling;
    public bool aboutlanding;
    public bool landedonground;
    public bool walking;
    public bool sprinting;
    public bool wallrunning;
    public bool climbing;
    public bool vaulting;
    public bool crouching;
    public bool sliding;
    public bool standingidle;
    bool keepMomentum;

    private PlayerInput playerInput;
    private Rigidbody rb;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    // InGameUI Manager Controll
    public bool pauseMenu = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;

        // Get the CapsuleCollider component from the current GameObject
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Check if capsuleCollider is not null before accessing its properties
        if (capsuleCollider != null)
        {
            // Store the original center and height
            originalCenter = capsuleCollider.center;
            originalHeight = capsuleCollider.height;
        }
    }

    private void Update() 
    {
        if (isActive)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Handle Mouse click
            }
            if (Keyboard.current.tKey.wasPressedThisFrame){
                Debug.Log("Switch to UI ActionMap!");
                playerInput.SwitchCurrentActionMap("UI");
            }
            if (Keyboard.current.yKey.wasPressedThisFrame){
                Debug.Log("Switch to Player ActionMap!");
                playerInput.SwitchCurrentActionMap("Player");
            }

            // Perform ground check
            GroundCheck();

            if (!pauseMenu)
            {
                HandleInput();
            }
            SpeedControl();
            StateHandler();
            
            // handle drag
            if (grounded)
                rb.drag = groundDrag;
            else
                rb.drag = 0;
        }
    }    
    
    private void FixedUpdate() 
    {
        if (isActive)
        {   
            MovePlayer();
        }
    }


    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        bool preformJump = false;
        float jumpDelay = 0.9f;
        // when to jump
        if(Input.GetKeyUp(jumpKey) && !grounded){
            Debug.Log("Jumped, but not ground");
        }
        else if(Input.GetKey(jumpKey) && readyToJump && grounded && !aboutlanding && !landedonground)
        {
            Debug.Log("Input.GetKey(jumpKey) && readyToJump && grounded is " + Input.GetKey(jumpKey) + readyToJump + grounded);
            Debug.Log("readyToJump is " + readyToJump);
            readyToJump = false;
            jumpStarted = false;
            Debug.Log("JumpUp, on ground" + " readyToJump is " + readyToJump);
            
            desiredMoveSpeed = 0f;

            // Set isJumping to true for the animator
            jumping = true; // movementState changed
            //SetAllAnimFalse();
            //myAnim.SetBool("isJumping", true);

            // Introduce a delay before initiating jump
            //float jumpDelay = 0.8f;
            
            restricted = true;
            preformJump = true;
            
            // jumping = true;
            // Jump();
            
            // Invoke(nameof(ResetJump), jumpCooldown);
            //yield return new WaitForSeconds(0.1f);
            //yield return null;
        }
        if (preformJump)
        {
            readyToJump = false;
            Debug.Log("preformJump");
            //StartCoroutine(DelayedJump(jumpDelay));
            //yield return new WaitForSeconds(0.1f);
            
            //yield return null;
            DelayedJump(jumpDelay);
        }


        // Don't do following codes until jumping is done
        if (jumping) return;

        
        if (Input.GetKey(crouchKey))
        {
            // start crouch
            if (!crouching)
            {
                if (horizontalInput == 0 && verticalInput == 0)
                {
                    crouching = true; // movementState changed
                    //transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                    //rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

                    //SetAllAnimFalse();
                    //myAnim.SetBool("isCrouching", true);
                    Debug.Log("StartCrouch");

                    // Check if capsuleCollider is not null before accessing its properties
                    if (capsuleCollider != null)
                    {
                        // Now can manipulate capsuleCollider.center and capsuleCollider.height

                        // Change the center of the capsule collider
                        capsuleCollider.center = new Vector3(originalCenter.x, originalCenter.y * 0.7f , originalCenter.z);

                        // Change the height of the capsule collider
                        capsuleCollider.height = originalHeight * 0.7f;
                    }
                }
                // start crouch walking
                else //if (horizontalInput != 0 || verticalInput != 0)
                {
                    crouching = true; // movementState changed

                    //SetAllAnimFalse();
                    //myAnim.SetBool("isCrouchedWalking", true);
                    Debug.Log("StartCrouchWalking");

                    // Check if capsuleCollider is not null before accessing its properties
                    if (capsuleCollider != null)
                    {
                        // Now can manipulate capsuleCollider.center and capsuleCollider.height

                        // Change the center of the capsule collider
                        capsuleCollider.center = new Vector3(originalCenter.x, originalCenter.y * 0.7f , originalCenter.z);

                        // Change the height of the capsule collider
                        capsuleCollider.height = originalHeight * 0.7f;
                    }
                }
            }
            else //if (crouching)
            {
                Debug.Log("crouching");
                // crouch walking to crouch idle
                if (horizontalInput == 0 && verticalInput == 0)
                {
                    crouching = true; // movementState changed

                    //SetAllAnimFalse();
                    //myAnim.SetBool("isCrouchingIdle", true);
                    Debug.Log("crouch walking to crouch idle");

                    // Check if capsuleCollider is not null before accessing its properties
                    if (capsuleCollider != null)
                    {
                        // Now can manipulate capsuleCollider.center and capsuleCollider.height

                        // Change the center of the capsule collider
                        capsuleCollider.center = new Vector3(originalCenter.x, originalCenter.y * 0.7f , originalCenter.z);

                        // Change the height of the capsule collider
                        capsuleCollider.height = originalHeight * 0.7f;
                    }
                }
                // crouch idle to crouch walking
                else //if ((horizontalInput != 0 || verticalInput != 0) && crouching)
                {
                    crouching = true; // movementState changed

                    //SetAllAnimFalse();
                    //myAnim.SetBool("isCrouchedWalking", true);
                    Debug.Log("crouch idle to crouch walking");

                    // Check if capsuleCollider is not null before accessing its properties
                    if (capsuleCollider != null)
                    {
                        // Now can manipulate capsuleCollider.center and capsuleCollider.height

                        // Change the center of the capsule collider
                        capsuleCollider.center = new Vector3(originalCenter.x, originalCenter.y * 0.7f , originalCenter.z);

                        // Change the height of the capsule collider
                        capsuleCollider.height = originalHeight * 0.7f;
                    }
                }
            }
            
        }


        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            crouching = false;
            //transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            Debug.Log("stop crouch");

            if (capsuleCollider != null)
            {
                capsuleCollider.center = originalCenter;
                capsuleCollider.height = originalHeight;
            }
        }
    }

    private void DelayedJump(float delay)
    {
        //readyToJump = false;
        //yield return new WaitForSeconds(delay);
        Debug.Log("DelayedJump start");
        jumping = true;
        restricted = false;
        //Jump();
        Invoke(nameof(Jump), delay);
        
        Invoke(nameof(ResetJump), delay + jumpCooldown);
    }

    private void SetAllAnimFalse()
    {
        myAnim.SetBool("isStandingIdle", false);
        myAnim.SetBool("isWalking", false);
        myAnim.SetBool("isRunning", false);
        myAnim.SetBool("isCrouching", false);
        myAnim.SetBool("isCrouchedWalking", false);
        myAnim.SetBool("isCrouchingIdle", false);
        myAnim.SetBool("isJumping", false);
        myAnim.SetBool("isFalling", false);
        myAnim.SetBool("isAboutLanding", false);
        myAnim.SetBool("isLandedOnGround", false);
    }

    private void StateHandler()
    {
        // Mode - Freeze
        if (freeze)
        {
            movementState = MovementState.freeze;
            rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0f;
        }

        // Mode - Unlimited
        else if (unlimited)
        {
            movementState = MovementState.unlimited;
            desiredMoveSpeed = 999f;
            return;
        }

        // Mode - Vaulting // Full Climbing System
        else if (vaulting)
        {
            movementState = MovementState.vaulting;
            desiredMoveSpeed = vaultSpeed;
        }

        // Mode - Climbing
        else if (climbing)
        {
            movementState = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
        }

        // Mode - Wallrunning
        else if (wallrunning)
        {
            movementState = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }

        // Mode - Sliding
        else if (sliding) 
        {
            movementState = MovementState.sliding;
            performState = PerformState.isSliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed; 
                keepMomentum = true;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }

        // Mode - Crouching
        else if (crouching) 
        {
            movementState = MovementState.crouching;
            performState = PerformState.isCrouching;
            desiredMoveSpeed = crouchSpeed; 

            //SetAllAnimFalse();
            //myAnim.SetBool("isCrouching", true);
        }

        // Mode - Jumping
        else if (jumping)
        {
            movementState = MovementState.jumping; // movementState changed
            performState = PerformState.isJumping;

            
            // Debug.Log("rb.velocity.y is " + rb.velocity.y + " and jumpUpwardThreshold is "+ jumpUpwardThreshold);
            // Check if the player is jumping up
            //if (rb.velocity.y > jumpUpwardThreshold)
            // Check if player is still ascending
            //if (rb.velocity.y == 0 && !jumpStarted)
            if (grounded && !jumpStarted)
            {
                // Set isJumping to true for the animator
                
                //SetAllAnimFalse();
                //myAnim.SetBool("isJumping", true);
            }
            //else if (rb.velocity.y > 0)
            else if (!grounded && !jumpStarted)
            {
                jumpStarted = true; 
                // Use Air Speed
                if (moveSpeed < airMinSpeed)
                {
                    desiredMoveSpeed = airMinSpeed;
                }
            }
            //else if (rb.velocity.y < 0)
            else if (!grounded && jumpStarted && rb.velocity.y < 0)
            {
                // jumpingdown
                jumping = false;
                jumpStarted = true;
                
                // Perform raycast downward to detect ground
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Abs(rb.velocity.y) + raycastLandingDistance, whatIsGround) && !grounded)
                {
                    //falling = false;
                    aboutlanding = true; // movementState changed
                    movementState = MovementState.aboutlanding;
                    performState = PerformState.isAboutLanding;
                    // Ground is detected, change to landing animation
                    //SetAllAnimFalse();
                    //myAnim.SetBool("isAboutLanding", true);
                }
                else if (grounded)
                {
                    //falling = false;
                    //aboutlanding = false;
                    landedonground = true; // movementState changed
                    movementState = MovementState.landedonground;
                    performState = PerformState.isLandedOnGround;
                    //SetAllAnimFalse();
                    //myAnim.SetBool("isLandedOnGround", true);

                    // Delay setting falling = false after the landing animation is played
                    //Invoke("SetFallingFalse", 0.2f); // Adjust delay time as needed
                    // Delay setting falling = false after the landing animation is played
                    
                    //Invoke("SetAboutLandingFalse", 0.2f); // Adjust delay time as needed
                }
                else
                {
                    // It is falling
                    falling = true; // movementState changed
                    movementState = MovementState.falling;
                    performState = PerformState.isFalling;

                    //SetAllAnimFalse();
                    //myAnim.SetBool("isFalling", true);
                }
            }
        }

        // Mode - Falling
        else if (falling)
        {
            //SetAllAnimFalse();
            //myAnim.SetBool("isFalling", true);

            // Perform raycast downward to detect ground
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastLandingDistance, whatIsGround) && !grounded)
                {
                    readyToJump = false;
                    falling = false;
                    aboutlanding = true; // movementState changed
                    movementState = MovementState.aboutlanding;
                    performState = PerformState.isAboutLanding;
                    // Ground is detected, change to landing animation
                    //SetAllAnimFalse();
                    //myAnim.SetBool("isAboutLanding", true);

                }
                else if (grounded)
                {
                    readyToJump = false;
                    falling = false;
                    landedonground = true; // movementState changed
                    movementState = MovementState.landedonground;
                    performState = PerformState.isLandedOnGround;
                    //SetAllAnimFalse();
                    //myAnim.SetBool("isLandedOnGround", true);

                    // Delay setting falling = false after the landing animation is played
                    //Invoke("SetFallingFalse", 0.2f); // Adjust delay time as needed
                }
        }

        // Mode - AboutLanding
        else if (aboutlanding)
        {
            movementState = MovementState.aboutlanding;
            //rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0f;

            
            if (grounded)
            {
                readyToJump = false;
                aboutlanding = false;
                landedonground = true; // movementState changed
                movementState = MovementState.landedonground;
                performState = PerformState.isLandedOnGround;
                //SetAllAnimFalse();
                //myAnim.SetBool("isLandedOnGround", true);

                // Delay setting falling = false after the landing animation is played
                //Invoke("SetAboutLandingFalse", 0.2f); // Adjust delay time as needed
            }
                
        }

        // Mode - LandedOnGround
        else if (landedonground)
        {
            movementState = MovementState.landedonground;
            
            //rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0f;

            //Invoke("SetLandedOnGroundFalse", 0.8f); // Adjust delay time as needed

            if (!landedOnGroundInvokeScheduled)
            {
                landedOnGroundInvokeScheduled = true;
                StartCoroutine(DelayedLandedOnGroundAction(1.0f));
            }
        }

        // Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            movementState = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed; 
            
            if (!myAnim.GetBool("isSprinting"))
            {
                sprinting = true; // movementState changed
                performState = PerformState.isSprinting;
                //SetAllAnimFalse();
                //myAnim.SetBool("isRunning", true);
            }
        }

        // Mode - Walking
        else if (grounded)
        {
            movementState = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            
            // Get the velocity components ignoring the y-axis
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Check if either magnitude is greater than 0.1f to detect movement
            //if (rb.velocity.magnitude > 0.1f || flatVel.magnitude > 0.1f)

            // Check if either magnitude is greater than 0.1f to detect movement
            if (flatVel.magnitude > 0.1f)
            {
                // Character is moving
                if (!myAnim.GetBool("isWalking"))
                {
                    walking = true; // movementState changed
                    performState = PerformState.isWalking;
                    //SetAllAnimFalse();
                    //myAnim.SetBool("isWalking", true);
                }
                
            } else {
                // Character is not moving
                movementState = MovementState.standingidle;
                if (!myAnim.GetBool("isStandingIdle"))
                {
                    standingidle = true; // movementState changed
                    performState = PerformState.isStandingIdle;
                    //SetAllAnimFalse();
                    //myAnim.SetBool("isStandingIdle", true);
                }
            }
        }

        // Mode - Air
        else
        {
            movementState = MovementState.air;

            if (moveSpeed < airMinSpeed)
                desiredMoveSpeed = airMinSpeed;

            if (rb.velocity.y < 0 && !grounded)
            {
                falling = true; // movementState changed
                movementState = MovementState.falling;
                performState = PerformState.isFalling;
                //SetAllAnimFalse();
                //myAnim.SetBool("isFalling", true);
            }
        }

        // check if desiredMoveSpeed has changed drastically
        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

        // deactivate keepMomentum
        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
    }

    private void SetLandedOnGroundFalse()
    {
        falling = false;
        aboutlanding = false;
        landedonground = false;
        readyToJump = true;
        Debug.Log("SetLandedOnGroundFalse");
        //myAnim.SetBool("isLandedOnGround", false);
    }

    private IEnumerator DelayedLandedOnGroundAction(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Perform action after delay
        SetLandedOnGroundFalse();

        // Reset flag so that this coroutine can be started again if needed
        landedOnGroundInvokeScheduled = false;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        if (restricted) return;

        if (climbingScript != null)
        if (climbingScript.exitingWall) return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            // since we turn off the gravity on slope
            // if the player is moving upwards which means its y velocity is greater than zero
            if (rb.velocity.y > 0)
            // we add a bit of downward force to keep the player constantly on the slope
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        if (!wallrunning) rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            // Calculate the angle between the player's direction and the surface normal
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            // Determine if the angle is within the acceptable slope range
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    private void GroundCheck() 
    {
        grounded = IsGrounded();

    }
    
    // Check if the player is grounded using a sphere cast.
    private bool IsGrounded()
    {
        // Start position for the sphere cast.
        Vector3 start = transform.position + Vector3.up * _groundCheckOffset;

        // Perform sphere cast.
        if (Physics.SphereCast(start, _groundCheckRadius, Vector3.down, out RaycastHit hit, _groundCheckDistance, whatIsGround))
        {
            // If the player is grounded, save the ground normal and return true.
            _groundNormal = hit.normal;
            return true;
        }
        _groundNormal = Vector3.up;
        return false;
    }

    // Draw debug spheres for ground checking.
    private void OnDrawGizmosSelected() 
    {
        // Set gizmos color.
        Gizmos.color = grounded ? Color.green : Color.red;

        // Find start/end positions of sphere cast.
        Vector3 start = transform.position + Vector3.up * _groundCheckOffset;
        Vector3 end = start + Vector3.down * _groundCheckDistance;

        // Draw wire spheres.
        Gizmos.DrawWireSphere(start, _groundCheckRadius);
        Gizmos.DrawWireSphere(end, _groundCheckRadius);
    }

    

    public string GetTextSpeed()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        string _text_speed;
        if (OnSlope())
            _text_speed = "Speed: " + Round(rb.velocity.magnitude, 1) + " / " + Round(moveSpeed, 1);
         else
            _text_speed = "Speed: " + Round(flatVel.magnitude, 1) + " / " + Round(moveSpeed, 1);

        return _text_speed;
    }

    public string GetTextMode()
    {
        string _textMode = movementState.ToString();
        //Debug.Log("GetTextMode: " + _textMode);
        return _textMode;
    }

}
