using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class StoneCubeController : MonoBehaviour
{
    private Vector3 offset;

    public Transform[] Targets; // Target positions for cube movement

    public GameObject _cube;
    public GameObject center;
    private GameObject forward;
    private GameObject back;
    private GameObject left;
    private GameObject right;

    private GameObject emptyGameObject; // Allow to control this object even _cube return to pool

    public float lengthOfCube = 10f;

    public int step = 9;
    public float speed = 0.01f;

    private bool input = false;

    public CoordinatePlane coordinatePlane;
    public string objName; // Name of the object


    private GridStat gridStat; // Reference to the GridStat component

    private void Awake()
    {
        gridStat = GetComponent<GridStat>(); // Get reference to the GridStat component
        // To get a reference to the CoordinatePlane component attached to a GameObject named "CoordinateMap" in the scene
        coordinatePlane = GameObject.Find("CoordinateMap").GetComponent<CoordinatePlane>();

        CreateEmptyObject();
    }

    void Start()
    {
        
        StartCoroutine(StartMovementTimer());
    }

    void Update()
    {
        if (coordinatePlane != null)
        {
            //MoveToRandomNearbyGrid();
            if (input)
            {
                // StartCoroutine(ChooseAndMoveGrid());
                // StartCoroutine(MoveToRandomNearbyGrid());
                StartCoroutine(MoveCloseToTarget());
            }
        }
        else
        {
            Debug.LogError("CoordinatePlane reference not set!");
        }

        /* if (input)
        {
            StartCoroutine(ChooseAndMove());
        } */
    }

    // Initialize the Setup for Stone's Transform, movement and lifetime
    public void Initialise(Vector3 targetPosition)
    {
        
        transform.position = targetPosition;

    }

    IEnumerator ChooseAndMove()
    {
        // Disable input while choosing and moving
        input = false;

        // Choose a random target
        int randomIndex = UnityEngine.Random.Range(0, Targets.Length);
        Vector3 targetPosition = Targets[randomIndex].position;

        // Calculate the direction to the target
        Vector3 direction = (targetPosition - _cube.transform.position).normalized;

        // Check if the target position is at Vector3.zero
        if (targetPosition == Vector3.zero)
        {
            // Choose a random direction if the target is at Vector3.zero
            direction = UnityEngine.Random.onUnitSphere;
        }

        // Determine which direction to move based on the dot product with the world axes
        float dotUp = direction.x;
        float dotDown = Vector3.Dot(direction, Vector3.down);
        float dotLeft = Vector3.Dot(direction, Vector3.left);
        float dotRight = Vector3.Dot(direction, Vector3.right);

        /* Debug.Log("direction is " + direction + " Vector3 up is " + Vector3.up);
        Debug.Log("dotUp is " + dotUp + " Vector3 up is " + Vector3.up);
        Debug.Log("dotDown is " + dotDown + " Vector3 down is " + Vector3.down);
        Debug.Log("dotLeft is " + dotLeft + " Vector3 left is " + Vector3.left);
        Debug.Log("dotRight is " + dotRight + " Vector3 right is " + Vector3.right); */

        //Debug.Log("Mathf.Abs(direction.x) " + Mathf.Abs(direction.x));
        //Debug.Log("Mathf.Abs(direction.z) " + Mathf.Abs(direction.z));
        if (Mathf.Abs(direction.x) ==0 && Mathf.Abs(direction.z) ==0 )
        {
            // don't do anything
            //Debug.Log("don't do anything " + direction);
        }
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.z))
        {
            // move Left or Right
            //Debug.Log("move Left or Right " + direction);
            if (direction.x > 0)
            {
                yield return StartCoroutine(moveRight());
            }
            else if (direction.x < 0)
            {
                yield return StartCoroutine(moveLeft());
            }
        }
        else if (Mathf.Abs(direction.x) <= Mathf.Abs(direction.z))
        {
            // move Forward or Back
            //Debug.Log("move Forward or Back " + direction);
            if (direction.z > 0)
            {
                yield return StartCoroutine(moveForward());
            }
            else if (direction.z < 0)
            {
                yield return StartCoroutine(moveBack());
            }
        }

        // Wait for 5 seconds before choosing the next target
        yield return new WaitForSeconds(3f);


        // Enable input after moving
        input = true;
    }

    IEnumerator ChooseAndMoveGridOld()
    {
        // Disable input while choosing and moving
        input = false;

        // Choose a random target
        int randomIndex = UnityEngine.Random.Range(0, Targets.Length);
        Vector3 targetPosition = Targets[randomIndex].position;

        // Calculate the direction to the target
        Vector3 direction = (targetPosition - _cube.transform.position).normalized;

        // Check if the target position is at Vector3.zero
        if (targetPosition == Vector3.zero)
        {
            // Choose a random direction if the target is at Vector3.zero
            direction = UnityEngine.Random.onUnitSphere;
        }

        if (Mathf.Abs(direction.x) ==0 && Mathf.Abs(direction.z) ==0 )
        {
            // don't do anything
            //Debug.Log("don't do anything " + direction);
        }
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.z))
        {
            // move Left or Right
            //Debug.Log("move Left or Right " + direction);
            if (direction.x > 0 && CheckGridRight())
            {
                UpdateCoordinates(gridStat.x + 1, gridStat.z);
                yield return StartCoroutine(moveRight());
            }
            else if (direction.x < 0 && CheckGridLeft())
            {
                UpdateCoordinates(gridStat.x - 1, gridStat.z);
                yield return StartCoroutine(moveLeft());
            }
        }
        else if (Mathf.Abs(direction.x) <= Mathf.Abs(direction.z))
        {
            // move Forward or Back
            //Debug.Log("move Forward or Back " + direction);
            Debug.Log("Check move Forward or Back: gridStat.x is "+ gridStat.x + ", gridStat.z is " + gridStat.z);

            if (direction.z > 0 && CheckGridForward())
            {
                UpdateCoordinates(gridStat.x, gridStat.z + 1);
                yield return StartCoroutine(moveForward());
            }
            else if (direction.z < 0 && CheckGridBack())
            {
                UpdateCoordinates(gridStat.x, gridStat.z - 1);
                yield return StartCoroutine(moveBack());
            }
        }
        else
        {
            Debug.Log("Fail to move!");
            Debug.Log("This gridStat.x " + gridStat.x + " gridStat.z " + gridStat.x );
            Debug.Log("coordinatePlane.IsWithinBounds(gridStat.x, gridStat.z - 1); " + coordinatePlane.IsWithinBounds(gridStat.x, gridStat.z - 1));
            
        }

        // Wait for 5 seconds before choosing the next target
        yield return new WaitForSeconds(3f);


        // Enable input after moving
        input = true;
    }

    IEnumerator ChooseAndMoveGrid()
    {
        // Disable input while choosing and moving
        input = false;

        // Choose a random target
        int randomIndex = UnityEngine.Random.Range(0, Targets.Length);
        Vector3 targetPosition = Targets[randomIndex].position;

        // Calculate the direction to the target
        Vector3 direction = (targetPosition - _cube.transform.position).normalized;

        // Check if the target position is at Vector3.zero
        if (targetPosition == Vector3.zero)
        {
            // Choose a random direction if the target is at Vector3.zero
            direction = UnityEngine.Random.onUnitSphere;
        }

        if (Mathf.Abs(direction.x) == 0 && Mathf.Abs(direction.z) == 0)
        {
            // don't do anything
        }
        else
        {
            // Number of consecutive moves allowed in the same direction
            int maxConsecutiveMoves = 3;
            int consecutiveMoves = 0;
            Vector3Int moveDirection = Vector3Int.zero;
            Vector3Int lastDirection = Vector3Int.zero;

            for (int moveAttempt = 0; moveAttempt < maxConsecutiveMoves; moveAttempt++)
            {
                if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.z))
                {
                    // move Left or Right
                    if (direction.x > 0 && CheckGridRight())
                    {
                        moveDirection = new Vector3Int(1, 0, 0);
                    }
                    else if (direction.x < 0 && CheckGridLeft())
                    {
                        moveDirection = new Vector3Int(-1, 0, 0);
                    }
                }
                else
                {
                    // move Forward or Back
                    if (direction.z > 0 && CheckGridForward())
                    {
                        moveDirection = new Vector3Int(0, 0, 1);
                    }
                    else if (direction.z < 0 && CheckGridBack())
                    {
                        moveDirection = new Vector3Int(0, 0, -1);
                    }
                }

                // Handle consecutive moves in the same direction
                if (lastDirection == moveDirection)
                {
                    consecutiveMoves++;
                    if (consecutiveMoves >= maxConsecutiveMoves)
                    {
                        // Choose an alternate direction if stuck
                        moveDirection = (moveDirection == new Vector3Int(1, 0, 0) || moveDirection == new Vector3Int(-1, 0, 0)) ? new Vector3Int(0, 0, 1) : new Vector3Int(1, 0, 0);
                        consecutiveMoves = 0;  // Reset consecutive moves
                    }
                }
                else
                {
                    consecutiveMoves = 0;  // Reset if changing direction
                }

                // Attempt to move in the calculated direction
                int newX = gridStat.x + moveDirection.x;
                int newZ = gridStat.z + moveDirection.z;

                if (coordinatePlane.IsWithinBounds(newX, newZ) && coordinatePlane.IsEmpty(newX, newZ) && (coordinatePlane.GetCheckoutTime(newX, newZ) + 1f > Time.deltaTime))
                {
                    UpdateCoordinates(newX, newZ);
                    if (moveDirection == new Vector3Int(0, 0, 1))
                    {
                        yield return StartCoroutine(moveForward());
                    }
                    else if (moveDirection == new Vector3Int(0, 0, -1))
                    {
                        yield return StartCoroutine(moveBack());
                    }
                    else if (moveDirection == new Vector3Int(-1, 0, 0))
                    {
                        yield return StartCoroutine(moveLeft());
                    }
                    else if (moveDirection == new Vector3Int(1, 0, 0))
                    {
                        yield return StartCoroutine(moveRight());
                    }
                    lastDirection = moveDirection;
                    break;
                }
            }

            // Wait for a short time before enabling input again
            yield return new WaitForSeconds(0.5f);
        }

        input = true;
    }


    IEnumerator moveForward()
    {
        for (int i = 0; i < (90 / step); i++)
        {
            _cube.transform.RotateAround(forward.transform.position, Vector3.right, step);
            yield return new WaitForSeconds(speed);
        }
        center.transform.position = _cube.transform.position;
    }

    IEnumerator moveBack()
    {
        for (int i = 0; i < (90 / step); i++)
        {
            _cube.transform.RotateAround(back.transform.position, Vector3.left, step);
            yield return new WaitForSeconds(speed);
        }
        center.transform.position = _cube.transform.position;
    }

    IEnumerator moveLeft()
    {
        for (int i = 0; i < (90 / step); i++)
        {
            _cube.transform.RotateAround(left.transform.position, Vector3.forward, step);
            yield return new WaitForSeconds(speed);
        }
        center.transform.position = _cube.transform.position;
    }

    IEnumerator moveRight()
    {
        for (int i = 0; i < (90 / step); i++)
        {
            // Spin the object around the right position in number of step
            _cube.transform.RotateAround(right.transform.position, Vector3.back, step);
            yield return new WaitForSeconds(speed);
        }
        center.transform.position = _cube.transform.position;
    }

    public void CreateEmptyObject()
    {
        float halfLengthOfCube = Mathf.Abs(lengthOfCube / 2);
        // Create an empty GameObject
        //GameObject emptyGameObject = new GameObject(gameObject.name + " Center");
        emptyGameObject = new GameObject(gameObject.name + " Center");
        
        // Create empty GameObjects for Forward, Back, Left, and Right
        GameObject forwardObject = new GameObject("Forward");
        GameObject backObject = new GameObject("Back");
        GameObject leftObject = new GameObject("Left");
        GameObject rightObject = new GameObject("Right");

        // assign empty Gameobjet
        center = emptyGameObject;
        forward = forwardObject;
        back = backObject;
        left = leftObject;
        right = rightObject;

        // Set the parent of Forward, Back, Left, and Right to EmptyGameObject
        forwardObject.transform.parent = emptyGameObject.transform;
        backObject.transform.parent = emptyGameObject.transform;
        leftObject.transform.parent = emptyGameObject.transform;
        rightObject.transform.parent = emptyGameObject.transform;
        
        // Set the position, rotation, and scale of EmptyGameObject to match _cube
        emptyGameObject.transform.position = _cube.transform.position;
        emptyGameObject.transform.rotation = _cube.transform.rotation;
        emptyGameObject.transform.localScale = _cube.transform.localScale;

        // Set the positions of Forward, Back, Left, and Right relative to EmptyGameObject
        forwardObject.transform.localPosition = new Vector3(0, -halfLengthOfCube, halfLengthOfCube);
        backObject.transform.localPosition = new Vector3(0, -halfLengthOfCube, -halfLengthOfCube);
        leftObject.transform.localPosition = new Vector3(-halfLengthOfCube, -halfLengthOfCube, 0);
        rightObject.transform.localPosition = new Vector3(halfLengthOfCube, -halfLengthOfCube, 0);
    }

    IEnumerator StartMovementTimer()
    {
        // Wait for 5 seconds before starting the movement
        yield return new WaitForSeconds(2f);

        // Start the movement
        input = true;
    }

    public void ActiveEmptyObj()
    {
        ResetEmptyObj();
        emptyGameObject.SetActive(true);
    }

    public void DeactiveEmptyObj()
    {
        emptyGameObject.SetActive(false);
    }

    private void ResetEmptyObj()
    {
        // Set the position, rotation, and scale of EmptyGameObject to match _cube
        emptyGameObject.transform.position = _cube.transform.position;
        emptyGameObject.transform.rotation = _cube.transform.rotation;
        emptyGameObject.transform.localScale = _cube.transform.localScale;
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
        Targets = targets;
    }

    public void SetTarget(Transform target)
    {
        Targets = new Transform[] { target };
    }

    IEnumerator MoveToRandomNearbyGrid()
    {
        // Disable input while choosing and moving
        input = false;

        // Get current grid coordinates
        int currentX = gridStat.x;
        int currentZ = gridStat.z;

        // List of possible directions to move in (Forward, Back, Left, Right)
        List<Vector3Int> directions = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 1),   // Forward
            new Vector3Int(0, 0, -1),  // Back
            new Vector3Int(-1, 0, 0),  // Left
            new Vector3Int(1, 0, 0)    // Right
        };

        // Shuffle the directions list to pick a random direction
        for (int i = 0; i < directions.Count; i++)
        {
            Vector3Int temp = directions[i];
            int randomIndex = UnityEngine.Random.Range(i, directions.Count);
            directions[i] = directions[randomIndex];
            directions[randomIndex] = temp;
        }

        // Try to move in a random direction
        foreach (Vector3Int direction in directions)
        {
            int newX = currentX + direction.x;
            int newZ = currentZ + direction.z;

            if (coordinatePlane.IsWithinBounds(newX, newZ) && coordinatePlane.IsEmpty(newX, newZ) && (coordinatePlane.GetCheckoutTime(newX, newZ) + 1f > Time.deltaTime))
            {
                // Move in the chosen direction
                if (direction == new Vector3Int(0, 0, 1))
                {
                    UpdateCoordinates(newX, newZ);
                    yield return StartCoroutine(moveForward());
                }
                else if (direction == new Vector3Int(0, 0, -1))
                {
                    UpdateCoordinates(newX, newZ);
                    yield return StartCoroutine(moveBack());
                }
                else if (direction == new Vector3Int(-1, 0, 0))
                {
                    UpdateCoordinates(newX, newZ);
                    yield return StartCoroutine(moveLeft());
                }
                else if (direction == new Vector3Int(1, 0, 0))
                {
                    UpdateCoordinates(newX, newZ);
                    yield return StartCoroutine(moveRight());
                }
                break;
            }
        }

        // Wait for 3 seconds before choosing the next target
        yield return new WaitForSeconds(3f);

        // Enable input after moving
        input = true;
    }

    IEnumerator MoveCloseToTarget()
    {
        // If the best path is not possible, choose MoveToRandomNearbyGrid for 3 turns
        // Move 3 same directions if possible, if the same direction is moved more than 3 times, choose another direction
        // If the cube and target distance are less than 60f, choose MoveToRandomNearbyGrid()
        input = false;

        // Number of consecutive moves allowed in the same direction
        int maxConsecutiveMoves = 3;
        int consecutiveMoves = 0;
        Vector3Int lastDirection = Vector3Int.zero;

        for (int moveAttempt = 0; moveAttempt < maxConsecutiveMoves; moveAttempt++)
        {
            // Calculate direction to the closest target
            Vector3 targetPosition = Targets[0].position;  // Assuming we have at least one target, can be extended for closest target
            Vector3 direction = (targetPosition - _cube.transform.position).normalized;

            // Check the distance to the target
            if (Vector3.Distance(_cube.transform.position, targetPosition) < 60f)
            {
                yield return StartCoroutine(MoveToRandomNearbyGrid());
                yield break;
            }

            // Determine the primary axis of movement (X or Z)
            Vector3Int moveDirection;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                moveDirection = new Vector3Int(Mathf.RoundToInt(Mathf.Sign(direction.x)), 0, 0);
            }
            else
            {
                moveDirection = new Vector3Int(0, 0, Mathf.RoundToInt(Mathf.Sign(direction.z)));
            }

            // Handle consecutive moves in the same direction
            if (lastDirection == moveDirection)
            {
                consecutiveMoves++;
                if (consecutiveMoves >= maxConsecutiveMoves)
                {
                    // Choose an alternate direction if stuck
                    moveDirection = (moveDirection == new Vector3Int(1, 0, 0) || moveDirection == new Vector3Int(-1, 0, 0)) ? new Vector3Int(0, 0, 1) : new Vector3Int(1, 0, 0);
                    consecutiveMoves = 0;  // Reset consecutive moves
                }
            }
            else
            {
                consecutiveMoves = 0;  // Reset if changing direction
            }

            // Attempt to move in the calculated direction
            int newX = gridStat.x + moveDirection.x;
            int newZ = gridStat.z + moveDirection.z;

            if (coordinatePlane.IsWithinBounds(newX, newZ) && coordinatePlane.IsEmpty(newX, newZ) && (coordinatePlane.GetCheckoutTime(newX, newZ) + 1f > Time.deltaTime))
            {
                UpdateCoordinates(newX, newZ);
                if (moveDirection == new Vector3Int(0, 0, 1))
                {
                    yield return StartCoroutine(moveForward());
                }
                else if (moveDirection == new Vector3Int(0, 0, -1))
                {
                    yield return StartCoroutine(moveBack());
                }
                else if (moveDirection == new Vector3Int(-1, 0, 0))
                {
                    yield return StartCoroutine(moveLeft());
                }
                else if (moveDirection == new Vector3Int(1, 0, 0))
                {
                    yield return StartCoroutine(moveRight());
                }
                lastDirection = moveDirection;
                break;
            }
        }

        // Wait for a short time before enabling input again
        yield return new WaitForSeconds(0.5f);

        input = true;
    }

    IEnumerator RandomMoveGrid()
    {
        // move 3 same direction if possible, if same direction moved more than 3 time, choose other direction
        // Disable input while choosing and moving
        input = false;

        // Number of consecutive moves allowed in the same direction
        int maxConsecutiveMoves = 3;
        int consecutiveMoves = 0;
        Vector3Int lastDirection = Vector3Int.zero;

        while (true)
        {
            // List of possible directions to move in (Forward, Back, Left, Right)
            List<Vector3Int> directions = new List<Vector3Int>
            {
                new Vector3Int(0, 0, 1),   // Forward
                new Vector3Int(0, 0, -1),  // Back
                new Vector3Int(-1, 0, 0),  // Left
                new Vector3Int(1, 0, 0)    // Right
            };

            // Shuffle the directions list to pick a random direction
            for (int i = 0; i < directions.Count; i++)
            {
                Vector3Int temp = directions[i];
                int randomIndex = UnityEngine.Random.Range(i, directions.Count);
                directions[i] = directions[randomIndex];
                directions[randomIndex] = temp;
            }

            // Try to move in a random direction
            Vector3Int currentDirection = directions[0];
            foreach (Vector3Int direction in directions)
            {
                if (currentDirection == direction)
                {
                    consecutiveMoves++;
                    if (consecutiveMoves >= maxConsecutiveMoves)
                    {
                        // Choose an alternate direction if stuck
                        currentDirection = (direction == new Vector3Int(1, 0, 0) || direction == new Vector3Int(-1, 0, 0)) ? new Vector3Int(0, 0, 1) : new Vector3Int(1, 0, 0);
                        consecutiveMoves = 0; // Reset consecutive moves
                    }
                }
            }

            // Move in the chosen direction
            if (currentDirection == new Vector3Int(0, 0, 1))
            {
                UpdateCoordinates(gridStat.x + currentDirection.x, gridStat.z + currentDirection.z);
                yield return StartCoroutine(moveForward());
            }
            else if (currentDirection == new Vector3Int(0, 0, -1))
            {
                UpdateCoordinates(gridStat.x + currentDirection.x, gridStat.z + currentDirection.z);
                yield return StartCoroutine(moveBack());
            }
            else if (currentDirection == new Vector3Int(-1, 0, 0))
            {
                UpdateCoordinates(gridStat.x + currentDirection.x, gridStat.z + currentDirection.z);
                yield return StartCoroutine(moveLeft());
            }
            else if (currentDirection == new Vector3Int(1, 0, 0))
            {
                UpdateCoordinates(gridStat.x + currentDirection.x, gridStat.z + currentDirection.z);
                yield return StartCoroutine(moveRight());
            }
            
            // Wait for a short time before enabling input again
            yield return new WaitForSeconds(0.5f);

            // Enable input after moving
            input = true;
        }
    }


    // Check if coordinates on Forward is within bounds
    private bool CheckGridForward()
    {
        // Vector3.forward Shorthand for writing Vector3(0, 0, 1)
        // Check IsWithinBounds, IsEmpty, and GetCheckoutTime 1sec passed
        return coordinatePlane.IsWithinBounds(gridStat.x, gridStat.z + 1) && coordinatePlane.IsEmpty(gridStat.x, gridStat.z + 1) && (coordinatePlane.GetCheckoutTime(gridStat.x, gridStat.z + 1)+1f > Time.deltaTime);
    }

    // Check if coordinates on Back is within bounds
    private bool CheckGridBack()
    {
        // Vector3.back Shorthand for writing Vector3(0, 0, -1)
        // Check IsWithinBounds, IsEmpty, and GetCheckoutTime 1sec passed
        Debug.Log("coordinatePlane.IsWithinBounds(gridStat.x, gridStat.z - 1) is " + coordinatePlane.IsWithinBounds(gridStat.x, gridStat.z - 1));
        Debug.Log("coordinatePlane.IsEmpty(gridStat.x, gridStat.z - 1) is " + coordinatePlane.IsEmpty(gridStat.x, gridStat.z - 1));
        Debug.Log("(coordinatePlane.GetCheckoutTime(gridStat.x, gridStat.z - 1)+1f > Time.deltaTime) is " + (coordinatePlane.GetCheckoutTime(gridStat.x, gridStat.z - 1)+1f > Time.deltaTime));
        
        return coordinatePlane.IsWithinBounds(gridStat.x, gridStat.z - 1) && coordinatePlane.IsEmpty(gridStat.x, gridStat.z - 1) && (coordinatePlane.GetCheckoutTime(gridStat.x, gridStat.z - 1)+1f > Time.deltaTime);
    }

    // Check if coordinates on Left is within bounds
    private bool CheckGridLeft()
    {
        // Vector3.left Shorthand for writing Vector3(-1, 0, 0)
        // Check IsWithinBounds, IsEmpty, and GetCheckoutTime 1sec passed
        return coordinatePlane.IsWithinBounds(gridStat.x - 1, gridStat.z) && coordinatePlane.IsEmpty(gridStat.x - 1, gridStat.z) && (coordinatePlane.GetCheckoutTime(gridStat.x - 1, gridStat.z)+1f > Time.deltaTime);
    }

    // Check if coordinates on Right is within bounds
    private bool CheckGridRight()
    {
        // Vector3.right Shorthand for writing Vector3(1, 0, 0)
        // Check IsWithinBounds, IsEmpty, and GetCheckoutTime 1sec passed
        return coordinatePlane.IsWithinBounds(gridStat.x + 1, gridStat.z) && coordinatePlane.IsEmpty(gridStat.x + 1, gridStat.z) && (coordinatePlane.GetCheckoutTime(gridStat.x + 1, gridStat.z)+1f > Time.deltaTime);

    }

    private void UpdateCoordinates(int x, int z)
    {
        // Set Current gridStat.x and gridStat.z IsEmpty in CoordinatePlane
        coordinatePlane.SetGridUnitInfo(gridStat.x, gridStat.z, true, "", Time.deltaTime);

        // Set Destination Grid x, z in CoordinatePlane
        coordinatePlane.SetGridUnitInfo(x, z, false, gameObject.name);

        // Update GridStat
        gridStat.x = x;
        gridStat.z = z;
    }

}
