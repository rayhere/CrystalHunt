using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessTriggerHandler : MonoBehaviour
{
    [Header("References")]
    public PlayerStatsSO playerStats; // Reference to the ScriptableObject

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Crystal")
        {
            print("Crystal Enter");
            // Crystal destory
            // Crystal collection update
            // Check if the collided object is a Crystal


            CrystalController crystalController = other.GetComponent<CrystalController>();

            

            // Store the reference for later reactivation if needed
            // For example, you can store it in a list or use a callback from ObjectPooler
            crystalController.ReturnToPool(); // Assuming ObjectPooler has this method

            // Set the CrystalController to inactive state
            //crystalController.gameObject.SetActive(false);
            // Better do it in CrystalController.cs
            
        }

        if (other.gameObject.tag == "StoneCube")
        {
            print("StoneCube Stay");
            playerStats.currentHP -= 10; // Example: Taking damage
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Crystal")
        {
            print("Crystal Stay");
        }

        if (other.gameObject.tag == "StoneCube")
        {
            print("StoneCube Stay");
            playerStats.currentHP -= 10; // Example: Taking damage
        }
        

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Crystal")
        {
            print("Crystal Exit");
        }

        if (other.gameObject.tag == "StoneCube")
        {
            print("StoneCube Stay");
            playerStats.currentHP -= 10; // Example: Taking damage
        }
    }
}
