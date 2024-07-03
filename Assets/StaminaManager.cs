using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    [Header("References")]
    public DarknessStatsSO playerStats;
    public WASDController pm;

    public bool isSprinting;
    public bool isClimbing;
    public bool isSliding;
    public bool isJumping;

    private float sprintTimer;
    private float climbTimer;
    private float slideTimer;
    private float jumpTimer;

    private bool hasStartedSliding = false;

    private void Start()
    {
        pm = GetComponent<WASDController>();
        // Initialize timers
        sprintTimer = 0f;
        climbTimer = 0f;
        slideTimer = 0f;
        jumpTimer = 0f;
    }

    private void Update()
    {
        // Check for actions that consume stamina
        HandleSprint();
        HandleClimb();
        HandleSlide();
        HandleJump();

        // Recover stamina over time
        RecoverStamina();
    }

    private void HandleSprint()
    {
        isSprinting = pm.sprinting;
        if (isSprinting)
        {
            if (playerStats.currentSP >= playerStats.sprintSPCost)
            {   
                /* if (Time.time < sprintTimer) return;
                // perform every one sec
                sprintTimer = Time.time + 1; 
                playerStats.currentSP -= playerStats.sprintSPCost; */
                
                // Perform sprinting action here

                playerStats.currentSP -= (playerStats.sprintSPCost * Time.deltaTime);
            }
            else
            {
                isSprinting = false; // Stop sprinting if not enough stamina
            }
        }
        else
        {
            sprintTimer = 0f; // Reset sprint timer if not sprinting
        }
    }

    private void HandleClimb()
    {
        if (isClimbing)
        {
            if (playerStats.currentSP >= playerStats.climbSPCost)
            {
                playerStats.currentSP -= playerStats.climbSPCost;
                climbTimer += Time.deltaTime;
                // Perform climbing action here
            }
            else
            {
                isClimbing = false; // Stop climbing if not enough stamina
            }
        }
        else
        {
            climbTimer = 0f; // Reset climb timer if not climbing
        }
    }

    private void HandleSlide()
    {
        //isSliding = pm.sliding;
        if (isSliding && !hasStartedSliding)
        {
            if (playerStats.currentSP >= playerStats.slideSPCost)
            {
                playerStats.currentSP -= playerStats.slideSPCost;
                // Perform sliding action here

                hasStartedSliding = true; // Set flag to true to indicate sliding has started
            }
            else
            {
                isSliding = false; // Stop sliding if not enough stamina
            }
        }
        else if (!isSliding)
        {
            hasStartedSliding = false; // Reset flag when not sliding
        }
    }

    private void HandleJump()
    {
        if (isJumping)
        {
            if (playerStats.currentSP >= playerStats.jumpSPCost)
            {
                playerStats.currentSP -= playerStats.jumpSPCost;
                jumpTimer += Time.deltaTime;
                // Perform jumping action here

                isJumping = false; // jump performed
            }
            else
            {
                isJumping = false; // Stop jumping if not enough stamina
            }
        }
        else
        {
            jumpTimer = 0f; // Reset jump timer if not jumping
        }
    }

    private void RecoverStamina()
    {
        if (!isSprinting && !isClimbing && !isSliding && !isJumping)
        {
            if (playerStats.currentSP < playerStats.maxSP)
            {
                playerStats.currentSP += playerStats.sPRecoveryRate * Time.deltaTime;
                playerStats.currentSP = Mathf.Clamp(playerStats.currentSP, 0f, playerStats.maxSP);
            }
        }
    }

    // Methods to start and stop actions, called from your player controller or input manager
    public void StartSprint()
    {
        isSprinting = true;
    }

    public void StartClimb()
    {
        isClimbing = true;
    }

    public void StartSlide()
    {
        isSliding = true;
    }

    public void StartJump()
    {
        isJumping = true;
    }

    public void StopSprint()
    {
        isSprinting = false;
    }

    public void StopClimb()
    {
        isClimbing = false;
    }

    public void StopSlide()
    {
        isSliding = false;
    }

    public void StopJump()
    {
        isJumping = false;
    }
}
