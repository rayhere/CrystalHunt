using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* a script that spawns a stone cube every 1 second, repeated 3 times with a 3-second delay between each repetition */

public class StoneCubeSpawner : MonoBehaviour
{
    public StoneCubeController stoneCubePrefab; // Controller Script that with Prefab
    public int numberOfSpawns = 3;
    public float spawnInterval = 1f;
    public float repeatInterval = 3f;

    private void Start()
    {
        // Start the spawning process
        StartCoroutine(SpawnStoneCubes());
    }

    private IEnumerator SpawnStoneCubes()
    {
        // Repeat spawning process for the specified number of times
        for (int i = 0; i < numberOfSpawns; i++)
        {
            // Spawn a stone cube
            //SpawnStoneCube();
            StoneCubeController stoneCubeInstance = ObjectPooler.DequeueObject<StoneCubeController>("StoneCube");

            if(stoneCubeInstance != null)
            {
                stoneCubeInstance.transform.SetParent(transform, false);

                stoneCubeInstance.Initialise(new Vector3(-30 + i*10, 5, 40));

                stoneCubeInstance.ActiveEmptyObj(); // CALL Prefab Method, ResetEmptyObj and setActive

                stoneCubeInstance.gameObject.SetActive(true); // Accessing the GameObject directly to set active
            }

            // Wait for the specified interval before spawning the next cube
            yield return new WaitForSeconds(spawnInterval);
        }

        // Wait for the repeat interval before starting the next repetition
        yield return new WaitForSeconds(repeatInterval);

        // Restart the spawning process
        StartCoroutine(SpawnStoneCubes());
    }

    private void SpawnStoneCube()
    {
        // Instantiate a stone cube prefab at the spawner's position and rotation
        StoneCubeController cubeInstance = Instantiate(stoneCubePrefab, transform.position, Quaternion.identity);
    }
}

