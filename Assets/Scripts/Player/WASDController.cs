using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEditor;
using TMPro;
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

    [Header("Crouching")]
    public float crouchSpeed = 3.5f;
    public float crouchYScale = 0.5f;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Ground Check2")]
    // Parameters for ground check
    public float groundCheckDistance = 0.5f; // The distance to check for ground
    public int numRaycasts = 5; // Number of raycasts to cast
    public float slopeThreshold = 30f; // The maximum slope angle that is considered as walkable
    // Ground check variables
    //bool grounded;
    bool onSteepGround;

    [Header("Ground Check3")]
    [SerializeField] private float _groundCheckOffset = 0.35f;
    [SerializeField] private float _groundCheckDistance = 0.35f;
    [SerializeField] private float _groundCheckRadius = 0.3f;
    private Vector3 _groundNormal;
    
    [Header("References")]
    public Climbing climbingScript;
    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    public MovementState state;
    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        sprinting,
        wallrunning,
        climbing,
        vaulting,
        crouching,
        sliding,
        air
    }

    public bool freeze;
    public bool unlimited;
    public bool restricted;

    public bool walking;
    public bool sprinting;
    public bool wallrunning;
    public bool climbing;
    public bool vaulting;
    public bool crouching;
    public bool sliding;
    bool keepMomentum;

    public TextMeshProUGUI text_speed;
    public TextMeshProUGUI text_mode;









    // Define a delegate type
    public delegate void UpdateMethod();
    [Header("Movement Method")]
    // Serialize the delegate field to choose the update method
    [SerializeField]
    private UpdateMethod selectedUpdateMethod;

    // Enum to define different update methods
    public enum SelectedMethod
    {
        Force,
        ForceMovement,
        VelocityMovement,
        Impulse,
        TorqueRotation,
        CharacterControllerMovement,
        TranslateMovement
    }

    public SelectedMethod selectedMethod;
    public CharacterController controller;

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

        // Assign the selected update method based on the selectedMethod enum value
        switch (selectedMethod)
        {
            case SelectedMethod.Force:
                selectedUpdateMethod = Force;
                break;
            case SelectedMethod.ForceMovement:
                selectedUpdateMethod = ForceMovement;
                break;
            case SelectedMethod.VelocityMovement:
                selectedUpdateMethod = VelocityMovement;
                break;
            case SelectedMethod.Impulse:
                selectedUpdateMethod = Impulse;
                break;
            case SelectedMethod.TorqueRotation:
                selectedUpdateMethod = TorqueRotation;
                break;
            case SelectedMethod.CharacterControllerMovement:
                selectedUpdateMethod = CharacterControllerMovement;
                break;
            case SelectedMethod.TranslateMovement:
                selectedUpdateMethod = TranslateMovement;
                break;
            default:
                // Handle default case or throw an error
                break;
        }
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
        /* if (Gamepad.current.aButton.wasPressedThisFrame){
            playerInput.SwitchCurrentActionMap("UI");
        } */

        // Perform ground check
        GroundCheck();
        // ground check
        //grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

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
        if (inputMovement && selectedUpdateMethod != null)
        {
            Debug.Log("selectedUpdateMethod! ");
            // Call the selected update method
            selectedUpdateMethod();
        }else if (inputMovement){
            Debug.Log("selectedUpdateMethod! and selectedUpdateMethod is " + selectedUpdateMethod);
            // Call the selected update method
            selectedUpdateMethod();
        }

        MovePlayer();
    }
    // Input Action method to handle movement
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);
        rb.velocity = movement * moveSpeed;

        // Trigger movement animation or sound
/*         if (movementInput.magnitude > 0)
        {
            animator.Play(walkAnimation);
            if (!walkSound.isPlaying)
                walkSound.Play();
        }
        else
        {
            animator.Play(idleAnimation);
            walkSound.Stop();
        } */

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

    // Method for each update technique
    public void Force (){
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        Debug.Log(inputVector); // Show input value
        float speed = 5f;
        Debug.Log("I am Moving!");

        //This movement will 
        rb.AddForce(new Vector3(inputVector.x, 0, inputVector.y) * speed, ForceMode.Force);
    }
    public void ForceMovement (){
        // This will keep checking if the button is triggered every frame
        //Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        Vector2 inputVector = playerInput.actions.FindAction("Player/Move").ReadValue<Vector2>();
        Debug.Log(inputVector); // Show input value
        float speed = 5f;
        Debug.Log("I am Moving!");

        // Calculate movement direction based on input
        //Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        Vector3 movement = new Vector3(inputVector.x, 0f, inputVector.y);
        // Apply movement to the Rigidbody
        rb.AddForce(movement * speed);
        //playerRigidbody.AddForce(movement * speed, ForceMode.Force);
    }

    public void VelocityMovement (){
        // This will keep checking if the button is triggered every frame
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        Debug.Log(inputVector); // Show input value
        float speed = 5f;
        Debug.Log("I am Moving!");

        // Calculate movement direction based on input
        //Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        Vector3 movement = new Vector3(inputVector.x, 0f, inputVector.y);
        //Vector3 movement = new Vector3(inputVector.x, 0f, inputVector.y).normalized * speed * Time.deltaTime;
        //playerRigidbody.AddForce(new Vector3(inputVector.x, 0, inputVector.y) * speed, ForceMode.Force);
        // Apply movement to the Rigidbody
        rb.velocity = movement * speed;
        //playerRigidbody.velocity = movement.normalized * speed * Time.deltaTime;
    }

    public void Impulse (){
        // This will keep checking if the button is triggered every frame
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        Debug.Log(inputVector); // Show input value
        float speed = 5f;
        Debug.Log("I am Moving!");

        // Calculate movement direction based on input
        //Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        Vector3 movement = new Vector3(inputVector.x, 0f, inputVector.y);
        //Vector3 movement = new Vector3(inputVector.x, 0f, inputVector.y).normalized * speed * Time.deltaTime;
        // Apply movement to the Rigidbody
        rb.AddForce(movement * speed, ForceMode.Impulse);
        //playerRigidbody.AddForce(movement.normalized * speed, ForceMode.Impulse * Time.deltaTime);
    }

    public void TorqueRotation (){
        // This will keep checking if the button is triggered every frame
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        Debug.Log(inputVector); // Show input value
        float torque = 10f;
        Debug.Log("I am Moving!");

        // Calculate movement direction based on input
        //float rotate = Input.GetAxis("Horizontal");
        Vector2 movementInput = playerInputActions.Player.Move.ReadValue<Vector2>();
        float moveHorizontal = movementInput.x;
        float moveVertical = movementInput.y;
        
        // Horizontal
        float rotate = moveHorizontal;
        rb.AddTorque(Vector3.up * rotate * torque);

        // Vertical
    }

    public void CharacterControllerMovement (){
        // This will keep checking if the button is triggered every frame
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        Debug.Log(inputVector); // Show input value
        float speed = 5f;
        Debug.Log("I am Moving!");

        //float moveHorizontal = Input.GetAxis("Horizontal");
        //float moveVertical = Input.GetAxis("Vertical");

        //Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        //controller.Move(movement * speed * Time.deltaTime);

        // Calculate movement direction based on input
        Vector3 movement = new Vector3(inputVector.x, 0f, inputVector.y);
        controller.Move(movement * speed * Time.deltaTime);
    }

    public void TranslateMovement (){
        // This will keep checking if the button is triggered every frame
        //Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        Vector2 inputVector = playerInput.actions.FindAction("Player/Move").ReadValue<Vector2>();
        Debug.Log(inputVector); // Show input value
        float speed = 5f;
        Debug.Log("I am Moving!");

        //float moveHorizontal = Input.GetAxis("Horizontal");
        //float moveVertical = Input.GetAxis("Vertical");

        //Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        //transform.Translate(movement * speed * Time.deltaTime);

        // Calculate movement direction based on input
        Vector3 movement = new Vector3(inputVector.x, 0f, inputVector.y);
        transform.Translate(movement * speed * Time.deltaTime);
    }

    // Display DropDown menu For Different Player Movement Method
    [CustomEditor(typeof(WASDController))]
    public class WASDControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //WASDController myWASDController = (WASDController)target;

            // Display dropdown to select update method
            //myWASDController.selectedMethod = (SelectedMethod)EditorGUILayout.EnumPopup("Selected Update Method", myWASDController.selectedMethod);

            //WASDController myWASDController = (WASDController)target;

            // Display dropdown to select update method
            //myWASDController.selectedMethod = (WASDController.SelectedMethod)EditorGUILayout.EnumPopup("Selected Update Method", myWASDController.selectedMethod);
        }
    }


    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKeyUp(jumpKey) && !grounded){
            Debug.Log("Jumped, but not ground");
            Debug.Log("Jumped, onSteepGround is " + onSteepGround);
        }
        else if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            //Debug.Log("Jumped, on ground");
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

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

        // Mode - Vaulting
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
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            if (moveSpeed < airMinSpeed)
                desiredMoveSpeed = airMinSpeed;
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

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    private void GroundCheck() 
    {
        grounded = CheckGround();
        //GroundCheck0();
    }
    
    private void GroundCheck0()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.4f, whatIsGround);
    }
    private void GroundCheck1() // work
    {
        // Cast a single raycast straight down from the player's position
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, playerHeight * 0.5f + 0.2f, whatIsGround))
        {
            grounded = true;

            // Check if the slope angle exceeds the threshold
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle > maxSlopeAngle)
            {
                onSteepGround = true;
            }
            else
            {
                onSteepGround = false;
            }
        }
        else
        {
            grounded = false;
            onSteepGround = false;
        }
    }

    private void GroundCheck2()
    {
        grounded = false;
        onSteepGround = false;

        // Cast multiple rays downward from the player's position
        for (int i = 0; i < numRaycasts; i++)
        {
            float angle = i * (360f / numRaycasts); // Calculate angle for raycast direction
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * -transform.forward; // Calculate raycast direction

            //groundCheckDistance = playerHeight * 0.5f + 0.2f;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, playerHeight * 0.5f + 0.2f, whatIsGround))
            //if (Physics.Raycast(transform.position, direction, out hit, groundCheckDistance, whatIsGround))
            {
                grounded = true;

                // Check if the slope angle exceeds the threshold
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle > slopeThreshold)
                {
                    onSteepGround = true;
                    break; // Exit loop if a steep slope is detected
                }
            }
        }
    }


    // Check if the player is grounded using a sphere cast.
    private bool CheckGround()
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
}
