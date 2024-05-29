using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LockToGridV3 : MonoBehaviour
{
    [SerializeField]
    public float tileSize = 0.5f;
    [SerializeField]
    public Vector3 tileOffset = Vector3.zero;
    [SerializeField]
    private bool followParent = false;

    private Vector3 previousPosition;

    void Start()
    {
        // Initialize the previous position
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!EditorApplication.isPlaying)
        {
            Vector3 currentPosition;

            if (followParent && transform.parent != null)
            {
                currentPosition = transform.parent.position;
            }
            else
            {
                currentPosition = transform.position;
            }

            // Calculate the change in position
            float deltaX = currentPosition.x - previousPosition.x;
            float deltaY = currentPosition.y - previousPosition.y;
            float deltaZ = currentPosition.z - previousPosition.z;

            // Check if the change exceeds the threshold (0.5)
            if (Mathf.Abs(deltaX) >= 0.5f || Mathf.Abs(deltaY) >= 0.5f || Mathf.Abs(deltaZ) >= 0.5f)
            {
                // Snap the position
                float snappedX = Mathf.Round(currentPosition.x / tileSize) * tileSize + tileOffset.x;
                float snappedY = Mathf.Round(currentPosition.y / tileSize) * tileSize + tileOffset.y;
                float snappedZ = Mathf.Round(currentPosition.z / tileSize) * tileSize + tileOffset.z;

                Vector3 snappedPosition = new Vector3(snappedX, snappedY, snappedZ);
                transform.position = snappedPosition;

                // Update the previous position
                previousPosition = snappedPosition;
            }
        }
    }
}
