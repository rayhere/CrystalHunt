using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalController : MonoBehaviour
{
    [SerializeField] public GameObject _crystal;

    [SerializeField] Animator animator;
    // Wanna play clip that smaller the scale while picking up crystal
    private const string ShrinkToOriginal = "ShrinkToOriginal";

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
        // transform.position = targetPosition;
        transform.position = new Vector3 (targetPosition.x, targetPosition.y + 0.5f, targetPosition.z);
    }
    public void Initialise()
    {
        
    }

    public void ReturnToPool()
    {
        /* // Return this crystal to the object pool
        ObjectPooler.EnqueueObject(this, "Crystal"); */

        StartCoroutine(ConfirmationReturnToPool());
    }

    private IEnumerator ConfirmationReturnToPool()
    {
        Debug.Log("Crystal ConfirmationReturnToPool");
        /* ttime = Time.time;
        if (canDestory && timer < Time.time)
        {
            // Back to Object Pool
            ReturnToPool();   
        } */

        float startTime = Time.time;
        while (Time.time <= startTime + 0.5f)
        {
            //yield return null; // Wait until 1 second has passed
            yield return new WaitForSeconds(0.5f);
        }

        // Return this crystal to the object pool
        ObjectPooler.EnqueueObject(this, "Crystal");
        Debug.Log("Crystal return to pool");
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
    // Handle collision with player or Darkness
    public void PlayerCollisionDetected(Collision collision)
    {
        // Ensure collision with player or Darkness
        if (!(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Darkness") || collision.gameObject.CompareTag("Kazuma_Satou")))
            return;
        
        // Run your desired method here
        Debug.Log("Player collision detected!");

        //If the GameObject has the same tag as specified, output this message in the console
        // If crystal can be destroyed, skip further actions
        if (canDestory) return;

        // Trigger shrinking animation
        // Set the isShrinkToOriginal parameter to true
        //animator.SetBool("isShrinkToOriginal", true);

        //animator.SetTrigger(ShrinkToOriginal);

        //animator.SetInteger("popped", POPPED);
        //Debug.Log("Popped: " + POPPED);

        //2. play sound effect
        //AudioSource.PlayClipAtPoint(_audio.clip, transform.position);

        //audio.Play() -- this will work but won't work if coin gets destroyed before audio is played.

        

        // 3. Increase the number of crystals collected for player in PlayerPrefs
        int amount = GetCrystalCount();
        IncreaseCrystalCount(amount);

        // 4. Increase crystal count in PersistentData if available
        if (PersistentData.Instance != null)
        {
            PersistentData.Instance.IncreaseCrystalCount(1);
            Debug.Log("Crystal collected! Total: " + PersistentData.Instance.GetCrystalCount());
        }
        else
        {
            Debug.LogWarning("PersistentData instance not found. Crystal count not updated.");
        }

        // 5. Set collider to trigger true
        SetColliderTrigger(_crystal, false);

        // 6. Gradually reduce scale over time using coroutine
        StartCoroutine(ReduceScaleOverTime());
        
        // Optional: Destroy the crystal object after some delay
        // Destroy(gameObject, 1f); // not going to destory, return to pool
        canDestory = true;

        ReturnToPool();
    }

    void SetColliderTrigger(GameObject _crystal, bool isTrigger)
    {
        // Get the collider component attached to _crystal
        Collider crystalCollider = _crystal.GetComponent<Collider>();

        // Ensure crystalCollider is not null before setting properties
        if (crystalCollider != null)
        {
            // Initially set the collider to not be a trigger
            crystalCollider.isTrigger = isTrigger;
        }
        else
        {
            Debug.LogError("Collider component not found on _crystal GameObject.");
        }
    }

    // Coroutine to reduce scale over time
    private IEnumerator ReduceScaleOverTime()
    {
        // temp set it back to this object for adjust
        GameObject _crystal = gameObject;

        float duration = 1.0f; // Duration over which to reduce scale
        float timer = 0.0f;

        Vector3 initialScale = _crystal.transform.localScale;
        Vector3 targetScale = Vector3.zero; // Scale to reduce to (you can adjust this)

        while (timer < duration)
        {
            timer += Time.deltaTime;
            _crystal.transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / duration);
            yield return null;
        }

        // Ensure final scale is exactly the target scale
        _crystal.transform.localScale = targetScale;
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

        //2. play sound effect
        //AudioSource.PlayClipAtPoint(_audio.clip, transform.position);

        //audio.Play() -- this will work but won't work if coin gets destroyed before audio is played.

        canDestory = true;

        // 3. Increase the number of crystals collected for player in PlayerPrefs
        int amount = GetCrystalCount();
        IncreaseCrystalCount(amount);


        // 4. Increase crystal count in PersistentData if available
        if (PersistentData.Instance != null)
        {
            PersistentData.Instance.IncreaseCrystalCount(1);
            Debug.Log("Crystal collected! Total: " + PersistentData.Instance.GetCrystalCount());
        }
        else
        {
            Debug.LogWarning("PersistentData instance not found. Crystal count not updated.");
        }
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
