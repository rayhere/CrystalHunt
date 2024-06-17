using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSpawner : MonoBehaviour
{
    [Header("Spawn settings")]
    [SerializeField, Tooltip("Drag Crystal Prefab Here.")]
    public GameObject crystalPrefab;
    [SerializeField, Tooltip("numberOfSpawns Default is 10")]
    public int numberOfSpawns = 10;
    [SerializeField, Tooltip("Range 0-102")]
    public float spawnChance;
 
    [Header("Raycast setup")]
    public float distanceBetweenCheck;
    public float heightOfCheck = 10f, rangeOfCheck = 30f;
    public LayerMask layerMask;
    public Vector2 positivePosition, negativePosition;
 
    private void Start()
    {
        SpawnResources(numberOfSpawns);
    }
 
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            //DeleteResources();
            Debug.Log("CrystalSpawner R Key Pressed");
            SpawnResources(numberOfSpawns);
        }
    }
 
    void SpawnResources()
    {
        for(float x = negativePosition.x; x < positivePosition.x; x += distanceBetweenCheck)
        {
            for(float z = negativePosition.y; z < positivePosition.y; z += distanceBetweenCheck)
            {
                RaycastHit hit;
                if(Physics.Raycast(new Vector3(x, heightOfCheck, z), Vector3.down, out hit, rangeOfCheck, layerMask))
                {
                    if(spawnChance > Random.Range(0f, 101f))
                    {
                        //Instantiate(resourcePrefab, hit.point, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform);
                        CabbageController cabbageInstance = ObjectPooler.DequeueObject<CabbageController>("Cabbage");

                        if(cabbageInstance != null)
                        {
                            //cabbageInstance.transform.SetParent(transform, false); // will set the parent of the pooled instance
                            cabbageInstance.Initialise(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                            cabbageInstance.gameObject.SetActive(true); // Accessing the GameObject directly to set active
                        }
                    }
                }
            }
        }
    }

    void SpawnResources(int numberOfSpawns)
    {
        int i = 0;
        while (i < numberOfSpawns)
        {
            for(float x = negativePosition.x; x < positivePosition.x; x += distanceBetweenCheck)
            {
                for(float z = negativePosition.y; z < positivePosition.y; z += distanceBetweenCheck)
                {
                    if (i >= numberOfSpawns) 
                    {
                        Debug.Log("SpawnResources Loop Stopped");
                        return; // Exit the Loop gracefully
                    }
                    RaycastHit hit;
                    if(Physics.Raycast(new Vector3(x, heightOfCheck, z), Vector3.down, out hit, rangeOfCheck, layerMask))
                    {
                        if(spawnChance > Random.Range(0f, 101f))
                        {
                            Debug.Log("Crystal allow to spawn in x " + x + " z " + z);
                            //Instantiate(resourcePrefab, hit.point, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform);
                            CrystalController crystalInstance = ObjectPooler.DequeueObject<CrystalController>("Crystal");
                            //Instantiate(crystalInstance, hit.point, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform); // Spawn Crystal in position hit.point, rotation Random 0-360, transform parent to this object
                            //Instantiate(crystalInstance, hit.point, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform);
                            i++;
                            if(crystalInstance != null)
                            {
                                //cabbageInstance.transform.SetParent(transform, false); // will set the parent of the pooled instance
                                //crystalInstance.Initialise(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                                crystalInstance.Initialise(hit.point);
                                crystalInstance.gameObject.SetActive(true); // Accessing the GameObject directly to set active
                            }
                        }
                    }
                }
            }
        }
        Debug.Log("CrystalSpawner SpawnResources i is " + i + " out of numberofSpawns is " + numberOfSpawns + " but Spawn ended");
    }
 
    void DeleteResources()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
