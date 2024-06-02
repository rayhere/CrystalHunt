using UnityEngine;
using UnityEngine.AI;

public class TestController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;
    private bool isJumping = false;
    public float jumpForce = 5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput);
        moveDirection.Normalize();

        if (moveDirection != Vector3.zero)
        {
            agent.enabled = false;
            // Convert from local space to world space
            moveDirection = transform.TransformDirection(moveDirection);
            // Move the agent
            //agent.Move(moveDirection * Time.deltaTime * agent.speed);

            float speed = 5f;
            rb.velocity = moveDirection * speed * Time.deltaTime;
        //playerRigidbody.velocity = movement.normalized * speed * Time.deltaTime;
        } else {
            Invoke("EnableAgent", 3.1f); // Adjust delay if needed
            
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
    }

    void Jump()
    {
        if (!isJumping)
        {
            isJumping = true;
            // Temporarily disable NavMeshAgent to allow free jump
            agent.enabled = false;
            // Apply upward force to the Rigidbody
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            // Invoke method to re-enable NavMeshAgent after a short delay
            Invoke("EnableAgent", 3.1f); // Adjust delay if needed
        }
    }

    void EnableAgent()
    {
        isJumping = false;
        agent.enabled = true;
    }
}
