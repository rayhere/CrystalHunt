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
    private GameObject up;
    private GameObject down;
    private GameObject left;
    private GameObject right;

    private GameObject emptyGameObject; // Allow to control this object even _cube return to pool

    public float lengthOfCube = 10f;

    public int step = 9;
    public float speed = 0.01f;

    private bool input = false;

    public CoordinatesTable coordinatesTable;
    public string objName; // Name of the object


    private GridStat gridStat; // Reference to the GridStat component

    private void Awake()
    {
        gridStat = GetComponent<GridStat>(); // Get reference to the GridStat component
        // To get a reference to the CoordinatesTable component attached to a GameObject named "CoordinateMap" in the scene
        coordinatesTable = GameObject.Find("CoordinateMap").GetComponent<CoordinatesTable>();

        CreateEmptyObject();
    }

    void Start()
    {
        
        StartCoroutine(StartMovementTimer());
    }

    void Update()
    {
        if (coordinatesTable != null)
        {
            //MoveToRandomNearbyGrid();
            if (input)
            {
                StartCoroutine(ChooseAndMoveGrid());
            }
        }
        else
        {
            Debug.LogError("CoordinatesTable reference not set!");
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
            // move Up or Down
            //Debug.Log("move Up or Down " + direction);
            if (direction.z > 0)
            {
                yield return StartCoroutine(moveUP());
            }
            else if (direction.z < 0)
            {
                yield return StartCoroutine(moveDown());
            }
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
            // move Up or Down
            //Debug.Log("move Up or Down " + direction);
            Debug.Log("CheckGridDown() is "+ CheckGridDown());

            if (direction.z > 0 && CheckGridUp())
            {
                UpdateCoordinates(gridStat.x, gridStat.z + 1);
                yield return StartCoroutine(moveUP());
            }
            else if (direction.z < 0 && CheckGridDown())
            {
                UpdateCoordinates(gridStat.x, gridStat.z - 1);
                yield return StartCoroutine(moveDown());
            }
        }
        else
        {
            Debug.Log("Fail to move!");
            Debug.Log("This gridStat.x " + gridStat.x + " gridStat.z " + gridStat.x );
            Debug.Log("coordinatesTable.IsWithinBounds(gridStat.x, gridStat.z - 1); " + coordinatesTable.IsWithinBounds(gridStat.x, gridStat.z - 1));
            
        }

        // Wait for 5 seconds before choosing the next target
        yield return new WaitForSeconds(3f);


        // Enable input after moving
        input = true;
    }

    IEnumerator moveUP()
    {
        for (int i

 = 0; i < (90 / step); i++)
        {
            _cube.transform.RotateAround(up.transform.position, Vector3.right, step);
            yield return new WaitForSeconds(speed);
        }
        center.transform.position = _cube.transform.position;
    }

    IEnumerator moveDown()
    {
        for (int i = 0; i < (90 / step); i++)
        {
            _cube.transform.RotateAround(down.transform.position, Vector3.left, step);
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
        lengthOfCube = Mathf.Abs(lengthOfCube / 2);
        // Create an empty GameObject
        //GameObject emptyGameObject = new GameObject(gameObject.name + " Center");
        emptyGameObject = new GameObject(gameObject.name + " Center");
        
        // Create empty GameObjects for Up, Down, Left, and Right
        GameObject upObject = new GameObject("Up");
        GameObject downObject = new GameObject("Down");
        GameObject leftObject = new GameObject("Left");
        GameObject rightObject = new GameObject("Right");

        // assign empty Gameobjet
        center = emptyGameObject;
        up = upObject;
        down = downObject;
        left = leftObject;
        right = rightObject;

        // Set the parent of Up, Down, Left, and Right to EmptyGameObject
        upObject.transform.parent = emptyGameObject.transform;
        downObject.transform.parent = emptyGameObject.transform;
        leftObject.transform.parent = emptyGameObject.transform;
        rightObject.transform.parent = emptyGameObject.transform;
        
        // Set the position, rotation, and scale of EmptyGameObject to match _cube
        emptyGameObject.transform.position = _cube.transform.position;
        emptyGameObject.transform.rotation = _cube.transform.rotation;
        emptyGameObject.transform.localScale = _cube.transform.localScale;

        // Set the positions of Up, Down, Left, and Right relative to EmptyGameObject
        upObject.transform.localPosition = new Vector3(0, -lengthOfCube, lengthOfCube);
        downObject.transform.localPosition = new Vector3(0, -lengthOfCube, -lengthOfCube);
        leftObject.transform.localPosition = new Vector3(-lengthOfCube, -lengthOfCube, 0);
        rightObject.transform.localPosition = new Vector3(lengthOfCube, -lengthOfCube, 0);
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

    private void MoveToRandomNearbyGrid()
    {
        // Read grid position from GridStat script
        Vector2Int currentGridPosition = coordinatesTable.WorldToGridCoordinates(transform.position);


        // Check each of the four adjacent grid positions
        Vector2Int[] directions = new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };


        foreach (Vector2Int direction in directions)
        {
            Vector2Int targetGridPosition = currentGridPosition + direction;


            if (coordinatesTable.IsWithinBounds(targetGridPosition.x, targetGridPosition.y))
            {
                bool targetIsEmpty = coordinatesTable.IsEmpty(targetGridPosition.x, targetGridPosition.y);
                bool targetIsNotTooOld = coordinatesTable.GetGridUnit(targetGridPosition.x, targetGridPosition.y).checkoutTime + 1f > Time.deltaTime;


                if (targetIsEmpty && targetIsNotTooOld)
                {
                    // Move to the target grid position
                    transform.position = coordinatesTable.GridToWorldCoordinates(targetGridPosition.x, targetGridPosition.y);


                    // Update grid status for the target position
                    coordinatesTable.SetGridUnitInfo(targetGridPosition.x, targetGridPosition.y, false, objName, 0f);


                    // Update grid status for the current position (0,0)
                    coordinatesTable.SetGridUnitInfo(currentGridPosition.x, currentGridPosition.y, true, "", Time.deltaTime);


                    // Update GridStat component attached to the cube object
                    if (gridStat != null)
                    {
                        gridStat.x = targetGridPosition.x;
                        gridStat.y = targetGridPosition.y;
                    }


                    // Exit the loop after moving to the first available position
                    return;
                }
            }
        }
    }

    // Check if coordinates on Up is within bounds
    private bool CheckGridUp()
    {
        // Check IsWithinBounds, IsEmpty, and GetCheckoutTime 1sec passed
        return coordinatesTable.IsWithinBounds(gridStat.x, gridStat.z + 1) && coordinatesTable.IsEmpty(gridStat.x, gridStat.z + 1) && (coordinatesTable.GetCheckoutTime(gridStat.x, gridStat.z + 1)+1f > Time.deltaTime);
    }

    // Check if coordinates on Down is within bounds
    private bool CheckGridDown()
    {
        // Check IsWithinBounds, IsEmpty, and GetCheckoutTime 1sec passed
        Debug.Log("coordinatesTable.IsWithinBounds(gridStat.x, gridStat.z - 1) is " + coordinatesTable.IsWithinBounds(gridStat.x, gridStat.z - 1));
        Debug.Log("coordinatesTable.IsEmpty(gridStat.x, gridStat.z - 1) is " + coordinatesTable.IsEmpty(gridStat.x, gridStat.z - 1));
        Debug.Log("(coordinatesTable.GetCheckoutTime(gridStat.x, gridStat.z - 1)+1f > Time.deltaTime) is " + (coordinatesTable.GetCheckoutTime(gridStat.x, gridStat.z - 1)+1f > Time.deltaTime));
        
        return coordinatesTable.IsWithinBounds(gridStat.x, gridStat.z - 1) && coordinatesTable.IsEmpty(gridStat.x, gridStat.z - 1) && (coordinatesTable.GetCheckoutTime(gridStat.x, gridStat.z - 1)+1f > Time.deltaTime);
    }

    // Check if coordinates on Left is within bounds
    private bool CheckGridLeft()
    {
        // Check IsWithinBounds, IsEmpty, and GetCheckoutTime 1sec passed
        return coordinatesTable.IsWithinBounds(gridStat.x - 1, gridStat.z) && coordinatesTable.IsEmpty(gridStat.x - 1, gridStat.z) && (coordinatesTable.GetCheckoutTime(gridStat.x - 1, gridStat.z)+1f > Time.deltaTime);
    }

    // Check if coordinates on Right is within bounds
    private bool CheckGridRight()
    {
        // Check IsWithinBounds, IsEmpty, and GetCheckoutTime 1sec passed
        return coordinatesTable.IsWithinBounds(gridStat.x + 1, gridStat.z) && coordinatesTable.IsEmpty(gridStat.x + 1, gridStat.z) && (coordinatesTable.GetCheckoutTime(gridStat.x + 1, gridStat.z)+1f > Time.deltaTime);

    }

    private void UpdateCoordinates(int x, int z)
    {
        // Set Current gridStat.x and gridStat.y IsEmpty in coordinatesTable
        coordinatesTable.SetGridUnitInfo(gridStat.x, gridStat.z, true, "", Time.deltaTime);

        // Set Destination Grid x, y in coordinatesTable
        coordinatesTable.SetGridUnitInfo(x, z, false, gameObject.name);

        // Update GridStat
        gridStat.x = x;
        gridStat.z = z;
    }

}
