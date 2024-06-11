using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CabbageController cabbagePrefab;
    public AutoRollCubeToTarget stoneCubePrefab;

    private void Awake()
    {
        // SetupPool();
        SetupGreenCabbagePool(6);
        SetupStoneCubePool(stoneCubePrefab, 5, "StoneCube");
    }

    private void SetupPool()
    {
        //ObjectPooler.SetupPool(cabbagePrefab, 1, "Cabbage");
    }

    private void SetupPool<T>(T pooledItemPrefab, int poolSize, string poolDictionary) where T : Component
    {
        ObjectPooler.SetupPool(pooledItemPrefab, poolDictionary);

        for (int i = 0; i < poolSize; i++)
        {
            T newInstance = ObjectPooler.EnqueueNewInstance<T>(pooledItemPrefab, poolDictionary);

            if (newInstance != null)
            {
                newInstance.transform.SetParent(transform, false);
                
                // Check if the instance is of type T
                if (newInstance is T)
                {
                    T objInstance = newInstance as T;
                    // CALL Prefab Method
                    //objInstance.method();
                }

                newInstance.gameObject.SetActive(false); // Accessing the GameObject directly to set deactive
            }
        }
    }

    private void SetupStoneCubePool<T>(T pooledItemPrefab, int poolSize, string poolDictionary) where T : Component
    {
        ObjectPooler.SetupPool(pooledItemPrefab, poolDictionary);

        for (int i = 0; i < poolSize; i++)
        {
            T newInstance = ObjectPooler.EnqueueNewInstance<T>(pooledItemPrefab, poolDictionary);

            if (newInstance != null)
            {
                newInstance.transform.SetParent(transform, false);
                
                // Check if the instance is of type AutoRollCubeToTarget
                if (newInstance is AutoRollCubeToTarget)
                {
                    AutoRollCubeToTarget cubeInstance = newInstance as AutoRollCubeToTarget;
                    cubeInstance.DeactiveEmptyObj();
                }

                newInstance.gameObject.SetActive(false); // Accessing the GameObject directly to set deactive
            }
        }
    }

    private void SetupGreenCabbagePool(int poolSize)
    {
        ObjectPooler.SetupPool(cabbagePrefab, "Cabbage");

        for (int i = 0; i < poolSize; i++)
        {
            CabbageController cabbageInstance = ObjectPooler.EnqueueNewInstance<CabbageController>(cabbagePrefab, "Cabbage");
            if (cabbageInstance != null)
            {
                cabbageInstance.transform.SetParent(transform, false);
                cabbageInstance.gameObject.SetActive(false); // Accessing the GameObject directly to set deactive
            }
        }
    }

}
