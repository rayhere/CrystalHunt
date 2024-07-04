using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour, IDamageable
{
    // CombatCharacter properties
    public int MaxHealth = 100;
    private int currentHealth;

    // Attack properties
    public float AttackRange  = 1.5f; // Range of the attack
    public int AttackDamage  = 10; // Damage inflicted by the attack
    public float AttackSpeed  = 1.5f; // Speed of the attack animation
    public string AttackAnimation  = "Attack"; // Name of the attack animation trigger
    public AudioClip AttackSoundEffect; // Sound effect for the attack
    public GameObject AttackEffectPrefab; // Visual effect for the attack


    // Animator reference
    public Animator Animator; // Animator for controlling attack animations

    private Coroutine attackCoroutine; // Coroutine reference for attack animation

    private void Start()
    {
        currentHealth = MaxHealth;
    }

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
        Animator.SetTrigger(AttackAnimation);

        // Play attack sound effect
        if (AttackSoundEffect != null)
        {
            AudioSource.PlayClipAtPoint(AttackSoundEffect, transform.position);
        }

        // Instantiate attack visual effect
        if (AttackEffectPrefab != null)
        {
            Instantiate(AttackEffectPrefab, transform.position, Quaternion.identity);
        }

        // Wait for attack animation to finish
        yield return new WaitForSeconds(AttackSpeed);

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
        currentHealth -= damage;

        // Example: Play hurt animation or effects
        if (Animator != null)
        {
            Animator.SetTrigger("Hurt");
        }

        // Example: Check if character is defeated
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has been defeated!");
        gameObject.SetActive(false); // Disable character object or handle death state
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

    // Implementing IDamageable interface properties
    float IDamageable.AttackRange => AttackRange;

    int IDamageable.AttackDamage => AttackDamage;

    float IDamageable.AttackSpeed => AttackSpeed;

    string IDamageable.AttackAnimation => AttackAnimation;

    AudioClip IDamageable.AttackSoundEffect => AttackSoundEffect;

    GameObject IDamageable.AttackEffect => AttackEffectPrefab;

    // Implementing IDamageable interface method
    public Transform GetTransform()
    {
        return transform;
    }
}

