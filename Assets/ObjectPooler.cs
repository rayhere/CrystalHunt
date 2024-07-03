using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPooler
{
    // Dictionary to look up prefab by name
    public static Dictionary<string, Component> poolLookup = new Dictionary<string, Component>();

    // Dictionary to hold queues of inactive instances of each prefab
    public static Dictionary<string, Queue<Component>> poolDictionary = new Dictionary<string, Queue<Component>>();

    // Dictionary to store parent GameObjects for pooled objects
    public static Dictionary<string, GameObject> parentDictionary = new Dictionary<string, GameObject>();

    // Method to return an object to the pool
    public static void EnqueueObject<T>(T item, string key) where T : Component
    {
        if (!item.gameObject.activeSelf)
        {
            return; // If the object is not active, do nothing
        }

        item.transform.position = Vector3.zero;

        // Set parent of the object if parent GameObject exists
        if (parentDictionary.ContainsKey(key) && parentDictionary[key] != null)
        {
            item.transform.SetParent(parentDictionary[key].transform);
        }
        else
        {
            Debug.LogWarning($"Parent GameObject for key '{key}' is null or destroyed.");
            item.transform.SetParent(null); // Set parent to null if the parent GameObject is null or destroyed
        }

        // Enqueue object back into the pool and deactivate it
        poolDictionary[key].Enqueue(item);
        item.gameObject.SetActive(false);
    }

    // Method to retrieve an object from the pool
    public static T DequeueObject<T>(string key) where T : Component
    {
        if (poolDictionary.ContainsKey(key))
        {
            if (poolDictionary[key].Count > 0)
            {
                var item = poolDictionary[key].Dequeue();
                item.gameObject.SetActive(true);

                // Set parent of the object if parent GameObject exists
                if (parentDictionary[key] != null)
                {
                    item.transform.SetParent(parentDictionary[key].transform);
                }
                else
                {
                    Debug.LogWarning($"Parent GameObject for key '{key}' is null or destroyed.");
                    item.transform.SetParent(null); // Set parent to null if the parent GameObject is null or destroyed
                }

                return (T)item;
            }
            else if (poolDictionary[key].Count == 0)
            {
                // If the pool is empty, instantiate a new object and add it to the pool
                T newInstance = Object.Instantiate(poolLookup[key]) as T;
                // newInstance.gameObject.SetActive(true);
                poolDictionary[key].Enqueue(newInstance);

                var item = poolDictionary[key].Dequeue();
                item.gameObject.SetActive(true);

                // Set parent of the object if parent GameObject exists
                if (parentDictionary[key] != null)
                {
                    item.transform.SetParent(parentDictionary[key].transform);
                }
                else
                {
                    Debug.LogWarning($"Parent GameObject for key '{key}' is null or destroyed.");
                    item.transform.SetParent(null); // Set parent to null if the parent GameObject is null or destroyed
                }
                return (T)item;
            }
            else
            {
                Debug.LogWarning($"Pool for key '{key}' is empty or does not exist.");
                return null;
            }
        }
        else
        {
            // Pool doesn't not exist
            // Need to Create a pool and instantiate a new object and add it to the pool
            Debug.LogWarning($"Pool for key '{key}' is empty or does not exist.");
            return null;
        }
        
    }

    // Method to instantiate a new object and add it to the pool
    public static T EnqueueNewInstance<T>(T item, string key) where T : Component
    {
        T newInstance = Object.Instantiate(item);
        newInstance.gameObject.SetActive(false);
        newInstance.transform.position = Vector3.zero;

        // Set parent of the object if parent GameObject exists
        if (parentDictionary.ContainsKey(key) && parentDictionary[key] != null)
        {
            newInstance.transform.SetParent(parentDictionary[key].transform);
        }
        else
        {
            Debug.LogWarning($"Parent GameObject for key '{key}' is null or destroyed.");
            newInstance.transform.SetParent(null); // Set parent to null if the parent GameObject is null or destroyed
        }

        // Add new object to the pool and set its name
        poolDictionary[key].Enqueue(newInstance);
        newInstance.gameObject.name = $"{key} Instance";

        return newInstance;
    }

    // Method to set up the initial pool for a prefab with a specific size
    public static void SetupPool<T>(T pooledItemPrefab, int poolSize, string dictionaryEntry) where T : Component
    {
        if (poolDictionary.ContainsKey(dictionaryEntry))
        {
            poolDictionary[dictionaryEntry].Clear(); // Clear existing pool if it already exists
        }
        else
        {
            poolDictionary.Add(dictionaryEntry, new Queue<Component>()); // Create a new pool queue if it doesn't exist
        }

        poolLookup[dictionaryEntry] = pooledItemPrefab; // Add or update the prefab in the lookup dictionary

        // Create a parent GameObject for the pooled objects if it doesn't exist
        if (!parentDictionary.ContainsKey(dictionaryEntry) || parentDictionary[dictionaryEntry] == null)
        {
            GameObject parent = new GameObject(dictionaryEntry + " Pool");
            parentDictionary[dictionaryEntry] = parent;
        }
    }

    // Method to set up the initial pool for a prefab without specifying size
    public static void SetupPool<T>(T pooledItemPrefab, string dictionaryEntry) where T : Component
    {
        if (poolDictionary.ContainsKey(dictionaryEntry))
        {
            poolDictionary[dictionaryEntry].Clear(); // Clear existing pool if it already exists
        }
        else
        {
            poolDictionary.Add(dictionaryEntry, new Queue<Component>()); // Create a new pool queue if it doesn't exist
        }

        poolLookup[dictionaryEntry] = pooledItemPrefab; // Add or update the prefab in the lookup dictionary

        // Create a parent GameObject for the pooled objects if it doesn't exist
        if (!parentDictionary.ContainsKey(dictionaryEntry) || parentDictionary[dictionaryEntry] == null)
        {
            GameObject parent = new GameObject(dictionaryEntry + " Pool");
            parentDictionary[dictionaryEntry] = parent;
        }
    }

    // Method to clear all pools and destroy parent GameObjects
    public static void ClearPools()
    {
        poolLookup.Clear();
        poolDictionary.Clear();

        // Destroy all parent GameObjects
        foreach (var pair in parentDictionary)
        {
            if (pair.Value != null)
            {
                GameObject.Destroy(pair.Value);
            }
        }
        parentDictionary.Clear();
    }

    // Method to check if there are inactive objects in a specific pool
    public static bool HasInactiveObjects(string key)
    {
        if (poolDictionary.ContainsKey(key))
        {
            foreach (var item in poolDictionary[key])
            {
                if (!item.gameObject.activeSelf)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
