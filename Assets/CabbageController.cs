using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// EnemyMovement.cs
[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover))]
public class CabbageController : MonoBehaviour, IDamageable
{
    public Transform[] targets;
    public float UpdateRate = 0.1f;
    private NavMeshAgent Agent;
    private AgentLinkMover LinkMover;
    float speed = 5f;
    float lifetime = 3f;

    [SerializeField]
    private AttackRadius AttackRadius;
    [SerializeField]
    private Animator Animator;
    private Coroutine LookCoroutine;

    [SerializeField]
    private int Health; // Read EnemyStats.cs
    // Reference to the EnemyStats
    private EnemyStats enemyStats;

    private const string ATTACK_TRIGGER = "Attack";

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        LinkMover = GetComponent<AgentLinkMover>();

        //LinkMover.OnLinkStart += HandleLinkStart;
        //LinkMover.OnLinkEnd += HandleLinkEnd;
        Health = enemyStats.currentHP;
    }

    private void Start()
    {
        StartCoroutine(FollowTarget());
    }

    void Update()
    {
        //ReturnToPool();
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);
        
        while(enabled)
        {
            // Check all target in targets, then check
            // if (targets[i]!=null)
            //     Agent.SetDestination(targets[i].transform.position);
            yield return Wait;
        }
    }

    private void OnAttack(IDamageable Target)
    {
        Animator.SetTrigger(ATTACK_TRIGGER);

        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(LookAt(Target.GetTransform()));
    }

    private IEnumerator LookAt(Transform Target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(Target.position - transform.position);
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

            time += Time.deltaTime * 2;
            yield return null;
        }

        transform.rotation = lookRotation;
    }

    public void TakeDamage(int Damage)
    {
        Health -= Damage;

        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
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
}
