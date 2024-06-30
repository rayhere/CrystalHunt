using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSwitcher : MonoBehaviour
{
    public ClickToMove clickToMove; // Reference to ClickToMove script
    public WASDController wasdController; // Reference to WASDController script
    public Climbing climbing;
    public LedgeGrabbing ledgeGrabbing;
    public Sliding sliding;
    public WallRunningAdvanced wallRunningAdvanced;
    
    [SerializeField, Tooltip("mode 0 is wasd mode, mode 1 is click to move mode")]
    private int mode;

    private CursorLock cursorLock;
    private bool isCursorLocked; // Track the cursor lock state

    private void Awake()
    {
        // Initialize default mode and movement controllers
        mode = 0; // Assuming mode 0 is WASD mode
        wasdController.isActive = true;
        clickToMove.isActive = false;

        // Get reference to CursorLock script
        cursorLock = GetComponent<CursorLock>();

        // Initialize cursor lock state based on initial isActive state
        isCursorLocked = wasdController.isActive;
        UpdateCursorLockState();
    }

    private void Update()
    {
        // Toggle movement scripts on Tab key press
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMovementScripts();
        }
    }

    private void ToggleMovementScripts()
    {
        // Toggle isActive flag for WASDController and ClickToMove
        wasdController.isActive = !wasdController.isActive;
        clickToMove.isActive = !clickToMove.isActive;

        // Call toggle functions for other movement scripts based on your logic
        clickToMove.ToggleFunction();

        // Update cursor lock state after toggling
        isCursorLocked = wasdController.isActive;
        UpdateCursorLockState();

        // Example debug messages to verify toggle state
        Debug.Log("WASDController active: " + wasdController.isActive);
        Debug.Log("ClickToMove active: " + clickToMove.isActive);
    }

    private void UpdateCursorLockState()
    {
        // Set cursor lock state based on isActive flag
        if (isCursorLocked)
        {
            cursorLock.SetCursorLocked(true);
        }
        else
        {
            cursorLock.SetCursorLocked(false);
        }
    }
}
