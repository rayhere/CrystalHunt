using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// EnemyMovement.cs
[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover), typeof(LineRenderer))]
public class SupporterController : MonoBehaviour
{
    [Header("References")]
    //[SerializeField, Tooltip("Reference your NavMesh data asset here")]
    //public NavMeshData myNavMeshData; // Reference to your NavMeshData
    //private NavMeshDataInstance navMeshDataInstance; // Instance of the NavMeshData
    public Transform playerModel;

    // Patrol Settings
    [Header("Patrol Settings")]
    [Tooltip("Array of patrol waypoints for the Supporter to follow.")]
    public Transform[] patrolWaypoints;  // Array of patrol waypoints
    [Tooltip("Speed of patrol movement.")]
    public float patrolSpeed = 3f;       // Speed of patrol movement
    public float chaseSpeed = 10f;        // Speed of chasing the target
    public float advoidSpeed = 15f;        // Speed of chasing the target
    public float chaseDistance = 20f;    // Distance at which Supporter starts chasing
    public float chaseAngle = 40f;       // Angle within which Supporter detects the target
    private int currentWaypointIndex;
    private bool isChasing = false;
    [Tooltip("Minimum distance for waypoints from spawn location.")]
    public float minWaypointDistance = 5f; // Minimum distance for waypoints from spawn location
    [Tooltip("Maximum distance for waypoints from spawn location.")]
    public float maxWaypointDistance = 20f; // Maximum distance for waypoints from spawn location
    public Vector3 mySpawnedPosition; // Used for setup Nearby PatrolWaypoints


    // Coroutine for Chasing Target
    private Coroutine chasingCoroutine;
    
    // Inspector Variable for Chasing Distance
    [Header("Chasing Distance Settings")]
    [Tooltip("Distance at which the Supporter will start chasing the target.")]
    public float chasingTargetDistance = 15f; // Distance at which the Supporter will start chasing the target

    public Transform[] items;
    public Transform selectedTarget;
    public float UpdateRate = 0.5f; // MoveTowardsItem, and FollowTarget
    public NavMeshAgent agent;
    private AgentLinkMover LinkMover;
    float speed = 5f;
    float lifetime = 3f; // Lifetime of the Supporter before returning to pool


    [SerializeField]
    private Animator Animator = null;
    private const string IsIdle = "IsIdle";
    private const string IsWalking = "IsWalking";
    private const string IsRunning = "IsRunning";
    private const string Jump = "Jump";
    private const string Landed = "Landed";

    private Coroutine FollowCoroutine;

    [SerializeField]
    private int Health; // Read EnemyStats.cs
    // Reference to the EnemyStats
    private EnemyStats enemyStats;

    private LineRenderer myLineRenderer;

    public float avoidanceRange = 20f;  // Range within which to detect and avoid targets

    public List<Transform> avoidTargets = new List<Transform>();


    public float awarenessRadius = 20f;             // Radius within which the AI detects targets or items
    public float chaseRange = 20f;                  // Range within which the AI starts chasing a detected target
    public float searchRange = 30f;                 // Range within which the AI searches for items
    public float scanInterval = 2f;                 // Interval at which the AI scans its surroundings
    public LayerMask avoidTargetLayer;              // Layer(s) to consider as potential dangerous targets
    public LayerMask teammateTargetLayer;              // Layer(s) to consider as potential teammate targets
    public LayerMask enemyTargetLayer;              // Layer(s) to consider as potential enemy targets
    //public LayerMask targetLayer;                   // Layer(s) to consider as potential targets
    public LayerMask itemLayer;                     // Layer(s) to consider as valuable items
    //public float patrolSpeed = 3.5f;                // Speed at which the AI patrols
    //public float chaseSpeed = 5f;                   // Speed at which the AI chases the target
    public float searchSpeed = 3f;                  // Speed at which the AI moves towards an item
    public float turnSpeed = 5f;                    // Speed at which the AI turns towards its target

    //private NavMeshAgent agent;
    public Transform avoidTarget;
    public bool isAvoidTarget = false;
    private Transform teammateTarget;
    private Transform enemyTarget;
    private Transform target;
    private Vector3 lastKnownPosition;              // Last known position of the target
    public bool isAware;                           // Flag to indicate if the AI is aware of a target
    public Transform nearestItem;                  // Nearest detected valuable item
    public bool isSearchingItem;                   // Flag to indicate if the AI is searching for an item

    // Track if avoidTarget was added during the current frame
    private bool avoidTargetAddedThisFrame = false;


    public Vector3 currentDestination; // for display in Inspector

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        LinkMover = GetComponent<AgentLinkMover>();

        LinkMover.OnLinkStart += HandleLinkStart;
        LinkMover.OnLinkEnd += HandleLinkEnd;

        SetupItemTargetUsingTag("Crystal");
        SetupLineRenderer();
    }

    private void Start()
    {
        if (patrolWaypoints.Length > 0)
        {
            agent.SetDestination(patrolWaypoints[0].position);
            currentWaypointIndex = 0;
        }
        // Invoke SetupPatrolWayPoints after .1 second delay
        Invoke("DelayedStartUp", 0.1f);
    }

    private void DelayedStartUp()
    {
        SetupPatrolWayPoints();
        StartCoroutine(ScanForTargetsAndItems());
        StartCoroutine(UpdateAnimation());
        StartCoroutine(RotateTowardsPathDestination());
    }

    private void Update()
    {
        StartCoroutine(UpdateBehavior());
    }

    private void FixedUpdate()
    {
        DrawPath();
        currentDestination = agent.destination; // just for display in inspector
    }

    IEnumerator UpdateAnimation()
    {   
        while (true)
        {
            yield return new WaitForSeconds(0.5f) ;
            PlayIdlingAnim();
            PlayWalkingAnim();
            PlayRunningAnim();
        }
    }
    private void PlayIdlingAnim()
    {
        Animator.SetBool(IsIdle, agent.velocity.magnitude < 0.3f);
    }

    private void PlayWalkingAnim()
    {
        Animator.SetBool(IsWalking, (agent.velocity.magnitude > 0.3f) && (agent.velocity.magnitude < patrolSpeed +1));
    }

    private void PlayRunningAnim()
    {
        Animator.SetBool(IsRunning, agent.velocity.magnitude >= (patrolSpeed + 1));
    }

    private void HandleLinkStart()
    {
        // Handle any animations or behaviors when agent starts following a link (optional)
        Animator.SetTrigger(Jump);
    }

    private void HandleLinkEnd()
    {
        // Handle any animations or behaviors when agent finishes following a link (optional)
        Animator.SetTrigger(Landed);
    }

    IEnumerator UpdateBehavior()
    {
        yield return new WaitForSeconds (0.1f);

        if (isAware)
        {
            if (avoidTargets.Count > 0)
            {
                AvoidTargets();
                Debug.Log(gameObject.name + " is running away from ");
                isAvoidTarget = true;
            }
            // If aware and target is within chase range, chase it
            else if (Vector3.Distance(transform.position, lastKnownPosition) <= chaseRange)
            {
                ChaseTarget();
                isAvoidTarget = false;
            }
            else
            {
                // Lost track of target, stop chasing and resume patrolling
                isAware = false;
                target = null;
                Patrol();
            }
        }
        else if (isSearchingItem)
        {
            // If searching for an item, move towards it
            MoveTowardsItem();
            //Debug.Log(gameObject.name+": item found, MoveTowardsItem");
        }
        else
        {
            // Not aware of any targets or items, continue patrolling
            Patrol();
        }
    }

    IEnumerator ScanForTargetsAndItems()
    {
        // Scan for targets and items
        while (true)
        {
            // Check if items were added this frame and schedule clearing if they were
            if (avoidTargetAddedThisFrame)
            {
                StartCoroutine(ClearAvoidTargetsAfterDelay());
                avoidTargetAddedThisFrame = false; // Reset flag after scheduling
            }
            
            // Detect all nearby Danger objects with the tags "StoneCube" and "Cabbage"
            if (avoidTargets.Count < 1)
            {
                Collider[] avoidColliders = Physics.OverlapSphere(transform.position, avoidanceRange, avoidTargetLayer);
                Debug.Log($"Found {avoidColliders.Length} colliders in scan.");
                foreach (Collider avoidCollider in avoidColliders)
                {
                    if (avoidCollider.gameObject.activeSelf)
                    {
                        Debug.Log($"Collider tag: {avoidCollider.tag}");
                        if (avoidCollider.CompareTag("StoneCube") || avoidCollider.CompareTag("Cabbage"))
                        {
                            isAware = true;
                            isSearchingItem = false;  // Stop searching for items if chasing a teammateTarget
                            nearestItem = null;
                            avoidTargets.Add(avoidCollider.transform);
                            avoidTargetAddedThisFrame = true; // Set flag to true when items are added
                        }
                    }
                    else
                    {
                        Debug.Log($"Collider tag: {avoidCollider.tag}" + " but GameObject is not active");
                    }
                }
            }
            
            // Scan for teammateTargets
            Collider[] teammateColliders = Physics.OverlapSphere(transform.position, awarenessRadius, teammateTargetLayer);
            foreach (Collider teammateCollider in teammateColliders)
            {
                if (teammateCollider.gameObject.activeSelf)
                {
                    // Check if the collider is a valid target (for example, tagged as "Player")
                    if (teammateCollider.CompareTag("Player") || teammateCollider.CompareTag("Darkness"))
                    {
                        teammateTarget = teammateCollider.transform;
                        lastKnownPosition = teammateTarget.position;
                        isAware = true;
                        isSearchingItem = false;  // Stop searching for items if chasing a teammateTarget
                        nearestItem = null;
                        break;
                    }
                }
                else
                {
                    Debug.Log($"Collider tag: {teammateCollider.tag}" + " but GameObject is not active");
                }
            }

            // Scan for enemytargets
            Collider[] enemyColliders = Physics.OverlapSphere(transform.position, awarenessRadius, enemyTargetLayer);
            foreach (Collider enemyCollider in enemyColliders)
            {
                if (enemyCollider.gameObject.activeSelf)
                {
                    // Check if the collider is a valid target (for example, enemyTarget as "Cabbage")
                    if (enemyCollider.CompareTag("Cabbage"))
                    {
                        enemyTarget = enemyCollider.transform;
                        lastKnownPosition = enemyTarget.position;
                        isAware = true;
                        isSearchingItem = false;  // Stop searching for items if chasing a enemyTarget
                        nearestItem = null;
                        break;
                    }
                }
                else
                {
                    Debug.Log($"Collider tag: {enemyCollider.tag}" + " but GameObject is not active");
                }
            }

            // Scan for valuable items
            Collider[] itemColliders = Physics.OverlapSphere(transform.position, searchRange, itemLayer);
            // only gameobject with collider will be detected
            float nearestDistance = Mathf.Infinity;
            foreach (Collider itemCollider in itemColliders)
            {
                if (itemCollider.gameObject.activeSelf)
                {
                    // Check if the collider is a valuable item
                    // For simplicity, let's assume items are tagged as "Item"
                    if (itemCollider.CompareTag("Crystal"))
                    {
                        float distance = Vector3.Distance(transform.position, itemCollider.transform.position);
                        if (distance < nearestDistance)
                        {
                            // Get the parent game object of the item collider
                            GameObject parentObject = itemCollider.transform.parent.gameObject;

                            // Check if the parent game object is active
                            if (parentObject.activeSelf)
                            {
                                // Assign the parent transform of the itemCollider's gameObject to nearestItem
                                nearestItem = itemCollider.transform.parent.transform;
                                Debug.Log(gameObject.name + ": found " + itemCollider.transform.parent.name + " locate at: " + itemCollider.transform.parent.position);

                                // Update nearestDistance to the new closest distance
                                nearestDistance = distance;
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log($"Collider tag: {itemCollider.tag}" + " but GameObject is not active");
                }
            }

            // If found a nearest item, start searching for it
            if (nearestItem != null)
            {
                if (nearestItem.gameObject.activeSelf)
                {
                    isSearchingItem = true;
                    //isAware = false;  // Stop chasing targets if searching for an item
                }
            }
            yield return new WaitForSeconds(scanInterval);
        }
    }

    void AvoidTargets()
    {
        if (avoidTargets.Count > 0)
        {
            // Calculate the position to move away from all targets
            Vector3 awayDirection = Vector3.zero;
            foreach (Transform avoidTarget in avoidTargets)
            {
                awayDirection += (transform.position - avoidTarget.position).normalized;
            }
            Vector3 awayPosition = transform.position + awayDirection.normalized * avoidanceRange;
            
            // Move towards the calculated position
            NavMeshHit hit;
            NavMesh.SamplePosition(awayPosition, out hit, avoidanceRange, NavMesh.AllAreas);
            agent.SetDestination(hit.position);
            agent.speed = advoidSpeed;

            if (NavMesh.SamplePosition(awayPosition, out hit, avoidanceRange, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                agent.speed = advoidSpeed;
            }
            else
            {
                Debug.LogWarning("Failed to find a valid position on NavMesh.");
            }
        }
    }

    // Coroutine to clear avoidTargets after a delay
    IEnumerator ClearAvoidTargetsAfterDelay()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        avoidTargets.Clear(); // Clear the avoidTargets list
    }

    private void SetupItemTargetUsingTag(string tagName)
    {
        // Find the target object using tag
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(tagName);

        items = new Transform[foundObjects.Length];

        for (int i = 0; i < foundObjects.Length; i++)
        {
            items[i] = foundObjects[i].transform;
        }
    }

    void Patrol()
    {
        // Implement your patrolling logic here (e.g., move between waypoints)
        // For simplicity, let's just move randomly if no target or item is detected
        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas);
            agent.SetDestination(hit.position);
            agent.speed = patrolSpeed;
        }
    }

    void ChaseTarget()
    {
        // Chase the target's last known position
        agent.SetDestination(lastKnownPosition);
        agent.speed = chaseSpeed;

        // Rotate towards the target's position
        LookAtPosition(lastKnownPosition);
    }

    private void LookAtPosition(Vector3 targetPosition)
    {
        // Rotate towards the target's position
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    private void MoveTowardsItem()
    {
        Debug.Log(gameObject.name+": item found, MoveTowardsItem");
        if (nearestItem != null)
        {
            if (nearestItem.gameObject.activeSelf)
            {
                // Move towards the nearest item's position
                agent.SetDestination(nearestItem.position);
                Debug.Log("Set item " + nearestItem.name + " locate at: " + nearestItem.position);
                agent.speed = searchSpeed;

                // Rotate towards the item's position
                LookAtPosition(nearestItem.position);

                if (agent.remainingDistance < 0.5f) nearestItem = null;
            }
            else
            {
                nearestItem = null;
                isSearchingItem = false;
            }
        }
        else
        {
            // No item found, resume patrolling
            isSearchingItem = false;
            Patrol();
        }
    }

    private void SetupPatrolWayPoints()
    {
        // Clear previous waypoints
        List<Transform> waypoints = new List<Transform>();

        // Add first waypoint as the spawned position
        waypoints.Add(new GameObject($"{gameObject.name}_SpawnedWaypoint").transform);
        waypoints[0].position = mySpawnedPosition;
        waypoints[0].parent = transform.parent; // Set parent to the parent of Supporter GameObject

        // Generate additional waypoints near the spawned position
        for (int i = 1; i <= 3; i++) // Create 3 additional waypoints
        {
            Vector3 randomDirection = Random.insideUnitSphere * maxWaypointDistance;
            randomDirection.y = 0f; // Ensure waypoints are on the same plane as Supporter

            Vector3 waypointPosition = mySpawnedPosition + randomDirection;

            // Ensure the waypoints are within the NavMesh bounds and within specified distances
            NavMeshHit hit;
            if (NavMesh.SamplePosition(waypointPosition, out hit, maxWaypointDistance, NavMesh.AllAreas) &&
                Vector3.Distance(waypointPosition, mySpawnedPosition) >= minWaypointDistance)
            {
                Transform newWaypoint = new GameObject($"{gameObject.name}_Waypoint{i}").transform;
                newWaypoint.position = hit.position;
                newWaypoint.parent = transform.parent; // Set parent to the parent of Supporter GameObject
                waypoints.Add(newWaypoint);
            }
        }

        patrolWaypoints = waypoints.ToArray();
    }

    // Initialize the Supporter's movement and lifetime
    public void Initialise(Vector3 targetPosition)
    {
        transform.position = targetPosition;
        mySpawnedPosition = targetPosition;
        Debug.Log("Supporter spawned location is " + targetPosition);
        FountainDirectionSpawn();
    }

    void FountainDirectionSpawn()
    {
        Vector3 direction = Vector3.up;

        // Move the object towards the target position
        GetComponent<Rigidbody>().velocity = direction * speed;

        // Rotate the object to face the movement direction (optional)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        playerModel.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //lifetime = 3f;
    }

    // Method to handle the Supporter's death
    public void Die()
    {
        // Add death behavior here (e.g., play death animation, disable enemy object)
        // Read EmemyStats.cs

        lifetime = 3f;
        ReturnToPool();
    }

    void ReturnToPool()
    {
        // Handle returning to object pool based on lifetime (if needed)

        // Reduce lifetime
        lifetime -= Time.deltaTime;

        // If lifetime is expired, return the Supporter to the object pool
        if (lifetime <= 0f)
        {
            ResetState();
            ObjectPooler.EnqueueObject(this, "Supporter");
        }
    }

    void ResetState()
    {
        // Reset the Supporter's state when returning to the pool
        // EnqueueObject<T> will Reset the Supporter's state in ObjectPooler.cs

        // reset EnemyStats.cs 
    }

    private IEnumerator LookAtTarget(Transform Target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(Target.position - transform.position);
        float time = 0;

        while (time < 1)
        {
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, lookRotation, time);

            time += Time.deltaTime * 2;
            yield return null;
        }

        playerModel.transform.rotation = lookRotation;
    }
    
    void SetupLineRenderer()
    {
        myLineRenderer = GetComponent<LineRenderer>();
        myLineRenderer.positionCount = 0;

        // Set the width of the LineRenderer
        myLineRenderer.startWidth = 0.1f;
        myLineRenderer.endWidth = 0.1f;

        // Check if the LineRenderer has a material assigned
        if (myLineRenderer.material == null)
        {
            // Create a new material if none is assigned
            myLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        // Set the color of the material to green
        myLineRenderer.material.color = Color.green;
    }

    void DrawPath()
    {
        if (!agent.hasPath) return;
 
        myLineRenderer.positionCount = agent.path.corners.Length;
        myLineRenderer.SetPosition(0, transform.position);

        if (agent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < agent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(agent.path.corners[i].x, agent.path.corners[i].y, agent.path.corners[i].z);
            myLineRenderer.SetPosition(i, pointPosition);
        }
    }

    IEnumerator RotateTowardsPathDestination()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f) ;
            if (agent.hasPath && agent.path.corners.Length > 0)
            {
                // Get the direction to the current destination on the path
                Vector3 direction = agent.path.corners[agent.path.corners.Length - 1] - playerModel.transform.position;

                // Ensure the direction is not zero (to prevent zero division)
                if (direction != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, rotation, Time.deltaTime * turnSpeed);
                }
            }
        }
    }
}
