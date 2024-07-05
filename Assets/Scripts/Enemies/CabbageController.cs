using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// EnemyMovement.cs
[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover), typeof(LineRenderer))]
public class CabbageController : MonoBehaviour
{
    // Patrol Settings
    [Header("Patrol Settings")]
    [Tooltip("Array of patrol waypoints for the cabbage to follow.")]
    public Transform[] patrolWaypoints;  // Array of patrol waypoints
    [Tooltip("Speed of patrol movement.")]
    public float patrolSpeed = 2f;       // Speed of patrol movement
    public float chaseSpeed = 5f;        // Speed of chasing the target
    public float chaseDistance = 10f;    // Distance at which cabbage starts chasing
    public float chaseAngle = 30f;       // Angle within which cabbage detects the target
    public int currentWaypointIndex;
    public bool isChasing = false;
    [Tooltip("Minimum distance for waypoints from spawn location.")]
    public float minWaypointDistance = 5f; // Minimum distance for waypoints from spawn location
    [Tooltip("Maximum distance for waypoints from spawn location.")]
    public float maxWaypointDistance = 10f; // Maximum distance for waypoints from spawn location
    public Vector3 mySpawnedPosition; // Used for setup Nearby PatrolWaypoints

    // Coroutine for Chasing Target
    private Coroutine chasingCoroutine;
    
    // Inspector Variable for Chasing Distance
    [Header("Chasing Distance Settings")]
    [Tooltip("Distance at which the cabbage will start chasing the target.")]
    public float chasingTargetDistance = 15f; // Distance at which the cabbage will start chasing the target

    public Transform[] targets;
    public Transform selectedTarget;
    public float UpdateRate = 0.1f;
    public NavMeshAgent Agent;
    private AgentLinkMover LinkMover;
    float speed = 5f;
    float lifetime = 3f; // Lifetime of the cabbage before returning to pool


    [SerializeField]
    public Animator Animator = null;
    private const string IsWalking = "IsWalking";
    private const string Jump = "Jump";
    private const string Landed = "Landed";

    private Coroutine FollowCoroutine;

    [SerializeField]
    private int Health; // Read EnemyStats.cs
    // Reference to the EnemyStats
    private EnemyStats enemyStats;

    private LineRenderer myLineRenderer;

    [Header("Check")]

    [SerializeField]
    public float remainingDistance;
    public Vector3 givenPatorlWaypoint;
    public float distanceThreshold = 0.1f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        LinkMover = GetComponent<AgentLinkMover>();

        LinkMover.OnLinkStart += HandleLinkStart;
        LinkMover.OnLinkEnd += HandleLinkEnd;

        SetupTargetUsingTag();
        SetupLineRenderer();
    }

    private void Start()
    {
        if (patrolWaypoints.Length > 0)
        {
            Agent.SetDestination(patrolWaypoints[0].position);
            currentWaypointIndex = 0;
        }

        // Invoke SetupPatrolWayPoints after .1 second delay
        Invoke("DelayedSetupPatrolWayPoints", 0.1f);
    }

    private void DelayedSetupPatrolWayPoints()
    {
        SetupPatrolWayPoints();
        StartCoroutine(FollowPatrolRoute());
        StartCoroutine(CheckForTarget());
    }

    private void Update()
    {
        // Check if in range and angle to start chasing
        if (!isChasing)
        {
            CheckForChaseTarget();
        }
    }

    private void FixedUpdate()
    {
        DrawPath();
        StartCoroutine(UpdateAnimation());
    }

    private IEnumerator UpdateAnimation()
    {   
        while (true)
        {
            yield return new WaitForSeconds(0.5f) ;
            PlayWalkingAnim();
        }
    }

    private IEnumerator FollowPatrolRoute()
    {
        while (gameObject.activeSelf)
        {
            if (!isChasing && !Agent.pathPending)
            {
                // Check if agent has reached the current waypoint
                if (Agent.remainingDistance <= Agent.stoppingDistance)
                {
                    currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
                    Agent.SetDestination(patrolWaypoints[currentWaypointIndex].position);
                    givenPatorlWaypoint = patrolWaypoints[currentWaypointIndex].position;
                }
            }
            yield return null;
        }
    }


    private void SetupTargetUsingTag()
    {
        // Find the target object using tag
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag("Player");

        targets = new Transform[foundObjects.Length];

        for (int i = 0; i < foundObjects.Length; i++)
        {
            targets[i] = foundObjects[i].transform;
        }
    }

    private void SetupTargetUsingTag(string tagName)
    {
        // Find the target object using tag
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(tagName);

        targets = new Transform[foundObjects.Length];

        for (int i = 0; i < foundObjects.Length; i++)
        {
            targets[i] = foundObjects[i].transform;
        }
    }

    private void CheckForChaseTarget()
    {
        if (targets != null && targets.Length > 0)
        {
            foreach (Transform target in targets)
            {
                Vector3 toTarget = target.position - transform.position;
                float angle = Vector3.Angle(transform.forward, toTarget);

                if (angle <= chaseAngle)
                {
                    float distance = toTarget.magnitude;
                    if (distance <= chaseDistance)
                    {
                        StartCoroutine(ChasingTarget(target));
                        break;
                    }
                }
            }
        }
    }

    // Coroutine to continuously check distance to target and start/stop chasing
    private IEnumerator ChasingTarget(Transform selectedTarget)
    {
        while (gameObject.activeSelf)
        {
            if (!isChasing && selectedTarget != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, selectedTarget.position);

                if (distanceToTarget <= chasingTargetDistance)
                {
                    StartChasing(selectedTarget);
                }
            }
            else if (isChasing && selectedTarget != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, selectedTarget.position);

                if (distanceToTarget > chasingTargetDistance)
                {
                    StopChasing();
                    yield break;
                }
                else
                {
                    StartChasing(selectedTarget);
                }
            }
            else if (isChasing && selectedTarget == null)
            {
                StopChasing();
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
            // Code here will execute after 0.5 seconds of delay
            yield return null;
            // Code here will execute in the next frame
        }
    }

    // Method to start chasing the specified target
    private void StartChasing(Transform target)
    {
        isChasing = true;
        Agent.speed = chaseSpeed;
        Agent.SetDestination(target.position);
    }

    // Method to stop chasing and resume patrol
    private void StopChasing()
    {
        isChasing = false;
        Agent.speed = patrolSpeed;

        if (patrolWaypoints.Length > 0)
        {
            Agent.SetDestination(patrolWaypoints[currentWaypointIndex].position);
        }
    }

    private IEnumerator CheckForTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        while (gameObject.activeSelf)
        {
            FindTargetsInFront();
            yield return wait;
        }
    }

    private void FindTargetsInFront()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, chaseDistance);

        List<Transform> detectedTargets = new List<Transform>();

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Player") || col.CompareTag("Darkness"))
            {
                Vector3 directionToTarget = col.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToTarget);

                if (angle <= chaseAngle)
                {
                    detectedTargets.Add(col.transform);
                }
            }
        }
        targets = detectedTargets.ToArray();
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

    private void PlayWalkingAnim()
    {
        Animator.SetBool(IsWalking, Agent.velocity.magnitude > 0.01f);
    }

    private void SetupPatrolWayPoints()
    {
        // Clear previous waypoints
        List<Transform> waypoints = new List<Transform>();

        // Add first waypoint as the spawned position
        waypoints.Add(new GameObject($"{gameObject.name}_SpawnedWaypoint").transform);
        waypoints[0].position = mySpawnedPosition;
        waypoints[0].parent = transform.parent; // Set parent to the parent of Cabbage GameObject

        // Generate additional waypoints near the spawned position
        int i = 0;
        while (i < 3) // Create 4 additional waypoints
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * maxWaypointDistance;
            randomDirection.y = 0f; // Ensure waypoints are on the same plane as Cabbage

            Vector3 waypointPosition = mySpawnedPosition + randomDirection;

            // Ensure the waypoints are within the NavMesh bounds and within specified distances
            NavMeshHit hit;
            if (NavMesh.SamplePosition(waypointPosition, out hit, maxWaypointDistance, NavMesh.AllAreas) &&
                Vector3.Distance(waypointPosition, mySpawnedPosition) >= minWaypointDistance)
            {
                Transform newWaypoint = new GameObject($"{gameObject.name}_Waypoint{i}").transform;
                newWaypoint.position = hit.position;
                newWaypoint.parent = transform.parent; // Set parent to the parent of Cabbage GameObject
                waypoints.Add(newWaypoint);
                i++;
            }
        }

        patrolWaypoints = waypoints.ToArray();
    }



    // Initialize the cabbage's movement and lifetime
    public void Initialise(Vector3 targetPosition)
    {
        transform.position = targetPosition;
        mySpawnedPosition = targetPosition;
        Debug.Log("Cabbage spawned location is " + targetPosition);
        FountainDirectionSpawn();
    }


    void FountainDirectionSpawn()
    {
        Vector3 direction = Vector3.up;

        // Move the object towards the target position
        GetComponent<Rigidbody>().velocity = direction * speed;

        // Rotate the object to face the movement direction (optional)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //lifetime = 3f;
    }

    // Method to handle the cabbage's death
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

        // If lifetime is expired, return the cabbage to the object pool
        if (lifetime <= 0f)
        {
            ResetState();
            ObjectPooler.EnqueueObject(this, "Cabbage");
        }
    }

    void ResetState()
    {
        // Reset the cabbage's state when returning to the pool
        // EnqueueObject<T> will Reset the cabbage's state in ObjectPooler.cs

        // reset EnemyStats.cs 
    }
    
    

    public void SetTargets(Transform[] targets)
    {
        this.targets = targets;
    }

    public void SetTarget(Transform target)
    {
        targets = new Transform[] { target };
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
        if (!Agent.hasPath) return;
 
        myLineRenderer.positionCount = Agent.path.corners.Length;
        myLineRenderer.SetPosition(0, transform.position);

        if (Agent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < Agent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(Agent.path.corners[i].x, Agent.path.corners[i].y, Agent.path.corners[i].z);
            myLineRenderer.SetPosition(i, pointPosition);
        }
    }
}
