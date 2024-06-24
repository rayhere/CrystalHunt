using System.Collections;
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
    [Tooltip("NavMesh Data")]
    public NavMeshData navMeshData;
    private NavMeshDataInstance navMeshDataInstance;

    private NavMeshTriangulation triangulation;

    [Header("Spawn Distance from Player")]
    [SerializeField, Tooltip("Minimum distance from Player that Cabbage can spawn.")]
    private float minSpawnDistance = 50f;

    public float MinSpawnDistance
    {
        get { return minSpawnDistance; }
        set { minSpawnDistance = value; }
    }

    private void Awake()
    {
        triangulation = NavMesh.CalculateTriangulation();
        if (navMeshData != null)
        {
            navMeshDataInstance = NavMesh.AddNavMeshData(navMeshData);
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds wait = new WaitForSeconds(SpawnDelay);

        int spawnedEnemies = 0;
        while (spawnedEnemies < numberOfSpawns)
        {
            DoSpawnEnemy(1);
            spawnedEnemies++;
            yield return wait;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnResources(numberOfSpawns);
        }
    }

    void SpawnResources(int numberOfSpawns)
    {
        for (float x = negativePosition.x; x < positivePosition.x; x += distanceBetweenCheck)
        {
            for (float z = negativePosition.y; z < positivePosition.y; z += distanceBetweenCheck)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(x, heightOfCheck, z), Vector3.down, out hit, rangeOfCheck, layerMask))
                {
                    if (spawnChance > Random.Range(0f, 100f))
                    {
                        CabbageController cabbageInstance = ObjectPooler.DequeueObject<CabbageController>("Cabbage");

                        if (cabbageInstance != null)
                        {
                            Vector3 randomPosition = GetRandomNavMeshPosition();
                            cabbageInstance.Initialise(randomPosition);
                            cabbageInstance.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        NavMeshHit hit;

        if (navMeshData != null)
        {
            randomPosition = SamplePositionFromNavMeshData();
        }
        else
        {
            // Fallback to using triangulation
            int vertexIndex = Random.Range(0, triangulation.vertices.Length);
            randomPosition = triangulation.vertices[vertexIndex];
        }

        // Ensure the position is at least minSpawnDistance away from the player
        while (Vector3.Distance(randomPosition, Player.position) < minSpawnDistance)
        {
            randomPosition = GetRandomNavMeshPosition();
        }

        return randomPosition;
    }

    Vector3 SamplePositionFromNavMeshData()
    {
        Vector3 randomPosition = Vector3.zero;
        NavMeshHit hit;
        Vector3 randomDirection = Random.insideUnitSphere * 10f; // Max distance for sampling

        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            randomPosition = hit.position;
        }
        else
        {
            Debug.LogWarning("NavMesh.SamplePosition did not find a valid position.");
        }

        return randomPosition;
    }

    void DoSpawnEnemy(int spawnIndex)
    {
        Vector3 spawnPosition = GetRandomNavMeshPosition();

        CabbageController cabbageInstance = ObjectPooler.DequeueObject<CabbageController>("Cabbage");
        Debug.Log("Cabbage is spawning here spawnPosition " + spawnPosition);
        if (cabbageInstance != null)
        {   
            cabbageInstance.gameObject.SetActive(false);
            cabbageInstance.mySpawnedPosition = spawnPosition;
            //cabbageInstance.Agent.Warp(spawnPosition);
            cabbageInstance.transform.position = spawnPosition; // Optionally, move the object to spawnPosition
            cabbageInstance.gameObject.SetActive(true);
        }
    }
}
