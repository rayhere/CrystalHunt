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
        //base.OnDisable();
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

        Health = DarknessStatsSO.Health;

        (AttackRadius.Collider == null ? AttackRadius.GetComponent<SphereCollider>() : AttackRadius.Collider).radius = DarknessStatsSO.AttackRadius;
        AttackRadius.AttackDelay = DarknessStatsSO.AttackDelay;
        AttackRadius.Damage = DarknessStatsSO.Damage;
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
