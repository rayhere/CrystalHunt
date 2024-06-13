using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneCubeSpawner : MonoBehaviour
{
    public StoneCubeController stoneCubePrefab; // Controller Script with Prefab
    public int numberOfSpawns = 10;
    public float spawnInterval = 5f;
    public float repeatInterval = 3f;

    // Controls cube objects and updates grid status
    public CoordinatePlane coordinatePlane; // Drag CoordinateMap
    public GridStat gridStat;

    // Enum for spawn directions
    public enum SpawnDirection
    {
        TopLeft_Horizontal,
        BottomLeft_Horizontal,
        TopRight_Horizontal,
        BottomRight_Horizontal,
        TopLeft_Vertical,
        BottomLeft_Vertical,
        TopRight_Vertical,
        BottomRight_Vertical
    }

    public SpawnDirection spawnDirection = SpawnDirection.TopLeft_Horizontal;

    private void Start()
    {
        if (coordinatePlane == null)
        {
            Debug.LogError("CoordinatePlane is not assigned to StoneCubeSpawner.");
            return;
        }
        if (stoneCubePrefab == null)
        {
            Debug.LogError("StoneCubePrefab is not assigned to StoneCubeSpawner.");
            return;
        }

        // Start the spawning process
        StartCoroutine(SpawnStoneCubes());
    }

    private IEnumerator SpawnStoneCubes()
    {
        int xAxisSize = coordinatePlane.GetXAxisSize();
        int zAxisSize = coordinatePlane.GetZAxisSize();
        int unitSize = coordinatePlane.GetUnitSize();
        int offset = unitSize / 2;
        if (xAxisSize % 2 > 0) offset = 0;

        int startX, startZ, endX, endZ, stepX, stepZ;
        SetSpawnParameters(out startX, out startZ, out endX, out endZ, out stepX, out stepZ);

        for (int i = 0; i < numberOfSpawns; i++)
        {
            for (int x = startX; x != endX; x += stepX)
            {
                for (int z = startZ; z != endZ; z += stepZ)
                {
                    if (coordinatePlane.IsWithinBounds(x, z) && coordinatePlane.IsEmpty(x, z) && (coordinatePlane.GetCheckoutTime(x, z) + 1f > Time.deltaTime))
                    {   
                        Debug.Log("SpawnStoneCubes(1): x is " + x + ", z is " + z);
                        StoneCubeController stoneCubeInstance = ObjectPooler.DequeueObject<StoneCubeController>("StoneCube");
                        if (stoneCubeInstance != null)
                        {
                            Debug.Log("SpawnStoneCubes(2): x is " + x + ", z is " + z);
                            stoneCubeInstance.Initialise(new Vector3(x * unitSize + offset, unitSize / 2, z * unitSize - offset));
                            coordinatePlane.SetGridUnitInfo(x, z, false, "", 0);

                            GridStat gridStat = stoneCubeInstance.GetComponent<GridStat>();
                            if (gridStat != null)
                            {
                                gridStat.x = x;
                                gridStat.z = z;
                            }
                            stoneCubeInstance.transform.SetParent(transform, false);
                            stoneCubeInstance.ActivateEmptyObject();
                            stoneCubeInstance.gameObject.SetActive(true);
                        }
                        yield return new WaitForSeconds(spawnInterval);
                    }
                    else Debug.Log("SpawnStoneCubes(3): x is " + x + ", z is " + z);
                }
            }
        }
        yield return new WaitForSeconds(repeatInterval);
        Debug.Log("StoneCubeSpawner Coroutine Stopped");
    }

    private void SetSpawnParameters(out int startX, out int startZ, out int endX, out int endZ, out int stepX, out int stepZ)
    {
        int xAxisSize = coordinatePlane.GetXAxisSize();
        int zAxisSize = coordinatePlane.GetZAxisSize();
        switch (spawnDirection)
        {
            case SpawnDirection.TopLeft_Horizontal:
                startX = -xAxisSize / 2;
                startZ = zAxisSize / 2;
                endX = xAxisSize / 2;
                endZ = -zAxisSize / 2;
                stepX = 1;
                stepZ = -1;
                break;
            case SpawnDirection.BottomLeft_Horizontal:
                startX = -xAxisSize / 2;
                startZ = -zAxisSize / 2;
                endX = xAxisSize / 2;
                endZ = zAxisSize / 2;
                stepX = 1;
                stepZ = 1;
                break;
            case SpawnDirection.TopRight_Horizontal:
                startX = xAxisSize / 2;
                startZ = zAxisSize / 2;
                endX = -xAxisSize / 2;
                endZ = -zAxisSize / 2;
                stepX = -1;
                stepZ = -1;
                break;
            case SpawnDirection.BottomRight_Horizontal:
                startX = xAxisSize / 2;
                startZ = -zAxisSize / 2;
                endX = -xAxisSize / 2;
                endZ = zAxisSize / 2;
                stepX = -1;
                stepZ = 1;
                break;
            case SpawnDirection.TopLeft_Vertical:
                startX = -xAxisSize / 2;
                startZ = zAxisSize / 2;
                endX = xAxisSize / 2;
                endZ = -zAxisSize / 2;
                stepX = -1;
                stepZ = -1;
                break;
            case SpawnDirection.BottomLeft_Vertical:
                startX = -xAxisSize / 2;
                startZ = -zAxisSize / 2;
                endX = xAxisSize / 2;
                endZ = zAxisSize / 2;
                stepX = -1;
                stepZ = 1;
                break;
            case SpawnDirection.TopRight_Vertical:
                startX = xAxisSize / 2;
                startZ = zAxisSize / 2;
                endX = -xAxisSize / 2;
                endZ = -zAxisSize / 2;
                stepX = 1;
                stepZ = -1;
                break;
            case SpawnDirection.BottomRight_Vertical:
                startX = xAxisSize / 2;
                startZ = -zAxisSize / 2;
                endX = -xAxisSize / 2;
                endZ = zAxisSize / 2;
                stepX = 1;
                stepZ = 1;
                break;
            default:
                startX = -xAxisSize / 2;
                startZ = zAxisSize / 2;
                endX = xAxisSize / 2;
                endZ = -zAxisSize / 2;
                stepX = 1;
                stepZ = -1;
                break;
        }
    }
}
