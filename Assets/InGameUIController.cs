using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    public UIManager uiManager;

    void Start()
    {
        // Ensure UIManager reference is set either in Inspector or via script
        if (uiManager == null)
        {
            Debug.LogError("UIManager reference not set!");
            return;
        }

        // Example: Show in-game UI at start
        uiManager.ShowInGameUI();
    }

    void Update()
    {
        // Example: Switch UI to pause menu on key press (e.g., Escape key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiManager.ShowPauseMenuUI();
        }

        // Example: Switch UI to game over UI based on game condition (e.g., player dies)
        // Replace this condition with your actual game logic
        if (IsGameOverConditionMet())
        {
            uiManager.ShowGameOverUI();
        }
    }

    bool IsGameOverConditionMet()
    {
        // Example condition: player health reaches zero
        return false; // Replace with your actual game over condition
    }
}
