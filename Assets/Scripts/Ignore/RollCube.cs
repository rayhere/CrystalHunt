using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollCube : MonoBehaviour
{
    private Vector3 offset;

    public Transform[] Targets; // Target positions for cube movement

    public GameObject _cube;
    public GameObject center;
    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;
    public float lengthOfCube = 1f;

    public int step = 9;
    public float speed = 0.01f;

    private bool input = true;

    private void Awake()
    {
        CreateEmptyObject();
    }

    void Update()
    {
        if (input)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                StartCoroutine(moveUP());
                input = false;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                StartCoroutine(moveDown());
                input = false;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                StartCoroutine(moveLeft());
                input = false;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                StartCoroutine(moveRight());
                input = false;
            }
        }
    }

    IEnumerator moveUP()
    {
        for (int i = 0; i < (90 / step); i++)
        {
            _cube.transform.RotateAround(up.transform.position, Vector3.right, step);
            yield return new WaitForSeconds(speed);
        }
        center.transform.position = _cube.transform.position;
        input = true;
    }

    IEnumerator moveDown()
    {
        for (int i = 0; i < (90 / step); i++)
        {
            _cube.transform.RotateAround(down.transform.position, Vector3.left, step);
            yield return new WaitForSeconds(speed);
        }
        center.transform.position = _cube.transform.position;
        input = true;
    }

    IEnumerator moveLeft()
    {
        for (int i = 0; i < (90 / step); i++)
        {
            _cube.transform.RotateAround(left.transform.position, Vector3.forward, step);
            yield return new WaitForSeconds(speed);
        }
        center.transform.position = _cube.transform.position;
        input = true;
    }

    IEnumerator moveRight()
    {
        for (int i = 0; i < (90 / step); i++)
        {
            _cube.transform.RotateAround(right.transform.position, Vector3.back, step);
            yield return new WaitForSeconds(speed);
        }
        center.transform.position = _cube.transform.position;
        input = true;
    }

    public void CreateEmptyObject()
    {
        lengthOfCube = Mathf.Abs(lengthOfCube / 2);
        // Create an empty GameObject
        GameObject emptyGameObject = new GameObject(gameObject.name + " Center");

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

    public void ChooseTarget(Vector3 targetPosition)
    {
        StartCoroutine(MoveToTarget(targetPosition));
    }

    IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        // Calculate the direction to the target
        Vector3 direction = (targetPosition - _cube.transform.position).normalized;
        
        // Move the cube towards the target
        while (Vector3.Distance(_cube.transform.position, targetPosition) > 0.01f)
        {
            _cube.transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }
    }
}
