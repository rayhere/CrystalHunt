using UnityEngine;

public class TopDownCameraV3 : MonoBehaviour
{
    public Transform target; // The target to follow (e.g., the player character)
    public Vector3 offset = new Vector3(0f, 10f, -10f); // The offset from the target's position
    public float moveSpeed = 5f; // The speed at which the camera moves towards the target
    public float rotationSpeed = 5f; // The speed at which the camera rotates to follow the target's orientation
    public float borderSize = 25f; // The size of the border zone (in pixels) to trigger camera movement
    [SerializeField] private bool updatePositionToTarget = true; // Enable/disable updating camera position to transform target

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            if (updatePositionToTarget)
            {
                // Calculate the desired position of the camera relative to the target
                Vector3 desiredPosition = target.position + offset;

                // Move camera towards mouse position if it's near the border
                Vector3 moveDirection = Vector3.zero;
                Vector3 mousePosition = Input.mousePosition;

                if (mousePosition.x <= borderSize)
                {
                    moveDirection += Vector3.left;
                }
                else if (mousePosition.x >= Screen.width - borderSize)
                {
                    moveDirection += Vector3.right;
                }

                if (mousePosition.y <= borderSize)
                {
                    moveDirection += Vector3.back;
                }
                else if (mousePosition.y >= Screen.height - borderSize)
                {
                    moveDirection += Vector3.forward;
                }

                // Convert move direction from screen space to world space
                moveDirection = mainCamera.transform.TransformDirection(moveDirection.normalized);
                moveDirection.y = 0f;

                desiredPosition += moveDirection * moveSpeed * Time.deltaTime;

                transform.position = desiredPosition;
            }

            transform.LookAt(target.position);
        }
    }

    public bool UpdatePositionToTarget
    {
        get { return updatePositionToTarget; }
        set { updatePositionToTarget = value; }
    }
}
