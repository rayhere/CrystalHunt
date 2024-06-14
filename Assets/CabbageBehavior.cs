using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Enemy.cs
public class CabbageBehavior : MonoBehaviour, IDamageable
{
    public AttackRadius AttackRadius;
    public Animator Animator;
    public CabbageController cabbageController; // Movement
    public NavMeshAgent Agent;
    public CabbageScriptableObject CabbageScriptableObject;
    public int Health = 100;

    private Coroutine LookCoroutine;
    private const string ATTACK_TRIGGER = "Attack";

    private void Awake()
    {
        AttackRadius.OnAttack += OnAttack;
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

    public virtual void OnEnable()
    {
        SetupAgentFromConfiguration();
    }

    public void OnDisable()
    {
        // Return the cabbage to the object pool
        ObjectPooler.EnqueueObject(this, "Cabbage");
    }

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

        Health = CabbageScriptableObject.Health;

        (AttackRadius.Collider == null ? AttackRadius.GetComponent<SphereCollider>() : AttackRadius.Collider).radius = CabbageScriptableObject.AttackRadius;
        AttackRadius.AttackDelay = CabbageScriptableObject.AttackDelay;
        AttackRadius.Damage = CabbageScriptableObject.Damage;
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
}
