using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Enemy.cs
public class CabbageBehavior : MonoBehaviour, IDamageable
{
    // References
    public AttackRadius AttackRadius;
    public Animator Animator;
    public CabbageController cabbageController; // Movement
    public NavMeshAgent Agent;
    public CabbageScriptableObject CabbageScriptableObject;
    
    // State variables
    private Coroutine lookCoroutine;
    private const string ATTACK_TRIGGER = "Attack";
    public int health = 100;

    // Attack event setup
    private void Awake()
    {
        AttackRadius.OnAttack += OnAttack;
    }

    // Handle attack event
    private void OnAttack(IDamageable Target)
    {
        Animator.SetTrigger(ATTACK_TRIGGER);

        if (lookCoroutine != null)
        {
            StopCoroutine(lookCoroutine);
        }

        lookCoroutine = StartCoroutine(LookAt(Target.GetTransform()));
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

    public virtual void OnEnable()
    {
        SetupAgentFromConfiguration();
    }

    // Agent setup from scriptable object
    public virtual void SetupAgentFromConfiguration()
    {
        Agent.acceleration = CabbageScriptableObject.Acceleration;
        Agent.angularSpeed = CabbageScriptableObject.AngularSpeed;
        Agent.areaMask = CabbageScriptableObject.AreaMask;
        Agent.avoidancePriority = CabbageScriptableObject.AvoidancePriority;
        Agent.baseOffset = CabbageScriptableObject.BaseOffset;
        Agent.height = CabbageScriptableObject.Height;
        Agent.obstacleAvoidanceType = CabbageScriptableObject.ObstacleAvoidanceType;
        Agent.radius = CabbageScriptableObject.Radius;
        Agent.speed = CabbageScriptableObject.Speed;
        Agent.stoppingDistance = CabbageScriptableObject.StoppingDistance;
        
        cabbageController.UpdateRate = CabbageScriptableObject.AIUpdateInterval;

        health = CabbageScriptableObject.Health;

        (AttackRadius.Collider == null ? AttackRadius.GetComponent<SphereCollider>() : AttackRadius.Collider).radius = CabbageScriptableObject.AttackRadius;
        AttackRadius.AttackDelay = CabbageScriptableObject.AttackDelay;
        AttackRadius.Damage = CabbageScriptableObject.Damage;
    }

    // Return to object pool
    public void OnDisable()
    {
        // Return the cabbage to the object pool
        ObjectPooler.EnqueueObject(this, "Cabbage");
    }

    // Take damage method
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    // Get transform method
    public Transform GetTransform()
    {
        return transform;
    }

    // Implementing attack-related interface methods (placeholders)
    public void PerformAttack() { }
    public void StartAttack() { }
    public void EndAttack() { }
    public bool IsAttacking() { return false; }
    public void CancelAttack() { }
    public bool AttackCooldown() { return false; }

    // Attack-related properties (placeholders)
    public float AttackRange => 0f;
    public int AttackDamage => 0;
    public float AttackSpeed => 0f;
    public string AttackAnimation => "";
    public AudioClip AttackSoundEffect => null;
    public GameObject AttackEffect => null;
}
