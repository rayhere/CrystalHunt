using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LockToGridV4 : MonoBehaviour
{
    [SerializeField] public float tileSize = 1f;
    [SerializeField] public Vector3 tileOffset = Vector3.zero;
    [SerializeField] private bool followParent = false;

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

            float halfTileSize = tileSize / 2f;

            float snappedX = Mathf.Round(currentPosition.x / halfTileSize) * halfTileSize + tileOffset.x;
            float snappedY = Mathf.Round(currentPosition.y / halfTileSize) * halfTileSize + tileOffset.y;
            float snappedZ = Mathf.Round(currentPosition.z / halfTileSize) * halfTileSize + tileOffset.z;

            Vector3 snappedPosition = new Vector3(snappedX, snappedY, snappedZ);
            transform.position = snappedPosition;
        }
    }
}
