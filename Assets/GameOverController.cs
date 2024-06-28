using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverController : MonoBehaviour
{
    // Reference to the UI document
    public VisualTreeAsset gameOverUI;

    // Start is called before the first frame update
    void Start()
    {
        ShowGameOverUI();
    }

    // Method to show the game over UI
    void ShowGameOverUI()
    {
        // Load the UI document
        var uiInstance = gameOverUI.CloneTree();

        // Attach the UI to the root visual element of the panel or canvas
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Add(uiInstance);

        // Example: Handle restart button click
        var restartButton = uiInstance.Q<Button>("restartButton");
        if (restartButton != null)
        {
            restartButton.clicked += () =>
            {
                // Replace with your logic to restart the game or load main menu
                Debug.Log("Restart button clicked");
            };
        }
    }
}
