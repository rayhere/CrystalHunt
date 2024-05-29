using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LockToGridV5 : MonoBehaviour
{
    [SerializeField] public float positionTileSize = 1f;
    [SerializeField] public float scaleTileSize = 0.5f;
    [SerializeField] public Vector3 tileOffset = Vector3.zero;
    [SerializeField] private bool followParent = false;

    [SerializeField] private bool snapPosition = true;
    [SerializeField] private bool snapScale = true;

    private Vector3 lastPosition;
    private Vector3 lastScale;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
        lastScale = transform.localScale;
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

            Vector3 currentScale = transform.localScale;
            Vector3 positionDelta = currentPosition - lastPosition;
            Vector3 scaleDelta = currentScale - lastScale;

            if (snapPosition && (Mathf.Abs(positionDelta.x) >= positionTileSize || Mathf.Abs(positionDelta.y) >= positionTileSize || Mathf.Abs(positionDelta.z) >= positionTileSize))
            {
                // Calculate snapped position
                float snappedX = Mathf.Round((currentPosition.x - tileOffset.x) / positionTileSize) * positionTileSize + tileOffset.x;
                float snappedY = Mathf.Round((currentPosition.y - tileOffset.y) / positionTileSize) * positionTileSize + tileOffset.y;
                float snappedZ = Mathf.Round((currentPosition.z - tileOffset.z) / positionTileSize) * positionTileSize + tileOffset.z;

                if (followParent)
                {
                    // Adjust for parent scale
                    snappedX += Mathf.Round(scaleDelta.x / positionTileSize) * positionTileSize;
                    snappedY += Mathf.Round(scaleDelta.y / positionTileSize) * positionTileSize;
                    snappedZ += Mathf.Round(scaleDelta.z / positionTileSize) * positionTileSize;
                }

                // Adjust snapped position if offset is -0.5
                if (tileOffset.x == -0.5f && snappedX < -0.25f)
                {
                    snappedX = 0;
                }

                // Update snapped position
                Vector3 snappedPosition = new Vector3(snappedX, snappedY, snappedZ);
                transform.position = snappedPosition;

                lastPosition = transform.position;
            }

            if (snapScale && (Mathf.Abs(scaleDelta.x) >= scaleTileSize || Mathf.Abs(scaleDelta.y) >= scaleTileSize || Mathf.Abs(scaleDelta.z) >= scaleTileSize))
            {
                // Calculate snapped scale
                float snappedXScale = Mathf.Round(currentScale.x / scaleTileSize) * scaleTileSize;
                float snappedYScale = Mathf.Round(currentScale.y / scaleTileSize) * scaleTileSize;
                float snappedZScale = Mathf.Round(currentScale.z / scaleTileSize) * scaleTileSize;

                // Update snapped scale
                Vector3 snappedScale = new Vector3(snappedXScale, snappedYScale, snappedZScale);
                transform.localScale = snappedScale;

                lastScale = currentScale;
            }
        }
    }
}
