using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public ObjectPool objectPool; // Reference to the ObjectPool component
    
    void Start()
    {
        SpawnCrystal();
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

        // Set the transform of the crystal object
        //if (position != null)
        crystal.transform.position = position;
        Debug.Log("crystal.transform.position");
        //if (rotation != null)
        crystal.transform.rotation = rotation;
        Debug.Log("crystal.transform.rotation");
        //if (scale != null)
        crystal.transform.localScale = scale;
        Debug.Log("crystal.transform.localScale");

        // Activate the crystal object
        crystal.SetActive(true);
    }
}
