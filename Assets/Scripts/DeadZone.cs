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
    
    public LayerMask whatIsHuman; // Layer mask for identifying human objects

    private MeshRenderer meshRenderer; // Reference to this mesh renderer
    private Coroutine hpReductionCoroutine; // Coroutine reference for reducing HP

    // Name of the scene to load upon triggering the dead zone
    public string endGameScene = "EndGameScene";

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>(); // Assuming the DeadZone itself has a MeshRenderer
        meshRenderer.enabled = false; // Start with the mesh renderer disabled
    }

    // This method is called when another collider enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering collider is tagged as "Player"
        if (other.CompareTag(playerTag))
        {
            GameObject player = other.gameObject;

            // Check if the player is in the whatIsHuman layer
            if (((1 << player.layer) & whatIsHuman) != 0)
            {
                // Show the DeadZone's mesh renderer
                meshRenderer.enabled = true;

                // Start the delay before reducing HP only if not already reducing
                if (hpReductionCoroutine == null)
                {
                    StartCoroutine(StartHPReductionDelay());
                }
            }
            Debug.Log("DeadZone playerTag detected: playerStats.currentHP is " + playerStats.currentHP);
        }

        // Check if the entering collider is tagged as "Darkness"
        if (other.CompareTag(DarknessTag))
        {
            GameObject darkness = other.gameObject;

            // Check if the Darkness is in the whatIsHuman layer
            if (((1 << darkness.layer) & whatIsHuman) != 0)
            {
                // Show the DeadZone's mesh renderer
                meshRenderer.enabled = true;
                
                // Start the delay before reducing HP only if not already reducing
                if (hpReductionCoroutine == null)
                {
                    StartCoroutine(StartHPReductionDelay());
                }
            }
            Debug.Log("DeadZone DarknessTag detected: playerStats.currentHP is " + playerStats.currentHP);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the exiting collider is tagged as "Player"
        if (other.CompareTag(playerTag))
        {
            GameObject player = other.gameObject;

            // Check if the player is in the whatIsHuman layer
            if (((1 << player.layer) & whatIsHuman) != 0)
            {
                // Hide the DeadZone's mesh renderer
                meshRenderer.enabled = false;

                // Stop reducing HP
                if (hpReductionCoroutine != null)
                {
                    StopCoroutine(hpReductionCoroutine);
                    hpReductionCoroutine = null;
                }
            }
        }

        // Check if the exiting collider is tagged as "Darkness"
        if (other.CompareTag(DarknessTag))
        {
            GameObject darkness = other.gameObject;

            // Check if the Darkness is in the whatIsHuman layer
            if (((1 << darkness.layer) & whatIsHuman) != 0)
            {
                // Stop reducing HP
                if (hpReductionCoroutine != null)
                {
                    StopCoroutine(hpReductionCoroutine);
                    hpReductionCoroutine = null;
                }
            }
        }
    }

    private IEnumerator StartHPReductionDelay()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds

        // Start reducing HP using a coroutine
        hpReductionCoroutine = StartCoroutine(ReduceHPOverTime());
    }

    private IEnumerator ReduceHPOverTime()
    {
        while (playerStats.currentHP > 0)
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second

            // Reduce currentHP by 10
            playerStats.currentHP -= 10;

            // Clamp currentHP to ensure it doesn't go below 0
            playerStats.currentHP = Mathf.Max(playerStats.currentHP, 0);

            Debug.Log("Current HP: " + playerStats.currentHP);
        }
    }
}
