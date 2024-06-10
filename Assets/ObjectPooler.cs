using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPooler
{
    // Dictionary to look up prefab by name
    public static Dictionary<string, Component> poolLookup = new Dictionary<string, Component>();

    // Dictionary to hold queues of inactive instances of each prefab
    public static Dictionary<string, Queue<Component>> poolDictionary = new Dictionary<string, Queue<Component>>();

    // Method to return an object to the pool
    public static void EnqueueObject<T>(T item, string name) where T : Component
    {
        // If the object is not active, do nothing
        if (!item.gameObject.activeSelf) {return;}

        // Reset object position
        item.transform.position = Vector3.zero;

        // Enqueue object back into the pool and deactivate it
        poolDictionary[name].Enqueue(item);
        item.gameObject.SetActive(false);
    }

    // Method to retrieve an object from the pool
    public static T DequeueObject<T>(string key) where T : Component
    {
        // Try to dequeue an object from the pool
        if (poolDictionary[key].TryDequeue(out var item))
        {
            // If successful, return the object
            return (T)item;
        }
        // If pool is empty, instantiate a new object and return it
        return (T)EnqueueNewInstance(poolLookup[key], key);
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

        // Add new object to the pool and return it
        poolDictionary[key].Enqueue(newInstance);
        return newInstance;
    }

    // Method to set up the initial pool for a prefab
    public static void SetupPool<T>(T pooledItemPrefab, int poolSize, string dictionaryEntry) where T : Component
    {
        // Create a new queue for the prefab
        poolDictionary.Add(dictionaryEntry, new Queue<Component>());

        // Add the prefab to the lookup dictionary
        poolLookup.Add(dictionaryEntry, pooledItemPrefab);

        // Instantiate and add the specified number of instances to the pool
        for(int i = 0; i < poolSize; i++)
        {
            T pooledInstance = Object.Instantiate(pooledItemPrefab);
            pooledInstance.gameObject.SetActive(false);
            poolDictionary[dictionaryEntry].Enqueue((T)pooledInstance);
        }
    }
}
