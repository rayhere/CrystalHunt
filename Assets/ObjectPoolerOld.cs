using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPoolerOld
{
    // Dictionary to look up prefab by name
    public static Dictionary<string, Component> poolLookup = new Dictionary<string, Component>();

    // Dictionary to hold queues of inactive instances of each prefab
    public static Dictionary<string, Queue<Component>> poolDictionary = new Dictionary<string, Queue<Component>>();
    public static Dictionary<string, GameObject> parentDictionary = new Dictionary<string, GameObject>(); // New dictionary to hold parent game objects

    // Method to return an object to the pool
    public static void EnqueueObject<T>(T item, string name) where T : Component
    {
        // If the object is not active, do nothing
        if (!item.gameObject.activeSelf) {return;}

        // Reset object position
        item.transform.position = Vector3.zero;

        // Enqueue object back into the pool and deactivate it
        poolDictionary[name].Enqueue(item);
        item.transform.SetParent(parentDictionary[name].transform); // Set parent of the object
        item.gameObject.SetActive(false);
    }

    // Method to retrieve an object from the pool
    public static T DequeueObject<T>(string key) where T : Component
    {
/*         // Try to dequeue an object from the pool
        if (poolDictionary[key].TryDequeue(out var item))
        {
            // If successful, return the object
            return (T)item;
        }
        // If pool is empty, instantiate a new object and return it
        return (T)EnqueueNewInstance(poolLookup[key], key); */
        if (poolDictionary.ContainsKey(key) && poolDictionary[key].Count > 0)
        {
            var item = poolDictionary[key].Dequeue();
            item.gameObject.SetActive(true);
            return (T)item;
        }
        else
        {
            Debug.LogWarning($"Pool for key '{key}' is empty or does not exist.");
            return null;
        }
    }

    // Method to instantiate a new object and add it to the pool
    public static T EnqueueNewInstance<T>(T item, string key) where T : Component
    {
        // Instantiate a new object
        T newInstance = Object.Instantiate(item);

        // Deactivate the new object
        newInstance.gameObject.SetActive(false);

        // Reset object position
        newInstance.transform.position = Vector3.zero;

/*         // Add new object to the pool and return it
        poolDictionary[key].Enqueue(newInstance);
        newInstance.transform.SetParent(parentDictionary[key].transform); // Set parent of the object */

        /* // Add new object to the pool and set its parent
        poolDictionary[key].Enqueue(newInstance);
        newInstance.transform.SetParent(parentDictionary[key].transform); // Set parent of the object

        // Set the name of the instance to match the pool name
        newInstance.gameObject.name = $"{key} Instance"; */

        // Check if parent dictionary has the entry
        if (parentDictionary.ContainsKey(key))
        {
            newInstance.transform.SetParent(parentDictionary[key].transform);
        }
        else
        {
            Debug.LogError($"Parent dictionary does not contain key: {key}");
        }

        poolDictionary[key].Enqueue(newInstance);
        newInstance.gameObject.name = $"{key} Instance";

        return newInstance;
    }

    // Method to set up the initial pool for a prefab
    public static void SetupPool<T>(T pooledItemPrefab, int poolSize, string dictionaryEntry) where T : Component
    {
        if (poolDictionary.ContainsKey(dictionaryEntry))
        {
            poolDictionary[dictionaryEntry].Clear();
        }
        else
        {
            poolDictionary.Add(dictionaryEntry, new Queue<Component>());
        }

        poolLookup[dictionaryEntry] = pooledItemPrefab;

        if (!parentDictionary.ContainsKey(dictionaryEntry))
        {
            GameObject parent = new GameObject(dictionaryEntry + " Pool");
            parentDictionary.Add(dictionaryEntry, parent);
        }

/*         // Create a new queue for the prefab
        poolDictionary.Add(dictionaryEntry, new Queue<Component>());

        // Add the prefab to the lookup dictionary
        poolLookup.Add(dictionaryEntry, pooledItemPrefab);

        // Create a parent GameObject for the pooled objects
        GameObject parent = new GameObject(dictionaryEntry + " Pool");
        
        parentDictionary.Add(dictionaryEntry, parent); // Store the parent GameObject in the parentDictionary */


        /* // Instantiate and add the specified number of instances to the pool
        for(int i = 0; i < poolSize; i++)
        {
            //T pooledInstance = Object.Instantiate(pooledItemPrefab, Vector3.zero, Quaternion.identity, parent.transform);
            T pooledInstance = Object.Instantiate(pooledItemPrefab);
            pooledInstance.gameObject.SetActive(false);
            poolDictionary[dictionaryEntry].Enqueue((T)pooledInstance);
            //poolDictionary[dictionaryEntry].Enqueue(pooledInstance);
        } */
        
        // Instantiate and add the specified number of instances to the pool
        // for(int i = 0; i < poolSize; i++)
        // {
        //     T pooledInstance = Object.Instantiate(pooledItemPrefab);
        //     pooledInstance.gameObject.SetActive(false);
        //     poolDictionary[dictionaryEntry].Enqueue(pooledInstance);
        //     pooledInstance.transform.SetParent(parent.transform); // Set parent of the object
        // }
    }

    // Method to set up the initial pool for a prefab
    public static void SetupPool<T>(T pooledItemPrefab, string dictionaryEntry) where T : Component
    {
        // Check if the dictionary already contains the entry
        if (poolDictionary.ContainsKey(dictionaryEntry))
        {
            // If it does, clear the existing queue
            poolDictionary[dictionaryEntry].Clear();
        }
        else
        {
            // If it doesn't, create a new queue for the prefab
            poolDictionary.Add(dictionaryEntry, new Queue<Component>());
        }

        // Add the prefab to the lookup dictionary or update it if it exists
        poolLookup[dictionaryEntry] = pooledItemPrefab;

        // Create a parent GameObject for the pooled objects if it doesn't exist
/*         if (!parentDictionary.ContainsKey(dictionaryEntry))
        {
            GameObject parent = new GameObject(dictionaryEntry + " Pool");
            parentDictionary.Add(dictionaryEntry, parent); // Store the parent GameObject in the parentDictionary
        } */
        if (!parentDictionary.ContainsKey(dictionaryEntry))
        {
            GameObject parent = new GameObject(dictionaryEntry + " Pool");
            parentDictionary.Add(dictionaryEntry, parent); // Store the parent GameObject in the parentDictionary
        }
    }
}
