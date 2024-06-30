using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MovementStateAnimator : MonoBehaviour
{
    public Animator animator;
    private WASDController wasdController;
    private ClickToMove clickToMove;

    public enum PerformState
    {
        isStandingIdle,
        isWalking,
        isSprinting,
        isCrouching,
        isJumping,
        isFalling,
        isAboutLanding,
        isLandedOnGround,
        isGrounded,
        isCrouchedWalking,
        isCrouchingIdle,
        isSliding
        // Add more states as needed
    }

    public PerformState currentState;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
        //if (animator == null)
        //Debug.LogError("MovementStateAnimator animator is null");
        wasdController = GetComponent<WASDController>(); // Assuming WASDController is attached to the same GameObject
        clickToMove = GetComponent<ClickToMove>(); // Assuming ClickToMove is attached to the same GameObject
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        PerformState newState;
        if (wasdController.isActive)
        {
            newState = (PerformState)wasdController.performState;
        }
        else if (clickToMove.isActive)
        {
            newState = (PerformState)clickToMove.performState;
        }
        else
        {
            newState = PerformState.isStandingIdle;
        }

        if (newState != currentState)
        {
            SetBool(GetParameterName(currentState), false);
            UpdateAnimatorState(newState);
            currentState = newState;
        }
    }

    private void UpdateAnimatorState(PerformState newState)
    {
        Debug.Log("UpdateAnimatorState "+GetParameterName(newState));
        SetBool(GetParameterName(newState), true);
    }

    private void SetBool(string parameterName, bool value)
    {
        animator.SetBool(parameterName, value);
    }

    private string GetParameterName(PerformState state)
    {
        return state.ToString();
    }
}
