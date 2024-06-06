using System.Collections;
using UnityEngine;

public class CubeRoller : MonoBehaviour
{
    public Vector3 rollDirection = Vector3.forward; // Direction of the roll
    public float rollSpeed = 1.0f; // Speed of the roll
    public float rollDistance = 1.0f; // Distance to roll

    private bool isRolling = false; // Flag to track if the cube is currently rolling
    private Vector3 rollStartPosition; // Starting position of the roll

    // Update is called once per frame
    void Update()
    {
        // Check for input to start the roll
        if (!isRolling && Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Set the start position of the roll
            rollStartPosition = transform.position;
            // Start the roll coroutine
            StartCoroutine(RollCoroutine());
        }
    }

    IEnumerator RollCoroutine()
    {
        isRolling = true; // Set flag to indicate that the cube is rolling

        // Calculate the target position of the roll
        Vector3 targetPosition = rollStartPosition + rollDirection.normalized * rollDistance;
        float distanceCovered = 0.0f;

        // Perform the roll animation
        while (distanceCovered < rollDistance)
        {
            // Calculate the distance to move for this frame
            float step = rollSpeed * Time.deltaTime;
            // Move the cube towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            // Update the distance covered
            distanceCovered = Vector3.Distance(rollStartPosition, transform.position);
            // Wait for the next frame
            yield return null;
        }

        isRolling = false; // Reset flag when the roll is complete
    }
}
