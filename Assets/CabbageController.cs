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
        // Calculate the direction towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Move the object towards the target position
        GetComponent<Rigidbody>().velocity = direction * speed;

        // Rotate the object to face the movement direction (optional)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        lifetime = 3f;
    }

    void Update()
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
    public void ResetCabbage()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        lifetime = 3f;
    }
}
