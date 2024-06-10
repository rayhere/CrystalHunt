using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public CabbageController cabbagePrefab; // Change GameObject to CabbageController

    void Update() 
    {
        if (Input.GetKey(KeyCode.Space))  
        {
            CabbageController cabbageInstance = ObjectPooler.DequeueObject<CabbageController>("Cabbage"); // Use CabbageController type
            // No need to get the component, as it's already the correct type
            //CabbageController cabbageController = cabbageInstance.GetComponent<CabbageController>();

            if(cabbageInstance != null)
            {
                cabbageInstance.Initialise(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                cabbageInstance.gameObject.SetActive(true); // Accessing the GameObject directly to set active
            }
        } 
    }
}
