using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class WASDController : MonoBehaviour
{
    [Header("References")]
    public Climbing climbingScript;
    public Transform orientation;
    public Transform playerModel;
    public Animator myAnim;
    public PlayerStatsSO playerStats;
    private StaminaManager sm;
    // Declare a variable to hold the CapsuleCollider component
    private CapsuleCollider capsuleCollider;
    private Vector3 originalCenter;
    private float originalHeight;
    private CursorLock cursorLock;
    public ThirdPersonCam thirdPersonCam;

    [Header("Ground Check")]
    [SerializeField, Tooltip("SphereCast Ground Check")]
    public LayerMask whatIsGround;
    public bool grounded;
    [SerializeField] private float _groundCheckOffset = 0.35f;
    [SerializeField] private float _groundCheckDistance = 0.35f;
    [SerializeField] private float _groundCheckRadius = 0.3f;
    private Vector3 _groundNormal;

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
    public bool readyToJump;
    public bool jumpStarted;
    // public float jumpUpwardThreshold = 0.1f;
    [SerializeField, Tooltip("raycastLandingDistance for play landing animation")]
    public float raycastLandingDistance = 2.0f;
    private bool landedOnGroundInvokeScheduled = false;
    public float landingOnGroundDelay;
    public float landedOnGroundDelay = 0.5f;
    private float jumpGroundedCheckStartTime; // Time at which to start grounded check for jumping (3 seconds in the future)
    [SerializeField, Tooltip("Time delay before starting grounded check for jumping")]
    public float groundedCheckDelay = 1f; 
    //private float fallDuration;
    private float fallEndTime; // to calculate landOnGround anim
    [SerializeField, Tooltip("Minimum duration in seconds after which the player is considered falling for too long.")]
    public float fallingThreshold = 2f;
    

    [Header("Crouching")]
    public float crouchSpeed = 3.5f;
    public float crouchYScale = 0.5f;
    private float startYScale;
    bool readyToCrouch;
    private float crouchCheckStartTime;
    public float crouchCheckDelay = 0.5f;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    //public KeyCode crouchKey = KeyCode.Z;

    

    [Header("Slope Handling")]
    public float playerHeight = 2f; // For Slope Handling
    public float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    
    public bool isActive = true; // Flag to control whether script is active

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
        isMovingJump,
        isRunningJump,
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
    public bool movingJump;
    public bool runningJump;
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
        sm = GetComponent<StaminaManager>();
    }

    private void Start()
    {
        rb.freezeRotation = true;
        readyToJump = true;
        readyToCrouch = true;
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

        float jumpDelay = 0.7f;
        // when to jump
        if(Input.GetKeyUp(jumpKey) && !grounded){
            Debug.Log("Jumped, but not ground");
        }
        else if(Input.GetKey(jumpKey) && readyToJump && grounded && !aboutlanding && !landedonground && !jumping && (playerStats.currentSP >= playerStats.jumpSPCost))
        {
            Debug.Log("Input.GetKey(jumpKey) && readyToJump && grounded is " + Input.GetKey(jumpKey) + readyToJump + grounded);
            Debug.Log("readyToJump is " + readyToJump);
            readyToJump = false; // prevent jump before jumping action finish
            jumpStarted = false; // prevent jump action cancelled because grounded on start
            Debug.Log("JumpUp, on ground" + " readyToJump is " + readyToJump);
            if (sprinting)
            {
                if (rb.velocity.magnitude < 0.1f)
                {
                    // 2. Moving Jump: Describes a jump executed while the player is moving.
                    readyToJump = false;
                    restricted = false;
                    // stop jump type group
                    movingJump = false;
                    runningJump =false;
                    // stop all movement state group
                    standingidle = false;
                    walking = false;
                    sprinting = false;
                    
                    movingJump = true;
                    jumping = true;
                    sm.isJumping = true;
                    // Time at which to start grounded check for jumping (3 seconds in the future)
                    jumpGroundedCheckStartTime = Time.time + groundedCheckDelay; // Delay time for Any Jump
                    DelayedJump(0f);
                }
            }

            if (sliding)
            {
                sliding = false;
                // 2. Moving Jump: Describes a jump executed while the player is moving.
                readyToJump = false;
                restricted = false;
                // stop jump type group
                movingJump = false;
                runningJump =false;
                // stop all movement state group
                standingidle = false;
                walking = false;
                sprinting = false;
                
                movingJump = true;
                jumping = true;
                sm.isJumping = true;
                // Time at which to start grounded check for jumping (3 seconds in the future)
                jumpGroundedCheckStartTime = Time.time + groundedCheckDelay; // Delay time for Any Jump
                DelayedJump(0f);
            }
            if (crouching)
            {
                readyToCrouch = false;
                crouching = false;

                if (rb.velocity.magnitude < 0.1f)
                {
                    // 1. Stationary Jump: Describes a jump where the player is momentarily stationary before launching into the air.
                    desiredMoveSpeed = 0f;
                    restricted = true;
                    readyToJump = false;
                    // stop jump type group
                    movingJump = false;
                    runningJump =false;
                    // stop all movement state group
                    standingidle = false;
                    walking = false;
                    sprinting = false;

                    jumping = true;
                    sm.isJumping = true;
                    Debug.Log("sm.isJumping = true "+ 1);
                    // Time at which to start grounded check for jumping (3 seconds in the future)
                    jumpGroundedCheckStartTime = Time.time + groundedCheckDelay; // Delay time for Any Jump
                    DelayedJump(jumpDelay);
                }
                else 
                {
                    // 2. Moving Jump: Describes a jump executed while the player is moving.
                    readyToJump = false;
                    restricted = false;
                    // stop jump type group
                    movingJump = false;
                    runningJump =false;
                    // stop all movement state group
                    standingidle = false;
                    walking = false;
                    sprinting = false;
                    
                    movingJump = true;
                    jumping = true;
                    sm.isJumping = true;
                    Debug.Log("sm.isJumping = true "+ 2);
                    // Time at which to start grounded check for jumping (3 seconds in the future)
                    jumpGroundedCheckStartTime = Time.time + groundedCheckDelay; // Delay time for Any Jump
                    DelayedJump(0f);
                }
            }

            Debug.Log("Jumped Time.time is " + Time.time);
            if ((rb.velocity.magnitude < 0.1f) && standingidle == true)
            {
                // 1. Stationary Jump: Describes a jump where the player is momentarily stationary before launching into the air.
                desiredMoveSpeed = 0f;
                restricted = true;
                readyToJump = false;
                // stop jump type group
                movingJump = false;
                runningJump =false;
                // stop all movement state group
                standingidle = false;
                walking = false;
                sprinting = false;

                jumping = true;
                sm.isJumping = true;
                Debug.Log("sm.isJumping = true "+ 3);
                // Time at which to start grounded check for jumping (3 seconds in the future)
                jumpGroundedCheckStartTime = Time.time + groundedCheckDelay; // Delay time for Any Jump
                DelayedJump(jumpDelay);
            }
            else if ((rb.velocity.magnitude < walkSpeed) && (walking == true))
            {
                // 2. Moving Jump: Describes a jump executed while the player is moving.
                readyToJump = false;
                restricted = false;
                // stop jump type group
                movingJump = false;
                runningJump =false;
                // stop all movement state group
                standingidle = false;
                walking = false;
                sprinting = false;
                
                movingJump = true;
                jumping = true;
                sm.isJumping = true;
                Debug.Log("sm.isJumping = true "+ 4);
                // Time at which to start grounded check for jumping (3 seconds in the future)
                jumpGroundedCheckStartTime = Time.time + groundedCheckDelay; // Delay time for Any Jump
                DelayedJump(0f);
            }
            else if ((rb.velocity.magnitude >= walkSpeed) && (sprinting == true) || (walking == true))
            {
                // 3. Running Jump: Indicates a jump performed while the player is in motion.
                readyToJump = false;
                restricted = false;

                // stop jump type group
                movingJump = false;
                runningJump =false;
                // stop all movement state group
                standingidle = false;
                walking = false;
                sprinting = false;
                
                movingJump = true;
                jumping = true;
                sm.isJumping = true;
                // Time at which to start grounded check for jumping (3 seconds in the future)
                DelayedJump(0f);
            }
        }

        // Don't do following codes until jumping is done
        if (jumping) return;

        if (Input.GetKey(crouchKey) && readyToCrouch && grounded && !aboutlanding && !landedonground && !falling)
        {
            // start crouch
            if (!crouching)
            {
                if (horizontalInput == 0 && verticalInput == 0)
                {
                    if (standingidle == true)
                    {
                        standingidle = false;
                    }
                    
                    readyToCrouch = false;
                    crouching = true;
                    crouchCheckStartTime = Time.time + crouchCheckDelay;

                    Debug.Log("StartCrouch");

                    // Check if capsuleCollider is not null before accessing its properties
                    if (capsuleCollider != null)
                    {
                        // Change the center of the capsule collider
                        capsuleCollider.center = new Vector3(originalCenter.x, originalCenter.y * 0.7f , originalCenter.z);

                        // Change the height of the capsule collider
                        capsuleCollider.height = originalHeight * 0.7f;
                    }
                }
                // start crouch walking
                else
                {
                    readyToCrouch = false;
                    crouching = true;
                    crouchCheckStartTime = Time.time + crouchCheckDelay;

                    Debug.Log("StartCrouchWalking");

                    // Check if capsuleCollider is not null before accessing its properties
                    if (capsuleCollider != null)
                    {
                        // Change the center of the capsule collider
                        capsuleCollider.center = new Vector3(originalCenter.x, originalCenter.y * 0.7f , originalCenter.z);

                        // Change the height of the capsule collider
                        capsuleCollider.height = originalHeight * 0.7f;
                    }
                }
            }
            else
            {
                Debug.Log("crouching");
                // crouch walking to crouch idle
                if (horizontalInput == 0 && verticalInput == 0)
                {
                    crouching = true;
                    crouchCheckStartTime = Time.time + crouchCheckDelay;

                    Debug.Log("crouch walking to crouch idle");

                    // Check if capsuleCollider is not null before accessing its properties
                    if (capsuleCollider != null)
                    {
                        // Change the center of the capsule collider
                        capsuleCollider.center = new Vector3(originalCenter.x, originalCenter.y * 0.7f , originalCenter.z);

                        // Change the height of the capsule collider
                        capsuleCollider.height = originalHeight * 0.7f;
                    }
                }
                // crouch idle to crouch walking
                else
                {
                    crouching = true;
                    crouchCheckStartTime = Time.time + crouchCheckDelay;

                    Debug.Log("crouch idle to crouch walking");

                    // Check if capsuleCollider is not null before accessing its properties
                    if (capsuleCollider != null)
                    {
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

            Debug.Log("stop crouch");

            if (capsuleCollider != null)
            {
                capsuleCollider.center = originalCenter;
                capsuleCollider.height = originalHeight;
            }
            readyToCrouch = true;
        }
    }

    private void DelayedJump(float jumpDelay)
    {
        Debug.Log("DelayedJump start");
        jumping = true;
        sm.isJumping = true;

        StartCoroutine(Jump(jumpDelay));
        StartCoroutine(ResetJump(jumpDelay + jumpCooldown));
    }

    IEnumerator ResetJump(float delay)
    {
        yield return new WaitForSeconds (delay);
        while (true)
        {
            if (grounded && !falling && !jumping && !movingJump && !runningJump && !aboutlanding && !landedonground)
            {
                readyToJump = true;
                exitingSlope = false;
                yield break;
            }
            yield return new WaitForSeconds (0.2f);
        }
    }

    IEnumerator Jump(float jumpDelay)
    {
        yield return new WaitForSeconds(jumpDelay);
        exitingSlope = true;
        restricted = false;
        // Time at which to start grounded check for jumping (3 seconds in the future)
        jumpGroundedCheckStartTime = Time.time + groundedCheckDelay;
        if (movingJump || runningJump) jumpGroundedCheckStartTime +=2.8f;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (runningJump || movingJump)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(transform.up * 1.18f * jumpForce, ForceMode.Impulse);
        }
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

            readyToCrouch = true;
            readyToJump = true;
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

            if (!(Time.time < crouchCheckStartTime))
            {
                if (grounded)
                {
                    // Start crouch finished
                    if (horizontalInput != 0 || verticalInput != 0)
                    {
                        performState = PerformState.isCrouchedWalking;
                    }
                    else 
                    {
                        performState = PerformState.isCrouchingIdle;
                    }
                }
                else
                {
                    // is not grounded
                    crouching = false;
                    readyToCrouch = true;
                }
            }
            else
            {
                // Start crouch is not done
                if (jumping)
                {
                    crouching = false;
                    readyToCrouch = true;
                }
            }
        }

        // Mode - Jumping
        else if (jumping)
        {
            movementState = MovementState.jumping;
            
            if (grounded && !jumpStarted)
            {
                if (!(performState == PerformState.isMovingJump || performState == PerformState.isJumping))
                {
                    if (movingJump)
                    {
                        performState = PerformState.isMovingJump;
                    }
                    else if (runningJump)
                    {
                        performState = PerformState.isMovingJump;
                    }
                    else
                    {
                        performState = PerformState.isJumping; // Stationary Jump
                    }
                    crouching = false;
                    readyToCrouch = true;
                }
                
                // Sometime it will jump on mountain and always ground
                float delay = 2.8f;
                if (!(Time.time < (jumpGroundedCheckStartTime - groundedCheckDelay + delay)))
                {
                    Debug.Log("Jump but hit the ground. Cannot Perform Jump. Time.time is " + Time.time + " jumpGroundedCheckStartTime is " + jumpGroundedCheckStartTime);
                    // Even it cannont jumping up, it still count jumped
                    jumpStarted = true; 
                }
            }
            //else if (rb.velocity.y > 0)
            else if (!grounded && !jumpStarted)
            {
                jumpStarted = true; 
                // Use Air Speedw
                if (moveSpeed < airMinSpeed)
                {
                    desiredMoveSpeed = airMinSpeed;
                }
            }
            else if (grounded && jumpStarted)
            {
                if (!(Time.time < jumpGroundedCheckStartTime))
                {
                    Debug.Log("Jump but hit the ground. Ready to do Ground Check. Time.time is " + Time.time + " jumpGroundedCheckStartTime is " + jumpGroundedCheckStartTime);
                    // Even it is jumping up, it may still landed on ground
                    jumping = false;
                    movingJump = false;
                    runningJump = false;
                    standingidle = false;
                    walking = false;
                    sprinting = false;
                    jumpStarted = false; // set to false only when exist state
                    landedonground = true;
                    movementState = MovementState.landedonground;
                    performState = PerformState.isLandedOnGround;
                }
                else
                {
                    Debug.Log("Jump but hit the ground. NOT ready to do Ground Check. Time.time is " + Time.time + " jumpGroundedCheckStartTime is " + jumpGroundedCheckStartTime);
                }
            }
            else if (!grounded && jumpStarted && rb.velocity.y < 0)
            {
                // jumpingdown
                standingidle = false;
                walking = false;
                sprinting = false;
                
                // Perform raycast downward to detect ground
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Abs(rb.velocity.y) + raycastLandingDistance, whatIsGround) && !grounded)
                {
                    //falling = false;
                    jumping = false;
                    movingJump = false;
                    runningJump =false;
                    jumpStarted = false; // set to false only when exist state
                    aboutlanding = true; 
                    movementState = MovementState.aboutlanding;
                    performState = PerformState.isAboutLanding;
                }
                else
                {
                    // It is falling
                    jumping = false;
                    movingJump = false;
                    runningJump =false;
                    jumpStarted = false; // set to false only when exist state 
                    falling = true;

                    fallEndTime = Time.time + fallingThreshold; // Set the fall end time to fallingThreshold seconds after current time
                    movementState = MovementState.falling;
                    performState = PerformState.isFalling;
                }
            }
        }

        // Mode - Falling
        else if (falling)
        {
            // Perform raycast downward to detect ground
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastLandingDistance, whatIsGround) && !grounded)
            {
                falling = false;
                aboutlanding = true;
                movementState = MovementState.aboutlanding;
                performState = PerformState.isAboutLanding;
            }
            else if (grounded)
            {
                falling = false;
                landedonground = true;
                movementState = MovementState.landedonground;
                performState = PerformState.isLandedOnGround;
            }
        }

        // Mode - AboutLanding
        else if (aboutlanding)
        {
            movementState = MovementState.aboutlanding;

            // BOOST Landing
            float landingTime = .7f;
            float boostFactor = .018f; // Adjust this value to control the boost amount
            float initialBoostFactor = 0.5f; // Adjust this value to control the initial boost amount

            float boostDuration = 0.5f; // Duration over which to interpolate the boost

            float timeSinceLanding = Time.time - landingTime; // Calculate time since landing, landingTime being the time when landing detected

            // Calculate interpolation factor based on time since landing and boostDuration
            float t = Mathf.Clamp01(timeSinceLanding / boostDuration);

            // Interpolate between initialBoostFactor and boostFactor to gradually increase boost over time
            float currentBoostFactor = Mathf.Lerp(initialBoostFactor, boostFactor, t);

            // Reduce the y-component of velocity to increase downward speed upon landing
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - (10f * currentBoostFactor), rb.velocity.z);
            if (grounded)
            {
                readyToJump = false;
                aboutlanding = false;
                landedonground = true;
                movementState = MovementState.landedonground;
                performState = PerformState.isLandedOnGround;
            }
                
        }

        // Mode - LandedOnGround
        else if (landedonground)
        {
            movementState = MovementState.landedonground;
            // Check if the player has been falling for more than 2 seconds
            if ((Time.time > fallEndTime) && (fallEndTime > jumpGroundedCheckStartTime))
            {
                Debug.Log("Player has been falling for more than " + fallingThreshold + " seconds." + "  Time.time: " + Time.time + " fallEndTime: " + fallEndTime);
                desiredMoveSpeed = 0f;

                if (!landedOnGroundInvokeScheduled)
                {
                    landedOnGroundInvokeScheduled = true;
                    StartCoroutine(DelayedLandedOnGroundAction(landedOnGroundDelay));
                }
            }
            else
            {
                if (!landedOnGroundInvokeScheduled)
                {
                    landedOnGroundInvokeScheduled = true;
                    StartCoroutine(DelayedLandedOnGroundAction(0.25f));
                }
            }
        }

        // Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            movementState = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed; 
            
            if (!myAnim.GetBool("isSprinting"))
            {
                falling = false;
                jumping = false;
                movingJump = false;
                runningJump = false;
                standingidle = false;
                walking = false;
                sprinting = true;
                performState = PerformState.isSprinting;
            }
        }

        // Mode - Walking
        else if (grounded)
        {
            desiredMoveSpeed = walkSpeed;
            
            // Get the velocity components ignoring the y-axis
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Check if either magnitude is greater than 0.1f to detect movement
            if (flatVel.magnitude > 0.1f)
            {
                movementState = MovementState.walking;
                // Character is moving
                if (!myAnim.GetBool("isWalking"))
                {
                    falling = false;
                    jumping = false;
                    movingJump = false;
                    runningJump = false;
                    standingidle = false;
                    sprinting = false;
                    walking = true;
                    performState = PerformState.isWalking;
                }
            } 
            else 
            {
                // Character is not moving
                movementState = MovementState.standingidle;
                if (!myAnim.GetBool("isStandingIdle"))
                {
                    falling = false;
                    jumping = false;
                    movingJump = false;
                    runningJump = false;
                    walking = false;
                    sprinting = false;
                    standingidle = true;
                    performState = PerformState.isStandingIdle;
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
                jumping = false;
                movingJump = false;
                runningJump =false;
                walking = false;
                sprinting = false;
                readyToJump = false;
                falling = true;
                fallEndTime = Time.time + fallingThreshold; // Set the fall end time to fallingThreshold seconds after current time
                movementState = MovementState.falling;
                performState = PerformState.isFalling;
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

        if (thirdPersonCam.currentStyle == ThirdPersonCam.CameraStyle.FirstPerson)
        {
            // Calculate movement direction using playerModel's forward and right vectors
        moveDirection = playerModel.forward * verticalInput + playerModel.right * horizontalInput;
        }
        else
        {
            // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        }

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
        return _textMode;
    }
}
