using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMoveV1 : MonoBehaviour
{
    public int speed = 300;
    bool isMoving = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving){
            return;
        }

        if (Input.GetKey(KeyCode.UpArrow)){
            StartCoroutine(Roll(Vector3.forward));
        }
        if (Input.GetKey(KeyCode.DownArrow)){
            StartCoroutine(Roll(Vector3.back));
        }
        if (Input.GetKey(KeyCode.LeftArrow)){
            StartCoroutine(Roll(Vector3.left));
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            StartCoroutine(Roll(Vector3.right));
        }

    }
    IEnumerator Roll (Vector3 direction)
    {
        isMoving = true;
        float remainingAngle = 90;
        Vector3 rotationCenter = transform.position + direction / 2 + Vector3.down / 2;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        while (remainingAngle > 0)
        {
            float rotationAngle = Mathf.Min(Time.deltaTime * speed, remainingAngle);
            transform.RotateAround(rotationCenter, rotationAxis, rotationAngle);
            remainingAngle -= rotationAngle;
            yield return null;
        }

        isMoving = false;
    }

}

