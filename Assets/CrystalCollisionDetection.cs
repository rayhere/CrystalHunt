using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalCollisionDetection : MonoBehaviour
{
    [SerializeField]
    public CrystalController crystalController;

    private void Awake()
    {
        // Try to find CrystalController on the current GameObject
        crystalController = GetComponent<CrystalController>();

        // If crystalController is null and this object has a parent, try to find it in the parent
        if (crystalController == null && transform.parent != null)
        {
            crystalController = transform.parent.GetComponent<CrystalController>();

            // If still null, try to find it in the parent's parent (grandparent)
            if (crystalController == null && transform.parent.parent != null)
            {
                crystalController = transform.parent.parent.GetComponent<CrystalController>();
            }
        }
    }

    // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
    private void OnCollisionEnter(Collision collision)
    {
        //Detecting Collisions with a certain tag

        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        // if (collision.gameObject.tag == "Player")
        if (collision.gameObject.CompareTag("Player")) // Check if collided with the player
        {
            crystalController.PlayerCollisionDetected(collision);
        }
        else
        {
            //collision from other GameObject
            Debug.Log("other collision detected");
        }

        
        /* // Check if the collision is with a specific tag or layer if needed
        // Example: if (collision.gameObject.CompareTag("Player")) { ... }

        // Get the parent object of the collider this script is attached to
        GameObject parentObject = transform.parent.gameObject;

        // Notify the parent object (you can call a method on the parent object or set a flag)
        parentObject.GetComponent<ParentScript>().NotifyCollisionDetected(); */
    }

}
