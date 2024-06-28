using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public DarknessStatsSO playerStats; // Reference to PlayerStatsSO asset
    public CabbageController cabbagePrefab;
    public CrystalController crystalPrefab;
    public StoneCubeController stoneCubePrefab;
    public GameObject targetObject;
    public InGameUI inGameUI;

    private void Awake()
    {
        InitializePlayerStats();
    }

    private void Start()
    {
        // SetupGreenCabbagePool(6);
        //SetupStoneCubePool(stoneCubePrefab, 10, "StoneCube");

        SetupGreenCabbagePool(cabbagePrefab, 20, "Cabbage");

        // Find the target object using tag
        targetObject = GameObject.FindGameObjectWithTag("StoneCube");

        // Setup Stone Cube pool with the found target object
        SetupStoneCubePool(stoneCubePrefab, 10, "StoneCube", targetObject.transform);

        SetupCrystalPool(crystalPrefab, 5, "Crystal");
    }

    private void Update()
    {
        // Check if player is dead
        if (playerStats != null && playerStats.IsDead())
        {
            Debug.Log("Player is dead!");
            // Handle player death logic (e.g., respawn, game over, etc.)
            inGameUI.playerAlive = false;
        }
    }

    private void InitializePlayerStats()
    {
        if (playerStats != null)
        {
            playerStats.Initialize(); // This initializes currentHP to maxHP
        }
        else
        {
            Debug.LogError("PlayerStatsSO is not assigned in GameManager!");
        }
    }

    private void SetupGreenCabbagePool<T>(T pooledItemPrefab, int poolSize, string poolDictionary) where T : Component
    {
        ObjectPooler.SetupPool(pooledItemPrefab, poolDictionary);

        for (int i = 0; i < poolSize; i++)
        {
            T newInstance = ObjectPooler.EnqueueNewInstance<T>(pooledItemPrefab, poolDictionary);

            if (newInstance != null)
            {
                // Setting parent to Crystal Pool
                newInstance.transform.SetParent(ObjectPooler.parentDictionary[poolDictionary].transform);
                //newInstance.transform.SetParent(transform, false);
                
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
                // Setting parent to Cabbage Pool
                cabbageInstance.transform.SetParent(ObjectPooler.parentDictionary["Cabbage"].transform);
                //cabbageInstance.transform.SetParent(transform, false);
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
                // Setting parent to Crystal Pool
                newInstance.transform.SetParent(ObjectPooler.parentDictionary[poolDictionary].transform);
                //newInstance.transform.SetParent(transform, false);
                
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
                //newInstance.transform.SetParent(transform, false);
                newInstance.transform.SetParent(ObjectPooler.parentDictionary[poolDictionary].transform);
                // Check if the instance is of type StoneCubeController
                if (newInstance is StoneCubeController)
                {
                    StoneCubeController cubeInstance = newInstance as StoneCubeController;
                    
                    if (target == null)
                    {
                        //target = GameObject.FindGameObjectWithTag("StoneCube").transform;
                        target = GameObject.FindGameObjectWithTag("Player").transform;
                    }
                    // Setting parent to StoneCube Pool
                    cubeInstance.transform.SetParent(ObjectPooler.parentDictionary[poolDictionary].transform);
                    cubeInstance.SetTarget(target); // Set the target for the stone cube
                    cubeInstance.SetEmptyObjectToParent();
                    cubeInstance.DeactivateEmptyObject();
                }

                newInstance.gameObject.SetActive(false); // Set the GameObject inactive
            }
        }
    }
}
