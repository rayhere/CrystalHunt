using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneCubeSpawner : MonoBehaviour
{
    public StoneCubeController stoneCubePrefab; // Controller Script with Prefab
    [SerializeField]
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

        
        Debug.Log(" xAxisSize % 2 > 0 is "+ (xAxisSize % 2 > 0) + "xAxisSize % 2 is " + (xAxisSize % 2) + "  xAxisSize is " + xAxisSize);

        int startX, startZ, endX, endZ, stepX, stepZ;
        bool horizontal;
        SetSpawnParameters(out startX, out startZ, out endX, out endZ, out stepX, out stepZ, out horizontal);
        int remainder = xAxisSize % 2 > 0 ? 0 : 1;
        startZ -= remainder;
        endX -= remainder;
        //for (int i = 0; i < numberOfSpawns; i++)
        int i = 0;
        if (horizontal)
        {
            int z = startZ;
            for (int x = startX; x != endX; x += stepX)
            {
                if (i >= numberOfSpawns) 
                {
                    Debug.Log("StoneCubeSpawner Coroutine Stopped");
                    yield break; // Exit the coroutine gracefully
                }

                Debug.Log(" x is " + x + " z is " + z + " i is " + i + " numberOfSpawns is " + numberOfSpawns);
                Debug.Log("SpawnStoneCubes");
                if (coordinatePlane.IsWithinBounds(x, z) && coordinatePlane.IsEmpty(x, z) && (coordinatePlane.GetCheckoutTime(x, z) + 1f > Time.deltaTime))
                {
                    Debug.Log("SpawnStoneCubes SpawnStoneCube x is " + x + " z is "+ z + " offset is " +offset + " i is " +i);
                    SpawnStoneCubes(x, z, unitSize, offset);
                    i++;
                    yield return new WaitForSeconds(spawnInterval);
                }
                else
                {
                    Debug.LogError("coordinatePlane.IsWithinBounds(x, z)" + coordinatePlane.IsWithinBounds(x, z) + " coordinatePlane.IsEmpty(x, z) is " +coordinatePlane.IsEmpty(x, z) + "(coordinatePlane.GetCheckoutTime(x, z) + 1f > Time.deltaTime) is " + (coordinatePlane.GetCheckoutTime(x, z) + 1f > Time.deltaTime) + "x is " + x + " z is "+ z);

                    StartCoroutine(LateSpawnStoneCubes(x, z, unitSize, offset));
                    i++;
                }
                //yield return new WaitForSeconds(spawnInterval);
            }
        }
        else
        {
            int x = startX;
            for (int z = startZ; z != endZ; z += stepZ)
            {
                /* if (coordinatePlane.IsWithinBounds(x, z) && coordinatePlane.IsEmpty(x, z) && (coordinatePlane.GetCheckoutTime(x, z) + 1f > Time.deltaTime))
                {
                    SpawnStoneCubes(x, z, unitSize, offset);
                } */
                if (i >= numberOfSpawns) 
                {
                    Debug.Log("StoneCubeSpawner Coroutine Stopped");
                    yield break; // Exit the coroutine gracefully
                }

                Debug.Log(" x is " + x + " z is " + z + " i is " + i + " numberOfSpawns is " + numberOfSpawns);
                Debug.Log("SpawnStoneCubes");
                if (coordinatePlane.IsWithinBounds(x, z) && coordinatePlane.IsEmpty(x, z) && (coordinatePlane.GetCheckoutTime(x, z) + 1f > Time.deltaTime))
                {
                    Debug.Log("SpawnStoneCubes SpawnStoneCube x is " + x + " z is "+ z + " offset is " +offset + " i is " +i);
                    SpawnStoneCubes(x, z, unitSize, offset);
                    i++;
                    yield return new WaitForSeconds(spawnInterval);
                }
                else
                {
                    Debug.LogError("coordinatePlane.IsWithinBounds(x, z)" + coordinatePlane.IsWithinBounds(x, z) + " coordinatePlane.IsEmpty(x, z) is " +coordinatePlane.IsEmpty(x, z) + "(coordinatePlane.GetCheckoutTime(x, z) + 1f > Time.deltaTime) is " + (coordinatePlane.GetCheckoutTime(x, z) + 1f > Time.deltaTime) + "x is " + x + " z is "+ z);

                    StartCoroutine(LateSpawnStoneCubes(x, z, unitSize, offset));
                    i++;
                }
            }
        }     
        
        yield return new WaitForSeconds(repeatInterval);
        // Here will keep spawning until pool is empty
    }

    private IEnumerator LateSpawnStoneCubes(int x, int z, int unitSize, int offset)
    {
        bool spawned = false;
        while (spawned == false)
        {
            if (coordinatePlane.IsWithinBounds(x, z) && coordinatePlane.IsEmpty(x, z) && (coordinatePlane.GetCheckoutTime(x, z) + 1f > Time.deltaTime))
            {
                Debug.Log("LateSpawnStoneCubes SpawnStoneCube x is " + x + " z is "+ z + " offset is " +offset);
                SpawnStoneCubes(x, z, unitSize, offset);

                spawned = true;
            }
            else
            {
                Debug.LogError("coordinatePlane.IsWithinBounds(x, z)" + coordinatePlane.IsWithinBounds(x, z) + " coordinatePlane.IsEmpty(x, z) is " +coordinatePlane.IsEmpty(x, z) + "(coordinatePlane.GetCheckoutTime(x, z) + 1f > Time.deltaTime) is " + (coordinatePlane.GetCheckoutTime(x, z) + 1f > Time.deltaTime) + "x is " + x + " z is "+ z);

                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    private void SpawnStoneCubes(int x, int z, int unitSize, int offset)
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
            //stoneCubeInstance.transform.SetParent(transform, false);
            stoneCubeInstance.ActivateEmptyObject();
            stoneCubeInstance.gameObject.SetActive(true);
        }
        
    }

    private void SetSpawnParameters(out int startX, out int startZ, out int endX, out int endZ, out int stepX, out int stepZ, out bool horizontal)
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
                horizontal = true;
                break;
            case SpawnDirection.BottomLeft_Horizontal:
                startX = -xAxisSize / 2;
                startZ = -zAxisSize / 2;
                endX = xAxisSize / 2;
                endZ = zAxisSize / 2;
                stepX = 1;
                stepZ = 1;
                horizontal = true;
                break;
            case SpawnDirection.TopRight_Horizontal:
                startX = xAxisSize / 2;
                startZ = zAxisSize / 2;
                endX = -xAxisSize / 2;
                endZ = -zAxisSize / 2;
                stepX = -1;
                stepZ = -1;
                horizontal = true;
                break;
            case SpawnDirection.BottomRight_Horizontal:
                startX = xAxisSize / 2;
                startZ = -zAxisSize / 2;
                endX = -xAxisSize / 2;
                endZ = zAxisSize / 2;
                stepX = -1;
                stepZ = 1;
                horizontal = true;
                break;
            case SpawnDirection.TopLeft_Vertical:
                startX = -xAxisSize / 2;
                startZ = zAxisSize / 2;
                endX = xAxisSize / 2;
                endZ = -zAxisSize / 2;
                stepX = -1;
                stepZ = -1;
                horizontal = false;
                break;
            case SpawnDirection.BottomLeft_Vertical:
                startX = -xAxisSize / 2;
                startZ = -zAxisSize / 2;
                endX = xAxisSize / 2;
                endZ = zAxisSize / 2;
                stepX = -1;
                stepZ = 1;
                horizontal = false;
                break;
            case SpawnDirection.TopRight_Vertical:
                startX = xAxisSize / 2;
                startZ = zAxisSize / 2;
                endX = -xAxisSize / 2;
                endZ = -zAxisSize / 2;
                stepX = 1;
                stepZ = -1;
                horizontal = false;
                break;
            case SpawnDirection.BottomRight_Vertical:
                startX = xAxisSize / 2;
                startZ = -zAxisSize / 2;
                endX = -xAxisSize / 2;
                endZ = zAxisSize / 2;
                stepX = 1;
                stepZ = 1;
                horizontal = false;
                break;
            default:
                startX = -xAxisSize / 2;
                startZ = zAxisSize / 2;
                endX = xAxisSize / 2;
                endZ = -zAxisSize / 2;
                stepX = 1;
                stepZ = -1;
                horizontal = false;
                break;
        }
    }
}
