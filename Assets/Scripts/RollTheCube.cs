using System.Collections;
using UnityEngine;

public class RollTheCube : MonoBehaviour
{
    public float InputThreshold; // Threshold for input detection
    public float duration; // Duration of the roll animation
    bool isRolling = false; // Flag to check if the cube is currently rolling
    float cubeLength; // Length of the cube along its x-axis

    // Start is called before the first frame update
    void Start()
    {
        // Get the length of the cube along its x-axis
        cubeLength = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Get input values for horizontal and vertical axes
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // Check if the cube is not currently rolling and input exceeds the threshold
        if (!isRolling && ((Mathf.Abs(x) > InputThreshold) || (Mathf.Abs(y) > InputThreshold)))
        {
            // Set flag to indicate that the cube is rolling
            isRolling = true;
            // Start the coroutine to roll the cube
            StartCoroutine(RollingCube(x, y));
        }
    }

    IEnumerator RollingCube(float x, float y)
    {
        // Initialize variables for rolling animation
        float elapsed = 0.0f;
        Vector3 point = Vector3.zero;
        Vector3 axis = Vector3.zero;
        float angle = 0.0f;
        Vector3 direction = Vector3.zero;

        // Determine the axis of rotation and direction based on input
        if (Mathf.Abs(x) > Mathf.Abs(y)) // If input is primarily horizontal
        {
            axis = Vector3.forward;
            point = x > 0 ?
                transform.position + (Vector3.right * (cubeLength / 2)) :
                transform.position + (Vector3.left * (cubeLength / 2));
            angle = x > 0 ? -90 : 90;
            direction = x > 0 ? Vector3.right : Vector3.left;
        }
        else // If input is primarily vertical
        {
            axis = Vector3.right;
            point = y > 0 ?
                transform.position + (Vector3.forward * (cubeLength / 2)) :
                transform.position + (Vector3.back * (cubeLength / 2));
            angle = y > 0 ? 90 : -90;
            direction = y > 0 ? Vector3.forward : Vector3.back;
        }

        // Adjust the point of rotation to the bottom center of the cube
        point += new Vector3(0, -(cubeLength / 2), 0);
        // Calculate the position to adjust after the roll
        Vector3 adjustPos = point + direction * (cubeLength / 2) - new Vector3(0, -0.5f, 0);
        // Calculate the rotation to adjust after the roll
        Quaternion adjustRotation = Quaternion.Euler(direction * 90f);

        // Perform the rolling animation
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Rotate the cube around the point with the given axis and angle
            transform.RotateAround(
                point, axis, angle / duration * Time.deltaTime
            );

            yield return null;
        }

        // Adjust the position and rotation of the cube after the roll animation
        transform.position = adjustPos;
        transform.rotation = adjustRotation;

        // Reset the flag to indicate that the cube has finished rolling
        isRolling = false;
    }
}
