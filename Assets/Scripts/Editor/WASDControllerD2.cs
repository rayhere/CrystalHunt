using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class WASDControllerD2 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;

    // Unity Events for Input Actions
    public UnityEvent onMove;
    public UnityEvent onJump;

    private Rigidbody rb;
    private PlayerInput playerInput;
    //private Animator animator;
    private bool isGrounded;

    // Animator parameter names
    private readonly string isRunningParam = "isRunning";

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        //animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Mouse clicked!
        }
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            Debug.Log("Switch to UI ActionMap!");
            playerInput.SwitchCurrentActionMap("UI");
        }
        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            Debug.Log("Switch to Player ActionMap!");
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            // Call the movement method
            Move();
        }
    }

    // Input Action method to handle movement
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);
        rb.velocity = movement * moveSpeed;

        if (context.performed)
        {
            Debug.Log("Movement! " + context.phase);
            onMove.Invoke(); // Invoke Unity Event for movement
            // Update animator parameter
            //animator.SetBool(isRunningParam, movementInput.magnitude > 0);
        }
    }

    // Input Action method to handle jumping
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            Debug.Log("Jump! " + context.phase);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            onJump.Invoke(); // Invoke Unity Event for jumping
        }
    }

    // Method to check if the player is grounded
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = collision.collider.CompareTag("Ground");
    }

    // Method for movement
    private void Move2()
    {
        //cause flip
        Vector2 inputVector = playerInput.actions.FindAction("Player/Move").ReadValue<Vector2>();
        Vector3 movement = new Vector3(inputVector.x, 0f, inputVector.y);
        rb.velocity = movement * moveSpeed;
    }

    // Method for movement
// Method for movement
// Method for movement
private void Move()
{
    // Get input direction relative to the world space
    Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

    // Transform the input direction to the character's local space
    Vector3 movement = transform.TransformDirection(inputDirection);

    // Preserve the current y-velocity to maintain gravity's effect
    float currentYVelocity = rb.velocity.y;

    // Apply the input movement only on the x-z plane
    rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);

    // Rotate the character towards the movement direction if moving
    if (rb.velocity.magnitude > 0.1f)
    {
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(rb.velocity.x, 0f, rb.velocity.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
    }
    
    // Preserve the y-velocity to maintain gravity's effect
    rb.velocity = new Vector3(rb.velocity.x, currentYVelocity, rb.velocity.z);
}



}
