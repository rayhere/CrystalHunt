using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLock : MonoBehaviour
{
    public bool cursorLocked;

    void Start()
    {
        // Initialize cursor lock state based on default value
        UpdateCursorLockState();
    }

    void UpdateCursorLockState()
    {
        Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !cursorLocked;
    }

    public void SetCursorLocked(bool locked)
    {
        cursorLocked = locked;
        UpdateCursorLockState(); // Update cursor state immediately when changed
    }
}