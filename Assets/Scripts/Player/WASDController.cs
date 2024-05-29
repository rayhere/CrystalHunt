using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class WASDController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Animation")]
    public Animator animator;
    public string idleAnimation = "Idle";
    public string walkAnimation = "Walk";
    public string jumpAnimation = "Jump";

    [Header("Audio")]
    public AudioSource jumpSound;
    public AudioSource walkSound;

    // Unity Events for Input Actions
    public UnityEvent onMove;
    public UnityEvent onJump;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Input Action method to handle movement
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);
        rb.velocity = movement * moveSpeed;

        // Trigger movement animation or sound
        if (movementInput.magnitude > 0)
        {
            animator.Play(walkAnimation);
            if (!walkSound.isPlaying)
                walkSound.Play();
        }
        else
        {
            animator.Play(idleAnimation);
            walkSound.Stop();
        }

        onMove.Invoke(); // Invoke Unity Event for movement
    }

    // Input Action method to handle jumping
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Trigger jump animation and sound
            animator.Play(jumpAnimation);
            jumpSound.Play();

            onJump.Invoke(); // Invoke Unity Event for jumping
        }
    }

    // Method to check if the player is grounded
    private bool IsGrounded()
    {
        // Implement your grounded check logic here
        return true; // Placeholder implementation
    }
}
