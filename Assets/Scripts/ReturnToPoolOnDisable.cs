using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPoolOnDisable : MonoBehaviour
{
    public ObjectPool Pool; // Reference to the pool
    public PoolObjectType PoolType; // Type of the object pool

    private void OnDisable()
    {
        // When the object is deactivated, return it to the pool
        switch (PoolType)
        {
            case PoolObjectType.Crystal:
                Pool.ReturnCrystal(gameObject);
                break;
            case PoolObjectType.Cabbage:
                Pool.ReturnCabbage(gameObject);
                break;
            case PoolObjectType.StoneCube:
                Pool.ReturnStoneCube(gameObject);
                break;
            default:
                Debug.LogError("Unknown pool type.");
                break;
        }
    }


    /* public ObjectPool Pool; // Reference to the pool
    public PoolObjectType PoolType; // Type of the object pool

    private void OnDisable()
    {
        // When the object is deactivated, return it to the pool
        switch (PoolType)
        {
            case PoolObjectType.Crystal:
                Pool.ReturnCrystal(gameObject);
                break;
            case PoolObjectType.Cabbage:
                Pool.ReturnCabbage(gameObject);
                break;
            case PoolObjectType.StoneCube:
                Pool.ReturnStoneCube(gameObject);
                break;
            default:
                Debug.LogError("Unknown pool type.");
                break;
        }
    } */
}

/* public enum PoolObjectType
{
    Crystal,
    Cabbage,
    StoneCube
} */