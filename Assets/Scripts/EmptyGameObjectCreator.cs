using UnityEngine;

public class EmptyGameObjectCreator : MonoBehaviour
{
    public float lengthOfCube = 1f;
    void Start()
    {
        lengthOfCube = Mathf.Abs(lengthOfCube / 2);
        // Create an empty GameObject
        GameObject emptyGameObject = new GameObject("EmptyGameObject");

        // Create empty GameObjects for Up, Down, Left, and Right
        GameObject upObject = new GameObject("Up");
        GameObject downObject = new GameObject("Down");
        GameObject leftObject = new GameObject("Left");
        GameObject rightObject = new GameObject("Right");

        // Set the parent of Up, Down, Left, and Right to EmptyGameObject
        upObject.transform.parent = emptyGameObject.transform;
        downObject.transform.parent = emptyGameObject.transform;
        leftObject.transform.parent = emptyGameObject.transform;
        rightObject.transform.parent = emptyGameObject.transform;

        // Set the position, rotation, and scale of EmptyGameObject
        emptyGameObject.transform.position = new Vector3(0, lengthOfCube, 0);
        emptyGameObject.transform.rotation = Quaternion.identity;
        emptyGameObject.transform.localScale = new Vector3(1, 1, 1);

        // Set the positions of Up, Down, Left, and Right relative to EmptyGameObject
        upObject.transform.localPosition = new Vector3(0, -lengthOfCube, lengthOfCube);
        downObject.transform.localPosition = new Vector3(0, -lengthOfCube, -lengthOfCube);
        leftObject.transform.localPosition = new Vector3(-lengthOfCube, -lengthOfCube, 0);
        rightObject.transform.localPosition = new Vector3(lengthOfCube, -lengthOfCube, 0);
    }
}
