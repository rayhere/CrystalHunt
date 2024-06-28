using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour
{
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
            SceneManager.LoadScene(endGameScene);
        }

        if (other.CompareTag(DarknessTag))
        {
            // Load the end game scene
            SceneManager.LoadScene(endGameScene);
        }
    }
}
