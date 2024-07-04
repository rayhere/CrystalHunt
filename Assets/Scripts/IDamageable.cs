using UnityEngine;

/// <summary>
/// A class should extend MonoBehaviour and implement this interface, then implement what should happen when that object takes damage.
/// </summary>
public interface IDamageable
{
    /* If characters can receive damage as a result of an attack, you might include a method to handle the damage calculation and response. */
    void TakeDamage(int Damage);
    Transform GetTransform();

    void PerformAttack();
    // Add other attack-related methods or properties as needed

    /* These methods can be used to signify the beginning and end of an attack sequence. They can be useful for coordinating animations, sound effects, or other visual/audio cues related to attacks. */
    void StartAttack();
    void EndAttack();

    /* A method to check if the character is currently in an attacking state. This can be helpful for AI or other systems to decide on behavior based on the character's current action. */
    bool IsAttacking();

    /* In cases where attacks can be interrupted or canceled (e.g., by player input or external events), provide a method to stop the current attack sequence. */
    void CancelAttack();

    /* If attacks have a cooldown period between uses, you could define a method to manage and check the cooldown status. */
    bool AttackCooldown();

    // Additional Attack-Related Properties

    float AttackRange { get; } // Define a property to specify the range or distance over which the attack can reach.

    int AttackDamage { get; } // Property to determine the amount of damage inflicted by the attack.
    float AttackSpeed { get; } // If attack speed or frequency is important, expose a property to control or indicate the attack speed.

    string AttackAnimation { get; } // If your characters have different attack animations, you could expose a property to manage which animation to play during attacks.


    AudioClip AttackSoundEffect { get; } // Property to specify the sound effect associated with the attack.

    GameObject AttackEffect { get; } // For visual effects related to attacks (e.g., particle effects), you might include a property to handle these.
}
