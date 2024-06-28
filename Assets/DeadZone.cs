using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour
{
    [Header("References")]
    public DarknessStatsSO playerStats; // Reference to the ScriptableObject

    // Define the tag for the player GameObject (adjust as per your player setup)
    public string playerTag = "Player";
    public string DarknessTag = "Darkness";
    
    // Name of the scene to load upon triggering the dead zone
    public string endGameScene = "EndGameScene";

    // This method is called when another collider enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering collider is tagged as "Player"
        if (other.CompareTag(playerTag))
        {
            // Load the end game scene
            //SceneManager.LoadScene(endGameScene);

            Debug.Log("DeadZone playerTag detected: playerStats.currentHP is " + playerStats.currentHP);
            playerStats.currentHP = 0; // Set currentHP to 0 directly
            //playerStats.TakeDamage(999); // Call TakeDamage with a large amount or however much you want to reduce
            Debug.Log("DeadZone playerTag detected: playerStats.currentHP is " + playerStats.currentHP);
        }

        if (other.CompareTag(DarknessTag))
        {
            // Load the end game scene
            //SceneManager.LoadScene(endGameScene);
            Debug.Log("DeadZone DarknessTag detected: playerStats.currentHP is " + playerStats.currentHP);
            playerStats.currentHP = 0; // Set currentHP to 0 directly
            //playerStats.TakeDamage(999); // Call TakeDamage with a large amount or however much you want to reduce
            Debug.Log("DeadZone DarknessTag detected: playerStats.currentHP is " + playerStats.currentHP);
        }

    }
}
