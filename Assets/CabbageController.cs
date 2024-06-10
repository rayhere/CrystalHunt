using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabbageController : MonoBehaviour
{
    float speed = 5f;
    float lifetime = 3f;

    // Initialize the cabbage's movement and lifetime
    public void Initialise(Vector3 targetPosition)
    {
        
        FountainDirectionMovement();

    }


    void FountainDirectionMovement()
    {
        // Calculate the direction towards the target position
        //Vector3 direction = (targetPosition - transform.position).normalized;

        Vector3 direction = Vector3.up;

        // Move the object towards the target position
        GetComponent<Rigidbody>().velocity = direction * speed;

        // Rotate the object to face the movement direction (optional)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        lifetime = 3f;
    }
            
    void Update()
    {
        ReturnToPool();
    }

    void ReturnToPool()
    {
        // Reduce lifetime
        lifetime -= Time.deltaTime;

        // If lifetime is expired, return the cabbage to the object pool
        if (lifetime <= 0f)
        {
            ObjectPooler.EnqueueObject(this, "Cabbage");
        }
    }

    // Reset the cabbage's state when returning to the pool
    // EnqueueObject<T> will Reset the cabbage's state in ObjectPooler.cs
}
