using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour, IDamageable
{
    public Animator Animator; // Animator for controlling attack animations
    public float attackRange = 1.5f; // Range of the attack
    public int attackDamage = 10; // Damage inflicted by the attack
    public float attackSpeed = 1.5f; // Speed of the attack animation
    public string attackAnimation = "Attack"; // Name of the attack animation trigger
    public AudioClip attackSoundEffect; // Sound effect for the attack
    public GameObject attackEffectPrefab; // Visual effect for the attack

    private Coroutine attackCoroutine; // Coroutine reference for attack animation

    public void PerformAttack()
    {
        if (attackCoroutine == null)
        {
            // Start the attack animation coroutine
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        // Trigger attack animation
        Animator.SetTrigger(attackAnimation);

        // Play attack sound effect
        if (attackSoundEffect != null)
        {
            AudioSource.PlayClipAtPoint(attackSoundEffect, transform.position);
        }

        // Instantiate attack visual effect
        if (attackEffectPrefab != null)
        {
            Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
        }

        // Wait for attack animation to finish
        yield return new WaitForSeconds(attackSpeed);

        // Reset attack coroutine
        attackCoroutine = null;
    }

    public void StartAttack()
    {
        PerformAttack();
    }

    public void EndAttack()
    {
        // End attack logic (if any)
    }

    public void TakeDamage(int damage)
    {
        // Implement damage logic here
        Debug.Log($"{gameObject.name} took {damage} damage.");
    }

    public bool IsAttacking()
    {
        return attackCoroutine != null;
    }

    public void CancelAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    public bool AttackCooldown()
    {
        // Implement cooldown logic if needed
        return attackCoroutine != null;
    }

    // Properties from IAttackable interface
    public float AttackRange => attackRange;

    public int AttackDamage => attackDamage;

    public float AttackSpeed => attackSpeed;

    public string AttackAnimation => attackAnimation;

    public AudioClip AttackSoundEffect => attackSoundEffect;

    public GameObject AttackEffect => attackEffectPrefab;

    // Implementing IDamageable interface method
    public Transform GetTransform()
    {
        return transform;
    }
}

