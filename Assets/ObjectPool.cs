using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject crystalPrefab;
    public GameObject enemyPrefab;
    public int initialPoolSize = 10;

    private Queue<GameObject> crystalPool = new Queue<GameObject>();
    private Queue<GameObject> enemyPool = new Queue<GameObject>();

    private void Start()
    {
        InitializePool(crystalPrefab, crystalPool);
        InitializePool(enemyPrefab, enemyPool);
    }

    private void InitializePool(GameObject prefab, Queue<GameObject> pool)
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetCrystal()
    {
        if (crystalPool.Count == 0)
        {
            GameObject newObj = Instantiate(crystalPrefab, transform);
            newObj.SetActive(false);
            crystalPool.Enqueue(newObj);
        }
        return crystalPool.Dequeue();
    }

    public void ReturnCrystal(GameObject crystal)
    {
        crystalPool.Enqueue(crystal);
        crystal.SetActive(false);
    }

    public GameObject GetEnemy()
    {
        if (enemyPool.Count == 0)
        {
            GameObject newObj = Instantiate(enemyPrefab, transform);
            newObj.SetActive(false);
            enemyPool.Enqueue(newObj);
        }
        return enemyPool.Dequeue();
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemyPool.Enqueue(enemy);
        enemy.SetActive(false);
    }
}

