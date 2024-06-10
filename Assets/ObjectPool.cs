using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject crystalPrefab;
    public GameObject cabbagePrefab;
    public GameObject stoneCubePrefab;

    private Queue<GameObject> crystalPool = new Queue<GameObject>();
    private Queue<GameObject> cabbagePool = new Queue<GameObject>();
    private Queue<GameObject> stoneCubePool = new Queue<GameObject>();

    // Initialize the object pools
    private void Start()
    {
        // InitializePool(crystalPrefab, crystalPool);
        // InitializePool(cabbagePrefab, cabbagePool);
        // InitializeStoneCubePool(stoneCubePrefab, stoneCubePool);

        InitializePool(crystalPrefab, crystalPool, PoolObjectType.Crystal);
        InitializePool(cabbagePrefab, cabbagePool, PoolObjectType.Cabbage);
        InitializeStoneCubePool(stoneCubePrefab, stoneCubePool, PoolObjectType.StoneCube);
    }

    // Method to initialize a pool for a specific prefab
    private void InitializePool(GameObject prefab, Queue<GameObject> pool, PoolObjectType poolType)
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(prefab);

            // Attach ReturnToPoolOnDisable script and assign the pool reference and object type
            ReturnToPoolOnDisable returnToPool = obj.AddComponent<ReturnToPoolOnDisable>();
            returnToPool.Pool = this;
            returnToPool.PoolType = poolType;

            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // Method to initialize a pool for a specific prefab
    private void InitializePool(GameObject prefab, Queue<GameObject> pool)
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(prefab);

            // Attach ReturnToPoolOnDisable script and assign the pool reference and object type
            ReturnToPoolOnDisable returnToPool = obj.AddComponent<ReturnToPoolOnDisable>();
            returnToPool.Pool = this;
            //returnToPool.PoolType = objectType;


            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    //private void InitializeStoneCubePool(GameObject prefab, Queue<GameObject> pool)
    private void InitializeStoneCubePool(GameObject prefab, Queue<GameObject> pool, PoolObjectType poolType)
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(prefab);

            // Get the StoneCube component attached to the GameObject
            AutoRollCubeToTarget stoneCubeScript = obj.GetComponent<AutoRollCubeToTarget>();
            // Call the method on the AutoRollCubeToTarget script
            stoneCubeScript.DeactiveEmptyObj();

            // Set up a script to return the stone cube to the pool when deactivated
            ReturnToPoolOnDisable returnToPool = obj.AddComponent<ReturnToPoolOnDisable>();
            returnToPool.Pool = this;

            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // Method to get a crystal from the pool
    public GameObject GetCrystal()
    {
        if (crystalPool.Count == 0)
        {
            Debug.LogWarning("Crystal pool is empty! Consider increasing the pool size.");
            return null;
        }

        return crystalPool.Dequeue();
    }

    // Method to return a crystal to the pool
    public void ReturnCrystal(GameObject crystal)
    {
        crystal.SetActive(false);
        crystalPool.Enqueue(crystal);
    }

    // Similar methods for cabbage and stone cube objects
    public GameObject GetCabbage()
    {
        if (cabbagePool.Count == 0)
        {
            Debug.LogWarning("Cabbage pool is empty! Consider increasing the pool size.");
            return null;
        }

        return cabbagePool.Dequeue();
    }

    public void ReturnCabbage(GameObject cabbage)
    {
        cabbage.SetActive(false);
        cabbagePool.Enqueue(cabbage);
    }

    public GameObject GetStoneCube()
    {
        if (stoneCubePool.Count == 0)
        {
            Debug.LogWarning("Stone cube pool is empty! Consider increasing the pool size.");
            return null;
        }
        return stoneCubePool.Dequeue();
    }

    public void ReturnStoneCube(GameObject stoneCube)
    {
        // Get the StoneCube component attached to the GameObject
        AutoRollCubeToTarget stoneCubeScript = stoneCube.GetComponent<AutoRollCubeToTarget>();

        // Call the method on the AutoRollCubeToTarget script
        stoneCubeScript.DeactiveEmptyObj();

        stoneCube.SetActive(false);
        stoneCubePool.Enqueue(stoneCube);
    }
}
