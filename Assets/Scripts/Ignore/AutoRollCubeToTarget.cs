using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class AutoRollCubeToTarget : MonoBehaviour
{
    private Vector3 offset;

    public Transform[] Targets; // Target positions for cube movement

    public GameObject _cube;
    private GameObject center;
    private GameObject up;
    private GameObject down;
    private GameObject left;
    private GameObject right;

    private GameObject emptyGameObject; // Allow to control this object even _cube return to pool

    public float lengthOfCube = 1f;

    public int step = 9;
    public float speed = 0.01f;

    private bool input = false;

    private void Awake()
    {
        CreateEmptyObject();
    }

    void Start()
    {
        StartCoroutine(StartMovementTimer());
    }

    void Update()
    {
        if (input)
        {
            StartCoroutine(ChooseAndMove());
        }
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

        /* if (direction.x > 0.1 || direction.x < -0.1)


        if (dotUp > dotDown && dotUp > dotLeft && dotUp > dotRight)
        {
            yield return StartCoroutine(moveUP());
        }
        else if (dotDown > dotLeft && dotDown > dotRight)
        {
            yield return StartCoroutine(moveDown());
        }
        else if (dotLeft > dotRight)
        {
            yield return StartCoroutine(moveLeft());
        }
        else
        {
            yield return StartCoroutine(moveRight());
        } */


        // Wait for 5 seconds before choosing the next target
        yield return new WaitForSeconds(3f);


        // Enable input after moving
        input = true;
    }

    IEnumerator moveUP()
    {
        for (int i = 0; i < (90 / step); i++)
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

        // Set the position, rotation, and scale of EmptyGameObject
        // emptyGameObject.transform.position = new Vector3(0, lengthOfCube, 0);
        // emptyGameObject.transform.rotation = Quaternion.identity;
        // emptyGameObject.transform.localScale = new Vector3(1, 1, 1);
        
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
}
