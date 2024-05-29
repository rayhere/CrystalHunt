using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraDirection : MonoBehaviour
{
    private float rotationX = 0;
    private float rotationY = 0;
    public float sensitivity = 25f;
    private float screenEdge = 20f;
    void LateUpdate()
    {

        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

        if (Input.mousePosition.x > Screen.width - screenEdge)
        {
            rotationY += sensitivity;
        }
        else if (Input.mousePosition.x < screenEdge)
        {
            rotationY -= sensitivity;
        }
        if (Input.mousePosition.y > Screen.height - screenEdge)
        {
            rotationX -= sensitivity;
        }
        else if (Input.mousePosition.y < screenEdge)
        {
            rotationX += sensitivity;
        }

    }
}