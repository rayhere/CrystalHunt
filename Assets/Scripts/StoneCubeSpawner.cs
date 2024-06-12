using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* a script that spawns a stone cube every 1 second, repeated 3 times with a 3-second delay between each repetition */

public class StoneCubeSpawner : MonoBehaviour
{
    public StoneCubeController stoneCubePrefab; // Controller Script that with Prefab
    public int numberOfSpawns = 10;
    public float spawnInterval = 5f;
    public float repeatInterval = 3f;

    // Controls cube objects and updates grid status
    public CoordinatePlane coordinatePlane; // Drag CoordinateMap
    public GridStat gridStat;

    private void Start()
    {
        if(coordinatePlane == null)
        {
            Debug.LogError("CoordinatePlane is not assigned to StoneCubeSpawner.");
            return;
        }
        if(stoneCubePrefab == null)
        {
            Debug.LogError("StoneCubePrefab is not assigned to StoneCubeSpawner.");
            return;
        }


        // Start the spawning process
        int i = numberOfSpawns;
        StartCoroutine(SpawnStoneCubes());
    }

    private IEnumerator SpawnStoneCubes()
    {
        // Repeat spawning process for the specified number of times
        //int i = numberOfSpawns;

        // CooridinateMap [x,z]
        int x = 0;
        int z = 0; // it is on axis z
        int height = 5; // offset pos.y for cube
        int xAxisSize = coordinatePlane.GetXAxisSize();
        int zAxisSize = coordinatePlane.GetZAxisSize();
        int unitSize = coordinatePlane.GetUnitSize(); // pre grid

        // The coordinate plane is divided into four quadrants
        // Quadrant 1 is in the top right.
        int[] topRight = new int[2] { xAxisSize / 2, zAxisSize / 2 };

        // Quadrant 2 is in the top left.
        int[] topLeft = new int[2] { -xAxisSize / 2, zAxisSize / 2 };

        // Quadrant 3 is in the bottom left.
        int[] bottomLeft = new int[2] { -xAxisSize / 2, -zAxisSize / 2 };
        
        // Quadrant 4 is in the bottom right.
        int[] bottomRight = new int[2] { xAxisSize / 2, -zAxisSize / 2 };

        //int xTopRight = topRight[0];
        //int yTopRight = topRight[1];
        int xTopLeft = topLeft[0];
        Debug.Log("xTopLeft is " + xTopLeft);
        int zTopLeft = topLeft[1];
        Debug.Log("zTopLeft is " + zTopLeft);

        //while (i > 0)

        for (int i = 0; i < numberOfSpawns; i++)
        {
            // Spawn a stone cube
            //SpawnStoneCube();
            StoneCubeController stoneCubeInstance = ObjectPooler.DequeueObject<StoneCubeController>("StoneCube");
            Debug.Log("stoneCubeInstance Spawned");
            if(stoneCubeInstance != null)
            {
                stoneCubeInstance.transform.SetParent(transform, false);

                // stoneCubeInstance.Initialise(new Vector3(-50 + i*10, 5, 40));
                // Debug.Log("stoneCubeInstance Spawned" + (-50 + i*10) + ", " + 5 + ", " + 40);
                if (coordinatePlane.IsEmpty(x,z+i) && (coordinatePlane.GetCheckoutTime(x,z)+1f > Time.deltaTime))
                {
                    //stoneCubeInstance.Initialise(new Vector3(x, height, z+i));
                    // Adjust with offset
                    // int offsetX = xTopLeft;
                    // int offsetZ = zTopLeft;
                    int offsetX = -xAxisSize / 2;
                    int offsetZ = -zAxisSize / 2;
                    //Initialize at the Bottom start from Left
                    //stoneCubeInstance.Initialise(new Vector3((x+i)*unitSize + offsetX*unitSize, height, z*unitSize + offsetZ*unitSize));
                    //Initialize at the Top start from Left
                    stoneCubeInstance.Initialise(new Vector3((x+i)*unitSize + offsetX*unitSize, height, -offsetZ*unitSize));
                    Debug.Log("StoneCube Spawn at " + ((x+i)*unitSize + offsetX*unitSize) + ", " + height + ", " + (z*unitSize + offsetZ*unitSize));

                    // Update grid status for the current position (0,0)
                    coordinatePlane.SetGridUnitInfo(x, z+i, false, "", 0);

                    // Update GridStat component attached to the cube object
                    GridStat gridStat = stoneCubeInstance.GetComponent<GridStat>();
                    if (gridStat != null)
                    {
                        gridStat.x = x;
                        gridStat.y = z+i;
                    }
                }
                else
                {
                    Debug.Log("coordinatePlane.IsEmpty(x,z+i) is " + coordinatePlane.IsEmpty(x,z+i));
                    Debug.Log("coordinatePlane.GetCheckoutTime(x,z)+1f > Time.deltaTime is " + (coordinatePlane.GetCheckoutTime(x,z)+1f > Time.deltaTime));
                }
                

                stoneCubeInstance.ActiveEmptyObj(); // CALL Prefab Method, ResetEmptyObj and setActive

                stoneCubeInstance.gameObject.SetActive(true); // Accessing the GameObject directly to set active

                
            }
            //i--;
            yield return new WaitForSeconds(spawnInterval);// this work
        }
        // Wait for the specified interval before spawning the next cube
        yield return new WaitForSeconds(spawnInterval);

        // Active All StoneCube Movement Here


        // Wait for the repeat interval before starting the next repetition
        //yield return new WaitForSeconds(repeatInterval);

        // Restart the spawning process
        //StartCoroutine(SpawnStoneCubes());
        Debug.Log("StoneCubeSpawner Coroutine Stopped");
        //StopCoroutine(SpawnStoneCubes());
    }

    private void SpawnStoneCube()
    {
        // Instantiate a stone cube prefab at the spawner's position and rotation
        //StoneCubeController cubeInstance = Instantiate(stoneCubePrefab, transform.position, Quaternion.identity);
    }

    private void SetGridPosition()
    {
        if (coordinatePlane != null && gridStat != null)
        {
            // Read grid position from GridStat script
            int currentX = gridStat.x;
            int currentZ = gridStat.z;

            // Check if the target grid position is within bounds
            if (currentX == 0 && currentZ == 1 && coordinatePlane.IsWithinBounds(currentX, currentZ))
            {
                CoordinatePlane.GridUnit gridUnit = coordinatePlane.GetGridUnit(currentX, currentZ);

                if (gridUnit.isEmpty && gridUnit.checkoutTime == 0f)
                {
                    // Update grid status
                    gridUnit.isEmpty = false;
                    gridUnit.objName = gameObject.name;
                    // You can leave checkoutTime unchanged if it's still 0f
                }
            }
        }
    }
}
