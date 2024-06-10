using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CabbageController cabbagePrefab;

    private void Awake()
    {
        // SetupPool();
        SetupGreenCabbagePool(6);
    }

    private void SetupPool()
    {
        //ObjectPooler.SetupPool(cabbagePrefab, 1, "Cabbage");
    }

    private void SetupGreenCabbagePool0(int poolSize)
    {
        //ObjectPooler.SetupPool(cabbagePrefab, 1, "Cabbage");
        ObjectPooler.SetupPool(cabbagePrefab, "Cabbage");
        for (int i = 0; i < poolSize; i++)
        {
            CabbageController cabbageInstance = ObjectPooler.DequeueObject<CabbageController>("Cabbage");
            if(cabbageInstance != null)
            {
                cabbageInstance.transform.SetParent(transform, false); // will set the parent of the pooled instance
                //cabbageInstance.Initialise(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                cabbageInstance.gameObject.SetActive(false); // Accessing the GameObject directly to set active
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
                cabbageInstance.gameObject.SetActive(true); // Set the object active after dequeuing from the pool
            }
        }
    }

}
