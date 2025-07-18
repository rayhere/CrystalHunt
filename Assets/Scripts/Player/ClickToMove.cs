using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class ClickToMove : MonoBehaviour
{
    private NavMeshAgent myNavMeshAgent;
    [SerializeField] [Tooltip("myAnim is Character Object with Animator, Character Object should be child of This Object")]
    private Animator myAnim;
    private LineRenderer myLineRenderer;
    private Rigidbody rb;

    [SerializeField] [Tooltip("clickMarkerPrefab Required, Prefab need to be child of GameObject, and Drag it Here")]
    private GameObject clickMarkerPrefab;
    [SerializeField] [Tooltip("visualClickMarker is EmptyObject created for set ClickMarkerPrefab location")]
    private GameObject visualClickMarker;
    [SerializeField] [Tooltip("Layers to perform raycast against")]
    private LayerMask raycastLayerMask = ~0; // Default to all layers

    [SerializeField] [Tooltip("Set Object Rotation Speed Here. Angular Speed set to 0 in NavMeshAgent")]
    private float lookRotationSpeed = 8f;

    private Vector3 destination;

    public bool pauseMenu = false;
    public bool isActive = true; // Flag to control whether script is active


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

    private void Awake()
    {
        CreateVisualObject();
        
    }

    private void CreateVisualObject()
    {
        visualClickMarker = new GameObject("visualClickMarker");
        clickMarkerPrefab.transform.SetParent(visualClickMarker.transform);
    }

    void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myNavMeshAgent.enabled = isActive;
        // Don't assign Animator in this object
        // myAnim = GetComponent<Animator>(); 
        myLineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();

        myLineRenderer.startWidth = 0.15f;
        myLineRenderer.endWidth = 0.15f;
        myLineRenderer.positionCount = 0;
        // Example adjustment to render queue
      /*   if (myLineRenderer.material != null)
        {
            myLineRenderer.material.renderQueue = 200; // Adjust as needed
        } */

        // Set stopping distance to 1.5 units
        //myNavMeshAgent.stoppingDistance = 1.5f;

        //myNavMeshAgent.stoppingDistance = 1.51f; // Adjust slightly above your intended stopping distance

    }

    void Update()
    {
        if (!pauseMenu && isActive)
        {
            HandleClickMovement();
            FaceTarget();
        }
        
    }

    public void ToggleFunction()
    {
        if (myNavMeshAgent.enabled) myNavMeshAgent.ResetPath(); // Stop NavMeshAgent from moving
        myNavMeshAgent.enabled = isActive;
        if (rb != null)
        {
            rb.isKinematic = isActive;
        }
    }

    private void HandleClickMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //if (Physics.Raycast(ray, out hit))
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayerMask))
            {
                destination = hit.point;
                SetDestination(destination);
                myLineRenderer.enabled = true; // Enable the LineRenderer
                myNavMeshAgent.isStopped = false; // Agent start moving
            }
        }

        // If Object keep moving, transform.position may not be reach
        // Set Stopping Distance to Pos.Y 

        if (Vector3.Distance(destination, transform.position) <= myNavMeshAgent.stoppingDistance)
        {
            StopMovement();
        }
        else if (myNavMeshAgent.hasPath)
        {
            DrawPath();
            //Debug.Log("Vector3.Distance(destination, transform.position) is "+ Vector3.Distance(destination, transform.position));
            //Debug.Log("Vector3.Distance(myNavMeshAgent.stoppingDistance) is " + myNavMeshAgent.stoppingDistance);
        }
    }

    private void HandleClickMovementTry()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayerMask))
            {
                int walkableAreaMask = NavMesh.GetAreaFromName("Walkable");
                int obstacleAreaMask = NavMesh.GetAreaFromName("Obstacle");
                int combinedAreaMask = walkableAreaMask | obstacleAreaMask;
                // Define area masks for specific NavMesh areas
                // int darknessAreaMask = NavMesh.GetAreaFromName("DarknessAreaName"); // Replace with actual name or ID
                // Assuming you have obtained the area ID for Darkness (replace with your actual ID)
                // Open NavMeshSurface, double click on the NavMesh Created to get Agent Type ID
                int darknessAreaID = NavMesh.GetAreaFromName("287145453"); 
                // Check if the hit point is on the NavMesh
                NavMeshHit navHit;
                if (NavMesh.SamplePosition(hit.point, out navHit, 1.0f, darknessAreaID))
                {
                    destination = navHit.position;
                    SetDestination(destination);
                    myLineRenderer.enabled = true; // Enable the LineRenderer
                    myNavMeshAgent.isStopped = false; // Agent start moving
                }
            }
        }

        // If Object keep moving, transform.position may not be reach
        // Set Stopping Distance to Pos.Y 

        if (Vector3.Distance(destination, transform.position) <= myNavMeshAgent.stoppingDistance)
        {
            StopMovement();
        }
        else if (myNavMeshAgent.hasPath)
        {
            DrawPath();
        }
    }


    private void SetDestination(Vector3 target)
    {
        //myAnim.SetBool("isStandingIdle", false);
        //myAnim.SetBool("isRunning", true);
        performState = PerformState.isSprinting;
        //myAnim.Play("PlayerRunning");
        myNavMeshAgent.SetDestination(target);

        clickMarkerPrefab.SetActive(true);
        //clickMarkerPrefab.transform.position = new Vector3(target.x, target.y + 0.01f, target.z);
        clickMarkerPrefab.transform.position = myNavMeshAgent.destination; // instead to use raycast.hit, use agent destination
        clickMarkerPrefab.transform.SetParent(visualClickMarker.transform);

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    private void StopMovement()
    {
        if (!myNavMeshAgent.isStopped)
        Debug.Log("Agent isStopped");
        performState = PerformState.isStandingIdle;
        //myAnim.SetBool("isRunning", false);
        //myAnim.SetBool("isStandingIdle", true);
        //myAnim.Play("PlayerStandingIdle");
        
        myNavMeshAgent.isStopped = true; // Agent stop moving

        if (rb != null)
        {
            //rb.velocity = Vector3.zero;
            rb.isKinematic = true; // Disable physics interactions
        }

        clickMarkerPrefab.SetActive(false);

        // Deactivate the LineRenderer
        myLineRenderer.positionCount = 0; // Clear any existing positions
        myLineRenderer.enabled = false; // Disable the LineRenderer
    }

    // Visualize the path
    void DrawPath()
    {
        myLineRenderer.positionCount = myNavMeshAgent.path.corners.Length;
        myLineRenderer.SetPosition(0, transform.position);

        if (myNavMeshAgent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < myNavMeshAgent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(myNavMeshAgent.path.corners[i].x, myNavMeshAgent.path.corners[i].y-.01f, myNavMeshAgent.path.corners[i].z);
            myLineRenderer.SetPosition(i, pointPosition);
        }
    }

    void FaceTarget()
	{
        Vector3 direction = (myNavMeshAgent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        // Check if the difference in rotation exceeds a threshold
        if (Quaternion.Angle(transform.rotation, lookRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
	}
}
