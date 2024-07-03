using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class ResourceGenerator : MonoBehaviour
{
    [Header("Spawn settings")]
    public GameObject resourcePrefab;
    public float spawnChance;
 
    [Header("Raycast setup")]
    public float distanceBetweenCheck;
    public float heightOfCheck = 10f, rangeOfCheck = 30f;
    public LayerMask layerMask;
    public Vector2 positivePosition, negativePosition;
 
    private void Start()
    {
        SpawnResources();
    }
 
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            //DeleteResources();
            SpawnResources();
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
                            cabbageInstance.transform.SetParent(transform, false); // will set the parent of the pooled instance
                            cabbageInstance.Initialise(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                            cabbageInstance.gameObject.SetActive(true); // Accessing the GameObject directly to set active
                        }
                    }
                }
            }
        }
    }
 
    void DeleteResources()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}