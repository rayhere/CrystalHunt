using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEditor;
using TMPro;
using System.Collections;
using System;
using UnityEngine.UIElements;

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

    
    public float walkSpeed = 7f;
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
    public float jumpForce = 12f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.4f;
    bool readyToJump;
    // public float jumpUpwardThreshold = 0.1f; // Adjust as needed
    [SerializeField, Tooltip("raycastLandingDistance for play landing animation")]
    public float raycastLandingDistance = 2.0f; // Adjust as needed

    [Header("Crouching")]
    public float crouchSpeed = 3.5f;
    public float crouchYScale = 0.5f;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

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


    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    public MovementState state;
    public enum MovementState
    {
        freeze,
        unlimited,
        jumping,
        falling,
        aboutlanding,
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

    // For StateHandler
    public bool freeze;
    public bool unlimited;
    public bool restricted;
    public bool jumping;
    public bool falling;
    public bool aboutlanding;
    public bool walking;
    public bool sprinting;
    public bool wallrunning;
    public bool climbing;
    public bool vaulting;
    public bool crouching;
    public bool sliding;
    public bool standingidle;
    bool keepMomentum;

    public TextMeshProUGUI text_speed;
    public TextMeshProUGUI text_mode;

    /*     [Header("Animation")]
    public Animator animator;
    public string idleAnimation = "Idle";
    public string walkAnimation = "Walk";
    public string jumpAnimation = "Jump";

    [Header("Audio")]
    public AudioSource jumpSound;
    public AudioSource walkSound; */

    // Unity Events for Input Actions
    public UnityEvent onMove;
    public UnityEvent onJump;

    
    private PlayerInput playerInput;
    private Animator anim;
    // private CharacterController controller;
    private Rigidbody rb;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    private InputAction moveAction;
    private InputAction jumpAction;
    
    private int jumpAnimationId;
    private int blendAnimationParameterID;

    // For onMove InvokeUnityEvents
    private bool inputMovement = false; 
    // DefaultInputActions
    private DefaultInputActions playerInputActions;

    void Awake()
    {
        //controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        //anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        // Assign the input actions
        //playerInputActions = playerInput.actions;
        //moveAction = playerInput.actions["Move"];
        //jumpAction = playerInput.actions["Jump"];

        jumpAnimationId = Animator.StringToHash("Jump");
        blendAnimationParameterID = Animator.StringToHash("Blend");

        //myAnim = GetComponent<Animator>(); // allow to control animations of the GameObject


        // Assigning stats to the player
        //playerStats = GameManager.Instance.playerStats; // Access through a GameManager or directly

    }

        private void Start()
    {
        //rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update() {
        if (Mouse.current.leftButton.wasPressedThisFrame){
            // Mouse clicked!
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

        MyInput();
        SpeedControl();
        StateHandler();
        TextStuff();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
        
    }    
    
    private void FixedUpdate() {
        MovePlayer();
    }

    // Input Action method to handle movement
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);
        rb.velocity = movement * moveSpeed;

        if (context.performed){
            Debug.Log("Movement! " + context.phase);
            //playerRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            inputMovement = true;
        }else{
            Debug.Log("Movement Stop! " + context.phase);
            inputMovement = false;
        }
        onMove.Invoke(); // Invoke Unity Event for movement
    }

    // Input Action method to handle jumping
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            Debug.Log("Jump! " + context.phase);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Trigger jump animation and sound
            /* animator.Play(jumpAnimation);
            jumpSound.Play(); */

            onJump.Invoke(); // Invoke Unity Event for jumping
        }
    }

    // Method to check if the player is grounded
    private bool IsGrounded()
    {
        // Implement your grounded check logic here
        return true; // Placeholder implementation
    }

    public void Submit (InputAction.CallbackContext context) {
        if (context.performed){
            Debug.Log("Submit " + context);
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKeyUp(jumpKey) && !grounded){
            Debug.Log("Jumped, but not ground");
        }
        else if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Debug.Log("Jumped, on ground");

            // Introduce a delay before initiating jump
            float jumpDelay = 0.8f;
            jumping = true;
            restricted = true;
            StartCoroutine(DelayedJump(jumpDelay));
            // jumping = true;
            // Jump();
            
            // Invoke(nameof(ResetJump), jumpCooldown);
        }
        // Don't do following codes until jumping is done
        if (jumping) return;

        // start crouch
        if (Input.GetKeyDown(crouchKey) && horizontalInput == 0 && verticalInput == 0)
        {
            crouching = true;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            crouching = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private IEnumerator DelayedJump(float delay)
    {
        yield return new WaitForSeconds(delay);

        jumping = true;
        restricted = false;
        Jump();
        
        Invoke(nameof(ResetJump), jumpCooldown);
    }


    private void StateHandler()
    {
        // Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0f;
        }

        // Mode - Unlimited
        else if (unlimited)
        {
            state = MovementState.unlimited;
            desiredMoveSpeed = 999f;
            return;
        }

        // Mode - Jumping
        else if (jumping)
        {
            state = MovementState.jumping;

            // Use Air Speed
            if (moveSpeed < airMinSpeed)
            {
                desiredMoveSpeed = airMinSpeed;
            }
            // Debug.Log("rb.velocity.y is " + rb.velocity.y + " and jumpUpwardThreshold is "+ jumpUpwardThreshold);
            // Check if the player is jumping up
            //if (rb.velocity.y > jumpUpwardThreshold)
            // Check if player is still ascending
            if (rb.velocity.y >= 0)
            {
                Debug.Log("Player is jumping up!");
                // Set isJumping to true for the animator
                myAnim.SetBool("isStandingIdle", false);
                myAnim.SetBool("isWalking", false);
                myAnim.SetBool("isRunning", false);
                myAnim.SetBool("isCrouching", false);
                myAnim.SetBool("isJumping", true);
                myAnim.SetBool("isFalling", false);
                myAnim.SetBool("isAboutLanding", false);
                myAnim.SetBool("isGrounded", false);
            }
            else if (rb.velocity.y <= 0)
            {
                // jumpingdown
                jumping = false;
                falling = true;
                state = MovementState.falling;

                myAnim.SetBool("isStandingIdle", false);
                myAnim.SetBool("isWalking", false);
                myAnim.SetBool("isRunning", false);
                myAnim.SetBool("isCrouching", false);
                myAnim.SetBool("isJumping", false);
                myAnim.SetBool("isFalling", true);
                myAnim.SetBool("isAboutLanding", false);
                myAnim.SetBool("isGrounded", false);
                // Perform raycast downward to detect ground
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Abs(rb.velocity.y) + raycastLandingDistance, whatIsGround) && !grounded)
                {
                    falling = false;
                    // Ground is detected, change to landing animation
                    myAnim.SetBool("isStandingIdle", false);
                    myAnim.SetBool("isWalking", false);
                    myAnim.SetBool("isRunning", false);
                    myAnim.SetBool("isCrouching", false);
                    myAnim.SetBool("isJumping", false);
                    myAnim.SetBool("isFalling", false);
                    myAnim.SetBool("isAboutLanding", true);
                    myAnim.SetBool("isGrounded", false);
                }
                else if (grounded)
                {
                    falling = false;
                    myAnim.SetBool("isStandingIdle", false);
                    myAnim.SetBool("isWalking", false);
                    myAnim.SetBool("isRunning", false);
                    myAnim.SetBool("isCrouching", false);
                    myAnim.SetBool("isJumping", false);
                    myAnim.SetBool("isFalling", false);
                    myAnim.SetBool("isAboutLanding", false);
                    myAnim.SetBool("isGrounded", true);

                    // Delay setting falling = false after the landing animation is played
                    Invoke("SetFallingFalse", 0.2f); // Adjust delay time as needed
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
                    // Ground is detected, change to landing animation
                    myAnim.SetBool("isStandingIdle", false);
                    myAnim.SetBool("isWalking", false);
                    myAnim.SetBool("isRunning", false);
                    myAnim.SetBool("isCrouching", false);
                    myAnim.SetBool("isJumping", false);
                    myAnim.SetBool("isFalling", false);
                    myAnim.SetBool("isAboutLanding", true);
                    myAnim.SetBool("isGrounded", false);
                }
                else if (grounded)
                {
                    myAnim.SetBool("isStandingIdle", false);
                    myAnim.SetBool("isWalking", false);
                    myAnim.SetBool("isRunning", false);
                    myAnim.SetBool("isCrouching", false);
                    myAnim.SetBool("isJumping", false);
                    myAnim.SetBool("isFalling", false);
                    myAnim.SetBool("isAboutLanding", false);
                    myAnim.SetBool("isGrounded", true);

                    // Delay setting falling = false after the landing animation is played
                    Invoke("SetFallingFalse", 0.2f); // Adjust delay time as needed
                }
        }

        // Mode - AboutLanding
        else if (aboutlanding)
        {
            state = MovementState.aboutlanding;
            //rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0f;

            // Delay setting falling = false after the landing animation is played
            if (grounded)
                Invoke("SetAboutLandingFalse", 0.2f); // Adjust delay time as needed
        }
        

        // Mode - Vaulting // Full Climbing System
        else if (vaulting)
        {
            state = MovementState.vaulting;
            desiredMoveSpeed = vaultSpeed;
        }

        // Mode - Climbing
        else if (climbing)
        {
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
        }

        // Mode - Wallrunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }

        // Mode - Sliding
        else if (sliding) 
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed; 
                keepMomentum = true;
            }
            else
                desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Crouching
        else if (crouching) 
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed; 
        }

        // Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed; 
            myAnim.SetBool("isStandingIdle", false);
            myAnim.SetBool("isWalking", false);
            myAnim.SetBool("isRunning", true);
            myAnim.SetBool("isCrouching", false);
            myAnim.SetBool("isJumping", false);
            myAnim.SetBool("isFalling", false);
            myAnim.SetBool("isAboutLanding", false);
            myAnim.SetBool("isGrounded", false);
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            
            // Get the velocity components ignoring the y-axis
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Check if either magnitude is greater than 0.1f to detect movement
            //if (rb.velocity.magnitude > 0.1f || flatVel.magnitude > 0.1f)

            // Check if either magnitude is greater than 0.1f to detect movement
            if (flatVel.magnitude > 0.1f)
            {
                // Character is moving
                myAnim.SetBool("isStandingIdle", false);
                myAnim.SetBool("isWalking", true);
                myAnim.SetBool("isRunning", false);
                myAnim.SetBool("isCrouching", false);
                myAnim.SetBool("isJumping", false);
                myAnim.SetBool("isFalling", false);
                myAnim.SetBool("isAboutLanding", false);
                myAnim.SetBool("isGrounded", false);
            } else {
                // Character is not moving
                state = MovementState.standingidle;
               
                myAnim.SetBool("isStandingIdle", true);
                myAnim.SetBool("isWalking", false);
                myAnim.SetBool("isRunning", false);
                myAnim.SetBool("isCrouching", false);
                myAnim.SetBool("isJumping", false);
                myAnim.SetBool("isFalling", false);
                myAnim.SetBool("isAboutLanding", false);
                myAnim.SetBool("isGrounded", false);
            }
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            if (moveSpeed < airMinSpeed)
                desiredMoveSpeed = airMinSpeed;

            if (rb.velocity.y < 0 && !grounded)
            {
                falling = true;
                state = MovementState.falling;
            }
            
        }

        // check if desiredMoveSpeed has changed drastically
        //if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
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

    private void SetFallingFalse()
    {
        falling = false;
        aboutlanding = true;
    }

    private void SetAboutLandingFalse()
    {
        aboutlanding = false;
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

    


    private void TextStuff()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (OnSlope())
            text_speed.SetText("Speed: " + Round(rb.velocity.magnitude, 1) + " / " + Round(moveSpeed, 1));

        else
            text_speed.SetText("Speed: " + Round(flatVel.magnitude, 1) + " / " + Round(moveSpeed, 1));

        text_mode.SetText(state.ToString());
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
        string _textMode = state.ToString();
        //Debug.Log("GetTextMode: " + _textMode);
        return _textMode;
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    private void GroundCheck() 
    {
        grounded = CheckIsGrounded();

    }
    
    // Check if the player is grounded using a sphere cast.
    private bool CheckIsGrounded()
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
        //Gizmos.color = Color.red;
        //if (_isGrounded) Gizmos.color = Color.green;
        Gizmos.color = grounded ? Color.green : Color.red;

        // Find start/end positions of sphere cast.
        Vector3 start = transform.position + Vector3.up * _groundCheckOffset;
        Vector3 end = start + Vector3.down * _groundCheckDistance;

        // Draw wire spheres.
        Gizmos.DrawWireSphere(start, _groundCheckRadius);
        Gizmos.DrawWireSphere(end, _groundCheckRadius);
    }

    // Debug Check variable
    private void checkVar()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            Debug.Log("check var ");
        }

        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Crystal")
        {
            print("Crystal Enter");
            // Crystal destory
            // Crystal collection update
            // Check if the collided object is a Crystal


            CrystalController crystalController = other.GetComponent<CrystalController>();

            

            // Store the reference for later reactivation if needed
            // For example, you can store it in a list or use a callback from ObjectPooler
            crystalController.ReturnToPool(); // Assuming ObjectPooler has this method

            // Set the CrystalController to inactive state
            //crystalController.gameObject.SetActive(false);
            // Better do it in CrystalController.cs
            
        }

        if (other.gameObject.tag == "StoneCube")
        {
            print("StoneCube Stay");
            playerStats.currentHP -= 10; // Example: Taking damage
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Crystal")
        {
            print("Crystal Stay");
        }

        if (other.gameObject.tag == "StoneCube")
        {
            print("StoneCube Stay");
            playerStats.currentHP -= 10; // Example: Taking damage
        }
        

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Crystal")
        {
            print("Crystal Exit");
        }

        if (other.gameObject.tag == "StoneCube")
        {
            print("StoneCube Stay");
            playerStats.currentHP -= 10; // Example: Taking damage
        }
    }

}
