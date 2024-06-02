using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEditor;

public class WASDControllerD3 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

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

    private Rigidbody rb;
    private PlayerInput playerInput;

    // For onMove InvokeUnityEvents
    private bool inputMovement = false; 
    // DefaultInputActions
    private DefaultInputActions playerInputActions;

    // Define a delegate type
    public delegate void UpdateMethod();

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
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        // Assign the input actions
        //playerInputActions = playerInput.actions;

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
}
