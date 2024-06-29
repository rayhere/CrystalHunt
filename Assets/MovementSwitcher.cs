using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementSwitcher : MonoBehaviour
{
    public ClickToMove clickToMove; // Reference to ClickToMove script
    public WASDController wasdController; // Reference to WASDController script
    public Climbing climbing;
    public LedgeGrabbing ledgeGrabbing;
    public Sliding sliding;
    public WallRunningAdvanced wallRunningAdvanced;
    [SerializeField, Tooltip("mode 0 is wasd mode, mode 1 is click to move mode")]
    public int mode;

    private void Awake()
    {
        mode = 0;
        wasdController.isActive = true;
        clickToMove.isActive = false;
        /* wasdController.enabled = true;
        clickToMove.enabled = false; */

        // Toggle between WASDController and ClickToMove
            /* if (wasdController.enabled)
            {
                wasdController.enabled = false;
                clickToMove.enabled = true;
                Debug.Log("Switched to ClickToMove");
            }
            else
            {
                wasdController.enabled = true;
                clickToMove.enabled = false;
                Debug.Log("Switched to WASDController");
            } */
    }

    private void Update()
    {
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
        
        //Better do it in ClickToMove.cs
        //clickToMove.GetComponent<NavMeshAgent>().ResetPath(); // Stop NavMeshAgent from moving
        // Enable or disable NavMeshAgent based on isActive in ClickToMove
        //clickToMove.GetComponent<NavMeshAgent>().enabled = clickToMove.isActive;
     
        clickToMove.ToggleFunction();

        Debug.Log("WASDController active: " + wasdController.isActive);
        Debug.Log("ClickToMove active: " + clickToMove.isActive);
    }
}