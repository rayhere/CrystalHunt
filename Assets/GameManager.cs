using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public ObjectPool objectPool; // Reference to the ObjectPool component
    
    void Start()
    {
        SpawnCrystal();
        SpawnStoneCube();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCrystal()
    {
        // Set a Vector3 variable to a specific value
        Vector3 position = new Vector3(1.0f, 1.0f, 3.0f);

        // Set a Quaternion variable to a specific rotation
        Quaternion rotation = Quaternion.Euler(45.0f, 90.0f, 0.0f);

        // Set the scale of the GameObject
        Vector3 scale = new Vector3(2.0f, 2.0f, 2.0f);

        SpawnCrystal(position, rotation, scale);
        return;

        // Output the Vector3 values
        Debug.Log("X: " + position.x + ", Y: " + position.y + ", Z: " + position.z);

        // Output the Quaternion values
        Debug.Log("X: " + rotation.x + ", Y: " + rotation.y + ", Z: " + rotation.z + ", W: " + rotation.w);

        // Output the scale values
        Debug.Log("X Scale: " + transform.localScale.x + ", Y Scale: " + transform.localScale.y + ", Z Scale: " + transform.localScale.z);
    }


    // Method to get a crystal from the object pool and set its transform
    public void SpawnCrystal(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        // Get a crystal game object from the object pool
        GameObject crystal = objectPool.GetCrystal();
        Debug.Log("Crystal object: " + crystal); // Check


        if (crystal != null)
        {
            // Set the transform of the crystal object
            crystal.transform.position = position;
            crystal.transform.rotation = rotation;
            crystal.transform.localScale = scale;

            // Activate the crystal object
            crystal.SetActive(true);
        }
        else
        {
            Debug.LogError("Failed to get crystal from object pool.");
        }
    }

    public void SpawnStoneCube()
    {
        // Set a Vector3 variable to a specific value
        Vector3 position = new Vector3(10.0f, 5.0f, 10.0f);

        // Set a Quaternion variable to a specific rotation
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);

        // Set the scale of the GameObject
        Vector3 scale = new Vector3(1.0f, 1.0f, 1f);

        SpawnStoneCube(position, rotation, scale);
        return;

        // Output the Vector3 values
        Debug.Log("X: " + position.x + ", Y: " + position.y + ", Z: " + position.z);

        // Output the Quaternion values
        Debug.Log("X: " + rotation.x + ", Y: " + rotation.y + ", Z: " + rotation.z + ", W: " + rotation.w);

        // Output the scale values
        Debug.Log("X Scale: " + transform.localScale.x + ", Y Scale: " + transform.localScale.y + ", Z Scale: " + transform.localScale.z);
    }

    // Method to get a stonecube from the object pool and set its transform
    public void SpawnStoneCube(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        // Get a stoneCube game object from the object pool
        GameObject stoneCube = objectPool.GetStoneCube();

        // Set the transform of the stoneCube object
        //if (position != null)
        stoneCube.transform.position = position;
        Debug.Log("stoneCube.transform.position");
        //if (rotation != null)
        stoneCube.transform.rotation = rotation;
        Debug.Log("stoneCube.transform.rotation");
        //if (scale != null)
        stoneCube.transform.localScale = scale;
        Debug.Log("stoneCube.transform.localScale");


        // Get the StoneCube component attached to the GameObject
        AutoRollCubeToTarget stoneCubeScript = stoneCube.GetComponent<AutoRollCubeToTarget>();

        // Call the method on the AutoRollCubeToTarget script
        stoneCubeScript.ActiveEmptyObj();


        // Activate the StoneCube object
        stoneCube.SetActive(true);
    }
}
