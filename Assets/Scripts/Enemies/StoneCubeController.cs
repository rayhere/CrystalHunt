using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneCubeController : MonoBehaviour
{
    private Vector3 offset;

    [SerializeField] public Transform[] targets; // Target positions for cube movement
    [SerializeField] public bool moveRandomWithoutTarget = true; // Choose a random target or move randomly without target
    [SerializeField] public float desiredRangeDistance = 60f; // Desired range distance between target
    [SerializeField] public int maxConsecutiveMoves = 3; // Maximum consecutive moves

    public GameObject cube;
    public GameObject center;
    private GameObject forward;
    private GameObject back;
    private GameObject left;
    private GameObject right;

    private GameObject emptyGameObject; // Allow to control this object even if cube returns to pool

    public float lengthOfCube = 10f;

    public int step = 9;
    public float speed = 0.1f;

    public bool input = false;

    public CoordinatePlane coordinatePlane;
    public string objName; // Name of the object

    private GridStat gridStat; // Reference to the GridStat component

    // Audio variables
    public AudioClip impactSoundClip;
    private AudioSource audioSource;
    public float impactSoundVolume = 0.1f; // Adjust this value to set impact sound volume

    // Variable for start timer delay
    public float startTimerDelay = 2f; // Default delay time

    // Variable for MoveCloseToTarget delay
    [SerializeField, Tooltip("Min Value is time of (speed * step)")]
    public float moveCloseToTargetDelay = 2f; // Default delay time


    private void Awake()
    {
        gridStat = GetComponent<GridStat>(); // Get reference to the GridStat component
        coordinatePlane = GameObject.Find("CoordinateMap").GetComponent<CoordinatePlane>();

        CreateEmptyObject();
        InitializeAudioSource();
        
    }

    void Start()
    {
        StartCoroutine(StartMovementTimer());
    }

    void Update()
    {
        if (coordinatePlane != null && input)
        {
            StartCoroutine(MoveCloseToTarget());
        }
        else if (coordinatePlane == null)
        {
            Debug.LogError("CoordinatePlane reference not set!");
        }
    }

    void InitializeAudioSource()
    {
        // Initialize AudioSource for impact sound
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Initialize the Setup for Stone's Transform, movement and lifetime
    public void Initialize(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private IEnumerator MoveCloseToTarget()
    {
        input = false;

        float randomDelay = UnityEngine.Random.Range(0f, 1f); // Random delay between 0 and 1 second
        yield return new WaitForSeconds(randomDelay);

        int consecutiveMoves = 0;
        Vector3Int lastDirection = Vector3Int.zero;

        for (int moveAttempt = 0; moveAttempt < maxConsecutiveMoves; moveAttempt++)
        {// if have target, and list of Target > than 0
        // our target will be target[random] position
        // our target will be random 4 directions
            Vector3 targetPosition = targets != null && targets.Length > 0 
                ? targets[UnityEngine.Random.Range(0, targets.Length - 1)].position
                : transform.position + new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1)) * desiredRangeDistance;

            Debug.Log("targets[0].position " + targets[0].position + " New targetPosition " + targetPosition + " targets.Length " + targets.Length);

            Vector3 direction = (targetPosition - cube.transform.position).normalized;
            Debug.Log(" direction is " + direction + " targetPosition is " + targetPosition + " cube.transform.position is " + cube.transform.position);

            // if target within Range Distance
            if (Vector3.Distance(cube.transform.position, targetPosition) < desiredRangeDistance)
            {
                Debug.Log("MoveToRandomNearbyGrid method because targetPosition - cube.transform.position is "+ (targetPosition - cube.transform.position) );
                yield return StartCoroutine(MoveToRandomNearbyGrid());
                yield break; // Stops the coroutine entirely
            }
            
            // Following will be do if target out of Range Distance

            // choose move direction
            Vector3Int moveDirection = DetermineMoveDirection(direction);

            if (lastDirection == moveDirection)
            {
                consecutiveMoves++;
                if (consecutiveMoves >= maxConsecutiveMoves)
                {
                    moveDirection = AlternateDirection(moveDirection);
                    consecutiveMoves = 0;
                }
            }
            else
            {
                consecutiveMoves = 0;
            }
            Debug.Log("Let move moveDirection "+ moveDirection);
            // try to move to the chosen direction
            // return true if it moved object to the direction
            if (AttemptMove(moveDirection))
            {
                lastDirection = moveDirection;
                //break; // Exits the loop, but the coroutine continues
                yield return new WaitForSeconds(moveCloseToTargetDelay);
            }
            else
            {
                // if taret out of range distance, and didn't move object to the direction in some reasons
                // consecutiveMoves should end
                yield return new WaitForSeconds(moveCloseToTargetDelay);
                input = true;
                Debug.Log("input true, target out of range distance, and didn't move object to the direction in some reasons, moveDirection is " + moveDirection);
                yield break; // Stops the coroutine entirely
            }
        }
        // Code here will still run after the loop

        yield return new WaitForSeconds(moveCloseToTargetDelay);
        input = true;
        Debug.Log("input true, MoveCloseToTarget end");
    }

    private Vector3Int DetermineMoveDirection(Vector3 direction)
    {// direction basiclly is horizontal, return Vector3 in int (pos or neg)
    Debug.Log("Mathf.Abs(direction.x) is " + Mathf.Abs(direction.x) + " Mathf.Abs(direction.z) is " + Mathf.Abs(direction.z));
    Debug.Log("direction.x is " + direction.x + " direction.z is " + direction.z);
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            return new Vector3Int(Mathf.RoundToInt(Mathf.Sign(direction.x)), 0, 0);
        }
        return new Vector3Int(0, 0, Mathf.RoundToInt(Mathf.Sign(direction.z)));
    }

    private Vector3Int AlternateDirection(Vector3Int direction)
    {
        //return (direction == new Vector3Int(1, 0, 0) || direction == new Vector3Int(-1, 0, 0)) ? new Vector3Int(0, 0, 1) : new Vector3Int(1, 0, 0);

        // return opposit result of DetermineMoveDirection(), instead of simple swap
        if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z))
        {
            return new Vector3Int(Mathf.RoundToInt(Mathf.Sign(direction.x)), 0, 0);
        }
        return new Vector3Int(0, 0, Mathf.RoundToInt(Mathf.Sign(direction.z)));
    }

    private bool AttemptMove(Vector3Int moveDirection)
    {
        int newX = gridStat.x + moveDirection.x;
        int newZ = gridStat.z + moveDirection.z;

        if (coordinatePlane.IsWithinBounds(newX, newZ) && coordinatePlane.IsEmpty(newX, newZ) && (coordinatePlane.GetCheckoutTime(newX, newZ) + 1f > Time.deltaTime))
        {
            UpdateCoordinates(newX, newZ);

            // Play impact sound after a delay
            StartCoroutine(PlayImpactSoundDelayed());

            if (moveDirection == new Vector3Int(0, 0, 1))
            {
                StartCoroutine(MoveForward());
            }
            else if (moveDirection == new Vector3Int(0, 0, -1))
            {
                StartCoroutine(MoveBack());
            }
            else if (moveDirection == new Vector3Int(-1, 0, 0))
            {
                StartCoroutine(MoveLeft());
            }
            else if (moveDirection == new Vector3Int(1, 0, 0))
            {
                StartCoroutine(MoveRight());
            }

            return true;
        }

        return false;
    }

    private IEnumerator MoveToRandomNearbyGrid()
    {
        input = false;
        bool moved = false;

        int currentX = gridStat.x;
        int currentZ = gridStat.z;

        List<Vector3Int> directions = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 1),   // Forward
            new Vector3Int(0, 0, -1),  // Back
            new Vector3Int(-1, 0, 0),  // Left
            new Vector3Int(1, 0, 0)    // Right
        };

        Shuffle(directions);

        // check 4 directions, see if they is allow to move
        foreach (Vector3Int direction in directions)
        {
            int newX = currentX + direction.x;
            int newZ = currentZ + direction.z;

            // if new position is allow to move
            if (coordinatePlane.IsWithinBounds(newX, newZ) && coordinatePlane.IsEmpty(newX, newZ) && (coordinatePlane.GetCheckoutTime(newX, newZ) + 1f > Time.deltaTime))
            {
                UpdateCoordinates(newX, newZ);

                if (direction == new Vector3Int(0, 0, 1))
                {
                    yield return StartCoroutine(MoveForward());
                    moved = true;
                    break; // Exits the loop, but the coroutine continues
                }
                else if (direction == new Vector3Int(0, 0, -1))
                {
                    yield return StartCoroutine(MoveBack());
                    moved = true;
                    break; // Exits the loop, but the coroutine continues
                }
                else if (direction == new Vector3Int(-1, 0, 0))
                {
                    yield return StartCoroutine(MoveLeft());
                    moved = true;
                    break; // Exits the loop, but the coroutine continues
                }
                else if (direction == new Vector3Int(1, 0, 0))
                {
                    yield return StartCoroutine(MoveRight());
                    moved = true;
                    break; // Exits the loop, but the coroutine continues
                }
                else
                {
                    // when the direction in list cannot move


                }
                // break;
            }
        }
        // when cannot move in all 4 directions, or already moved
        yield return new WaitForSeconds(3f);
        input = true;
        Debug.Log("input true, when cannot move in all 4 directions, or already moved. moved is " + moved);
    }

    private void Shuffle(List<Vector3Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector3Int temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private IEnumerator PlayImpactSoundDelayed()
    {
        yield return new WaitForSeconds((speed*9) -0.1f); // Adjust delay time as needed

        if (impactSoundClip != null && audioSource != null)
        {
            audioSource.volume = impactSoundVolume;
            audioSource.PlayOneShot(impactSoundClip);
        }
    }

    private IEnumerator MoveForward()
    {
        yield return MoveInDirection(forward.transform.position, Vector3.right);
    }

    private IEnumerator MoveBack()
    {
        yield return MoveInDirection(back.transform.position, Vector3.left);
    }

    private IEnumerator MoveLeft()
    {
        yield return MoveInDirection(left.transform.position, Vector3.forward);
    }

    private IEnumerator MoveRight()
    {
        yield return MoveInDirection(right.transform.position, Vector3.back);
    }

    private IEnumerator MoveInDirection(Vector3 pivot, Vector3 axis)
    {
        for (int i = 0; i < (90 / step); i++)
        {
            cube.transform.RotateAround(pivot, axis, step);
            yield return new WaitForSeconds(speed);
        }
        center.transform.position = cube.transform.position;
    }

    public void CreateEmptyObject()
    {
        float halfLengthOfCube = Mathf.Abs(lengthOfCube / 2);
        emptyGameObject = new GameObject(gameObject.name + " Center");

        forward = CreateDirectionObject("Forward", new Vector3(0, -halfLengthOfCube, halfLengthOfCube));
        back = CreateDirectionObject("Back", new Vector3(0, -halfLengthOfCube, -halfLengthOfCube));
        left = CreateDirectionObject("Left", new Vector3(-halfLengthOfCube, -halfLengthOfCube, 0));
        right = CreateDirectionObject("Right", new Vector3(halfLengthOfCube, -halfLengthOfCube, 0));

        center = emptyGameObject;
    }

    private GameObject CreateDirectionObject(string name, Vector3 localPosition)
    {
        GameObject directionObject = new GameObject(name);
        directionObject.transform.parent = emptyGameObject.transform;
        directionObject.transform.localPosition = localPosition;
        return directionObject;
    }

    IEnumerator StartMovementTimer()
    {
        yield return new WaitForSeconds(startTimerDelay);
        input = true;
        Debug.Log("input true, StartMovementTimer");
    }

    public void ActivateEmptyObject()
    {
        ResetEmptyObject();
        emptyGameObject.SetActive(true);
    }

    public void DeactivateEmptyObject()
    {
        emptyGameObject.SetActive(false);
    }

    private void ResetEmptyObject()
    {
        emptyGameObject.transform.position = cube.transform.position;
        emptyGameObject.transform.rotation = cube.transform.rotation;
        emptyGameObject.transform.localScale = cube.transform.localScale;
    }

    public void SetEmptyObjectName(string objectName)
    {
        emptyGameObject.name = objectName;
    }

    private void UpdateCoordinates(int x, int z)
    {
        coordinatePlane.SetEmpty(gridStat.x, gridStat.z);
        gridStat.x = x;
        gridStat.z = z;
        coordinatePlane.SetBusy(gridStat.x, gridStat.z);
        coordinatePlane.CheckoutTime(gridStat.x, gridStat.z, 0f);
    }

    public void SetEmptyObjectToParent()
    {
        // Check if the parent of StoneCubeController object is not null
        if (transform.parent != null)
        {
            // Set the parent of emptyGameObject to be the same as the parent of StoneCubeController
            emptyGameObject.transform.SetParent(transform.parent);
        }
    }
    
    public void SetTargets(Transform[] targets)
    {
        this.targets = targets;
    }

    public void SetTarget(Transform target)
    {
        targets = new Transform[] { target };
    }
}