using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    // Reference to the cabbage controller
    private CabbageController cabbageController;

    // Health Points (HP) of the enemy
    public int maxHP = 100; // Maximum HP
    public int currentHP; // Current HP

    // Other commonly used variables
    public float moveSpeed = 3f; // Movement speed of the enemy
    public int attackDamage = 10; // Damage inflicted by enemy's attacks
    public float attackRange = 2f; // Range of enemy's attack
    public float detectionRange = 10f; // Range for detecting the player
    public float attackCooldown = 2f; // Cooldown between attacks
    private float lastAttackTime; // Time when the last attack occurred

    // Method to initialize the enemy's HP
    void Start()
    {
        currentHP = maxHP;
    }

    // Method to handle taking damage
    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        
        // Check if the enemy's HP has dropped to or below zero
        if (currentHP <= 0)
        {
            // Call Die() method of CabbageController when HP is zero or below
            //cabbageController.Die();
        }
    }
    // Other methods and behaviors for enemy movement, attacking, etc. can be added here
}
