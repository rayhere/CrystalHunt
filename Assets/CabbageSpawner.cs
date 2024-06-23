using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CabbageSpawner : MonoBehaviour
{
    [Header("Spawn settings")]
    [SerializeField, Tooltip("Drag Cabbage Prefab Here.")]
    public GameObject cabbagePrefab;

    [SerializeField, Tooltip("numberOfSpawns Default is 10")]
    public int numberOfSpawns = 10;
    [SerializeField, Tooltip("Range 0-100")]
    [Range(0f, 100f)]
    public float spawnChance = 1f;

    public Transform Player;
    public float SpawnDelay = 1f;

  
 
    [Header("Raycast setup")]
    [SerializeField, Tooltip("Min Range 0.1")]
    public float distanceBetweenCheck = 1f;
    public float heightOfCheck = 10f, rangeOfCheck = 30f;
    public LayerMask layerMask;
    public Vector2 positivePosition, negativePosition;

    [Header("NavMesh setup")] 
    [Tooltip("Layer mask for NavMesh.")]
    public LayerMask navMeshLayerMask;

    private NavMeshTriangulation Triangulation;

    public NavMeshAgent Agent { get; private set; }
    
    private void Awake()
    {
        Triangulation = NavMesh.CalculateTriangulation();
    }

    private void Start()
    {
        //StartCoroutine(SpawnResources(numberOfSpawns));

        StartCoroutine(SpawnEnemies());
    }
 
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            //DeleteResources();
            Debug.Log("CabbageSpawner R Key Pressed");
            SpawnResources(numberOfSpawns);
        }
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds Wait = new WaitForSeconds(SpawnDelay);

        int SpawnedEnemies = 0;
        int NumberOfEnemiesToSpawn = numberOfSpawns;
        while (SpawnedEnemies < NumberOfEnemiesToSpawn)
        {
            DoSpawnEnemy(1);
            SpawnedEnemies++;
            yield return Wait;
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
                    if(spawnChance > Random.Range(0f, 100f))
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

    IEnumerator SpawnResources(int numberOfSpawns)
    {

        Debug.Log("SpawnResources Cabbage");
        //int i = 0;
        //while (i < numberOfSpawns)
        int spawnedCount = 0;
        while (spawnedCount < numberOfSpawns)
        //for (int i = 0; i < numberOfSpawns; i++)
        {
            if (spawnedCount >= numberOfSpawns) 
            {
                Debug.Log("SpawnResources Loop Stopped");
                break; // Exit the Loop gracefully
            }

            Debug.Log("SpawnResources Cabbage spawnedCount  is " + spawnedCount);
            Vector3 randomPosition = GetRandomNavMeshPosition();
            //Vector3 randomPosition = new Vector3 (0, 0, 0);
            Debug.Log("Cabbage randomPosition is " + randomPosition);
            if (randomPosition != Vector3.zero)
            {
                if (spawnChance > Random.Range(0f, 100f))
                {
                    CabbageController cabbageInstance = ObjectPooler.DequeueObject<CabbageController>("Cabbage");
                    //i++; 
                    spawnedCount++;
                    if (cabbageInstance != null)
                    {
                        cabbageInstance.Initialise(randomPosition);
                        cabbageInstance.gameObject.SetActive(true);
                        
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds
                        Debug.Log("Out of cabbageInstance");
                    }
                }
                else
                {
                    yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds
                    Debug.Log("Failed to Spawn, spawnChance out of ranged");
                }
            }
            yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds before spawning the next cabbage
        }
        Debug.Log("CabbageSpawner SpawnResources spawnedCount is " + spawnedCount + " out of numberofSpawns is " + numberOfSpawns + " but Spawn ended");
    }

    Vector3 GetRandomNavMeshPosition0()
    {
        Vector3 randomPosition = Vector3.zero;
        NavMeshHit hit;
        Vector3 randomDirection = Random.insideUnitSphere * 10f; // Max distance for sampling

        if (NavMesh.SamplePosition(transform.position + randomDirection, out hit, 10f, navMeshLayerMask))
        {
            randomPosition = hit.position;
        }

        return randomPosition;
    }

    Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        NavMeshHit hit;
        Vector3 randomDirection = Random.insideUnitSphere * 10f; // Max distance for sampling

        if (NavMesh.SamplePosition(transform.position + randomDirection, out hit, 10f, navMeshLayerMask))
        {
            randomPosition = hit.position;
        }
        else
        {
            Debug.LogWarning("NavMesh.SamplePosition did not find a valid position.");
        }

        return randomPosition;
    }
 
    void DeleteResources()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void DoSpawnEnemy(int SpawnIndex)
    {

        Debug.Log("DoSpawnEnemy Cabbage");
        int spawnedCount = 0;
        while (spawnedCount < SpawnIndex)
        {
            if (spawnedCount >= SpawnIndex) 
            {
                Debug.Log("SpawnResources Loop Stopped");
                break; // Exit the Loop gracefully
            }

            
            int VertexIndex = Random.Range(0, Triangulation.vertices.Length);

            NavMeshHit Hit;
            if (NavMesh.SamplePosition(Triangulation.vertices[VertexIndex], out Hit, 2f, -1))
            {
                CabbageController cabbageInstance = ObjectPooler.DequeueObject<CabbageController>("Cabbage");
                spawnedCount++;

                // enemy.Agent.Warp(Hit.position);
                // // enemy needs to get enabled and start chasing now.
                // enemy.Movement.Player = Player;
                // enemy.Agent.enabled = true;
                // enemy.Movement.StartChasing();

                if (cabbageInstance != null)
                {
                    //cabbageInstance.Initialise(Hit.position);
                    cabbageInstance.Agent.Warp(Hit.position);
                    cabbageInstance.gameObject.SetActive(true);
                    
                }
                else
                {
                    //yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds
                    Debug.Log("Out of cabbageInstance");
                    //Debug.LogError($"Unable to fetch enemy of type {SpawnIndex} from object pool. Out of objects?");
                }

            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use {Triangulation.vertices[VertexIndex]}");
            }
        }
    }

}
