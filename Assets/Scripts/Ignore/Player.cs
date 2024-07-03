using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Part controls are treated as analog meaning that the floating-point values read from controls will come through as is (minus the fact that the down and Left direction values are negated)
/// </summary>
[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [SerializeField, Tooltip("Player speed multiplier.")]
    private float playerSpeed = 5.0f;
    [SerializeField, Tooltip("How the player should jump.")]
    private float jumpHeight = 1.0f;
    [SerializeField, Tooltip("Downwards force on the player.")]
    private float gravityValue = -9.81f;
    [SerializeField, Tooltip("Rotation Speed multiplier.")]
    private float rotationSpeed = 5f;
    [SerializeField, Tooltip("Player speed multiplier.")]
    private float animationBlendDamp = .3f;
    [SerializeField, Tooltip("Input smooth damp speed.")]
    private float inputSmoothDamp = .3f;
    [SerializeField]
    private float smoothInputSpeed = .2f;

    private PlayerInput playerInput;
    private Animator anim;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    private InputAction moveAction;
    private InputAction jumpAction;
    
    private int jumpAnimationId;
    private int blendAnimationParameterID;

    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        jumpAnimationId = Animator.StringToHash("Jump");
        blendAnimationParameterID = Animator.StringToHash("Blend");
    }


    void Update() 
    {
        groundedPlayer = controller.isGrounded;
        // Remove downwards force when player is grounded.
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        currentInputVector = Vector2.SmoothDamp(currentInputVector, input, ref smoothInputVelocity, smoothInputSpeed);
        Vector3 move = new Vector3(currentInputVector.x, 0, currentInputVector.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Changes the height position of the player.
        if (jumpAction.triggered && groundedPlayer)
        {
            anim.Play(jumpAnimationId);
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Set blending animation when player is moving.
        anim.SetFloat (blendAnimationParameterID, input.sqrMagnitude, animationBlendDamp, Time.deltaTime);

        // Rotate the player depending on their input and camera direction.
        if (input != Vector2.zero){
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        } 
    }     
}
