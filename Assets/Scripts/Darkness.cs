using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Darkness : MonoBehaviour, IDamageable
{
    public AttackRadius AttackRadius;
    public Animator Animator;
    public WASDController wasdController;
    public NavMeshAgent Agent;
    public DarknessStatsSO DarknessStatsSO;
    public int health;

    private Coroutine lookCoroutine;
    private const string ATTACK_TRIGGER = "Attack";

    private void Awake()
    {
        AttackRadius.OnAttack += OnAttack;
    }

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

    public void OnDisable()
    {
        if (Agent != null)
            Agent.enabled = false;
    }

    public virtual void SetupAgentFromConfiguration()
    {
        Agent.acceleration = DarknessStatsSO.Acceleration;
        Agent.angularSpeed = DarknessStatsSO.AngularSpeed;
        Agent.areaMask = DarknessStatsSO.AreaMask;
        Agent.avoidancePriority = DarknessStatsSO.AvoidancePriority;
        Agent.baseOffset = DarknessStatsSO.BaseOffset;
        Agent.height = DarknessStatsSO.Height;
        Agent.obstacleAvoidanceType = DarknessStatsSO.ObstacleAvoidanceType;
        Agent.radius = DarknessStatsSO.Radius;
        Agent.speed = DarknessStatsSO.Speed;
        Agent.stoppingDistance = DarknessStatsSO.StoppingDistance;
        
        //wasdController.UpdateRate = DarknessStatsSO.AIUpdateInterval;

        health = DarknessStatsSO.maxHP;

        (AttackRadius.Collider == null ? AttackRadius.GetComponent<SphereCollider>() : AttackRadius.Collider).radius = DarknessStatsSO.AttackRadius;
        AttackRadius.AttackDelay = DarknessStatsSO.AttackDelay;
        AttackRadius.Damage = DarknessStatsSO.Damage;
    }



    public void TakeDamage(int Damage)
    {
        health -= Damage;

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

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
