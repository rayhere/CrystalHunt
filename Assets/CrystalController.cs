using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalController : MonoBehaviour
{
    [SerializeField] public GameObject _crystal;

    [SerializeField] Animator animator;
    // Wanna play clip that smaller the scale while picking up crystal

    [SerializeField] AudioSource _audio;



    private const string CrystalCountKey = "CrystalCount";

    const int UNPOPPED = 0;
    const int POPPED = 1;
    public float timer = .5f;
    public bool canDestory = false;
    public float ttime;
    public Vector3 maxScale;
    public bool collisionTriggered = false;
    public float swingForce;
    public float swingTime;

    private int goLeft = 0;



    void Start()
    {
        animator = GetComponent<Animator>();
        //animator.SetInteger("popped", UNPOPPED);

    }

    void Update()
    {

        // OnCollisionEnter is automatically called by Unity's physics engine when a collision or trigger event occurs
    }

    void UpdateCrystal()
    {

    }

    // Initialize the Crystal's Spawn Position
    public void Initialise(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }
    public void Initialise()
    {
        
    }

    public void ReturnToPool()
    {
        // Return this crystal to the object pool
        ObjectPooler.EnqueueObject(this, "Crystal");
    }

    private void ConfirmationReturnToPool()
    {
        ttime = Time.time;
        if (canDestory && timer < Time.time)
        {
            // Back to Object Pool
            ReturnToPool();   
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Detecting Collisions with a certain tag

        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        // if (collision.gameObject.tag == "Player")
        /* if (collision.gameObject.CompareTag("Player")) // Check if collided with the player
        {
            PlayerCollisionDetected();
        }
        else
        {
            //collision from other GameObject
            Debug.Log("Crystal collision detected");
        } */
    }

    // Method to run when the player triggers the collision
    public void PlayerCollisionDetected(Collision collision)
    {
        // Run your desired method here
        Debug.Log("Player collision detected!");

        //If the GameObject has the same tag as specified, output this message in the console
        if (canDestory) return;

        // Set the isShrinkToOriginal parameter to true
        animator.SetBool("isShrinkToOriginal", true);

        //animator.SetInteger("popped", POPPED);
        //Debug.Log("Popped: " + POPPED);

        Debug.Log("timer: " + timer);
        Debug.Log("deltaTime: " + Time.deltaTime);
        Debug.Log("Time.time: " + Time.time);
    
        timer += Time.time;

        Debug.Log("new timer: " + timer);

        //2. play sound effect
        //AudioSource.PlayClipAtPoint(_audio.clip, transform.position);

        //audio.Play() -- this will work but won't work if coin gets destroyed before audio is played.

        canDestory = true;

        // 3. Increase the number of crystals collected for player in PlayerPrefs
        int amount = GetCrystalCount();
        IncreaseCrystalCount(amount);
    }

    // Method to run when the player triggers the collision
    void PlayerCollisionDetected()
    {
        // Run your desired method here
        Debug.Log("Player collision detected!");

        //If the GameObject has the same tag as specified, output this message in the console
        if (canDestory) return;

        // Set the isShrinkToOriginal parameter to true
        animator.SetBool("isShrinkToOriginal", true);

        //animator.SetInteger("popped", POPPED);
        //Debug.Log("Popped: " + POPPED);

        Debug.Log("timer: " + timer);
        Debug.Log("deltaTime: " + Time.deltaTime);
        Debug.Log("Time.time: " + Time.time);
    
        timer += Time.time;

        Debug.Log("new timer: " + timer);

        //2. play sound effect
        //AudioSource.PlayClipAtPoint(_audio.clip, transform.position);

        //audio.Play() -- this will work but won't work if coin gets destroyed before audio is played.

        canDestory = true;

        // 3. Increase the number of crystals collected for player in PlayerPrefs
        int amount = GetCrystalCount();
        IncreaseCrystalCount(amount);
    }

    // Method to increase the number of crystals collected
    public static void IncreaseCrystalCount(int amount)
    {
        int currentCount = PlayerPrefs.GetInt(CrystalCountKey, 0);
        int newCount = currentCount + amount;
        PlayerPrefs.SetInt(CrystalCountKey, newCount);
        PlayerPrefs.Save(); // Save changes
    }

    // Method to get the current number of crystals collected
    public static int GetCrystalCount()
    {
        return PlayerPrefs.GetInt(CrystalCountKey, 0);
    }
}
