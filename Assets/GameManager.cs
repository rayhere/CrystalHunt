using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CabbageController cabbagePrefab;
    public CrystalController crystalPrefab;
    public StoneCubeController stoneCubePrefab;
    public GameObject targetObject;

    private void Awake()
    {
        // SetupPool();
        SetupGreenCabbagePool(6);
        //SetupStoneCubePool(stoneCubePrefab, 5, "StoneCube");

        // Find the target object using tag
        targetObject = GameObject.FindGameObjectWithTag("StoneCube");

        // Setup Stone Cube pool with the found target object
        SetupStoneCubePool(stoneCubePrefab, 10, "StoneCube", targetObject.transform);
        //SetupStoneCubePool(stoneCubePrefab, 5, "StoneCube", target);

        SetupCrystalPool(crystalPrefab, 5, "Crystal");
    }

    private void Start()
    {
        
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

    private void SetupCrystalPool<T>(T pooledItemPrefab, int poolSize, string poolDictionary) where T : Component
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

    private void SetupStoneCubePool<T>(T pooledItemPrefab, int poolSize, string poolDictionary, Transform target) where T : Component
    {
        ObjectPooler.SetupPool(pooledItemPrefab, poolDictionary);

        for (int i = 0; i < poolSize; i++)
        {
            T newInstance = ObjectPooler.EnqueueNewInstance<T>(pooledItemPrefab, poolDictionary);

            if (newInstance != null)
            {
                newInstance.transform.SetParent(transform, false);
                
                // Check if the instance is of type StoneCubeController
                if (newInstance is StoneCubeController)
                {
                    StoneCubeController cubeInstance = newInstance as StoneCubeController;
                    cubeInstance.SetTarget(target); // Set the target for the stone cube
                    cubeInstance.SetEmptyObjectToParent();
                    cubeInstance.DeactiveEmptyObj();
                }

                newInstance.gameObject.SetActive(false); // Set the GameObject inactive
            }
        }
    }
}
