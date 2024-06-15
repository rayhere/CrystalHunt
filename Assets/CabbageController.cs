using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// EnemyMovement.cs
[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover), typeof(LineRenderer))]
public class CabbageController : MonoBehaviour
{
    public Transform[] targets;
    public Transform selectedTarget;
    public float UpdateRate = 0.1f;
    private NavMeshAgent Agent;
    private AgentLinkMover LinkMover;
    float speed = 5f;
    float lifetime = 3f;


    [SerializeField]
    private Animator Animator = null;
    private const string IsWalking = "IsWalking";
    private const string Jump = "Jump";
    private const string Landed = "Landed";

    private Coroutine FollowCoroutine;

    [SerializeField]
    private int Health; // Read EnemyStats.cs
    // Reference to the EnemyStats
    private EnemyStats enemyStats;

    private LineRenderer myLineRenderer;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        LinkMover = GetComponent<AgentLinkMover>();

        LinkMover.OnLinkStart += HandleLinkStart;
        LinkMover.OnLinkEnd += HandleLinkEnd;
        //Health = enemyStats.currentHP;

        SetupTargetUsingTag();
        //SetupTargetUsingTag("Player");


        SetupLineRenderer();
        
    }

    private void Start()
    {
        SetupTargetUsingTag();
        StartCoroutine(FollowTarget());
    }

    private void Update()
    {
        //ReturnToPool();
        
    }

    private void FixedUpdate()
    {
        DrawPath();
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

    private Transform SelectTargetFromTargets()
    {
        // Pick the last target if targets is not null and contains elements
        if (targets != null && targets.Length > 0)
        {
            Transform lastTarget = targets[targets.Length - 1];
            // Use lastTarget as needed
            Debug.Log("Last target: " + lastTarget.name);
            return lastTarget;
        }
        else
        {
        Debug.LogWarning("No targets found or targets array is empty.");
        return null; // Return null if no valid target is found
    }
    }



    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);
        
        while (gameObject.activeSelf)
        {
            if (selectedTarget == null) 
            {
                Debug.Log("selectedTarget == null, do SelectTargetFromTargets()");
                selectedTarget = SelectTargetFromTargets();
            }
            
            Agent.SetDestination(selectedTarget.transform.position);
            yield return Wait;
        }
    }

    private void HandleLinkStart()
    {
        Animator.SetTrigger(Jump);
    }

    private void HandleLinkEnd()
    {
        Animator.SetTrigger(Landed);
    }

    private void PlayWalkingAnim()
    {
        Animator.SetBool(IsWalking, Agent.velocity.magnitude > 0.01f);
    }

    // Initialize the cabbage's movement and lifetime
    public void Initialise(Vector3 targetPosition)
    {
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
