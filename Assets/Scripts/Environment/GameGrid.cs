using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] private bool useZAxisForGrid = true;
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private Vector3 startingPosition = Vector3.zero;
    private GameObject[,] gameGrid;
    private bool isGridCreated = false;
    private int height = 11;
    private int width = 11;
    private float gridSpaceSize = 10f;

    void Start()
    {
        StartCoroutine(CreateGrid());
    }

    private IEnumerator CreateGrid()
    {
        gameGrid = new GameObject[height, width];

        if (gridCellPrefab == null)
        {
            Debug.LogError("ERROR: Grid Cell Prefab on the Game grid is not assigned");
            yield break; // Exit the coroutine early if prefab is not assigned
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float posX = startingPosition.x + x * gridSpaceSize;
                float posY = useZAxisForGrid ? startingPosition.y : startingPosition.y + y * gridSpaceSize;
                float posZ = useZAxisForGrid ? startingPosition.z + y * gridSpaceSize : startingPosition.z;

                gameGrid[x, y] = Instantiate(gridCellPrefab, new Vector3(posX, posY, posZ), Quaternion.identity);
                gameGrid[x, y].GetComponent<GridCell>().SetPosition(x, y);
                gameGrid[x, y].transform.parent = transform;
                gameGrid[x, y].gameObject.name = "Grid Space (X: " + x.ToString() + ", Y: " + y.ToString() + ")";

                yield return null;
            }
        }

        isGridCreated = true; // Set flag to indicate grid creation is complete
    }

    public Vector2Int GetGridPosFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - startingPosition.x) / gridSpaceSize);
        int y = Mathf.FloorToInt((worldPosition.z - startingPosition.z) / gridSpaceSize);

        x = Mathf.Clamp(x, 0, width);
        y = Mathf.Clamp(y, 0, height);

        return new Vector2Int(x, y);
    }

    public Vector3 GetWorldPosFromGridPos(Vector2Int gridPos)
    {
        float x = startingPosition.x + gridPos.x * gridSpaceSize;
        float y = useZAxisForGrid ? startingPosition.y : startingPosition.y + gridPos.y * gridSpaceSize;
        float z = useZAxisForGrid ? startingPosition.z + gridPos.y * gridSpaceSize : startingPosition.z;

        return new Vector3(x, y, z);
    }

    // Example of stopping the coroutine from external code
    public void StopGridCreation()
    {
        StopCoroutine("CreateGrid");
    }
}
