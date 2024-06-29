using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;
using System.Runtime.InteropServices;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    //[SerializeField, Tooltip("PlayerObj, GameObject")]
    //public GameObject playerObj;
    [SerializeField, Tooltip("Player, GameObject hold movement script")]
    public Transform player;
    [SerializeField, Tooltip("PlayerModel, hold Model")]
    public Transform playerModel;
    [SerializeField, Tooltip("Agent, GameObject hold Agent Component")]
    public NavMeshAgent agent;
    public LineRenderer lineRenderer;
    public Rigidbody rb;

    [Header("Cinemachine Cameras")]
    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject topDownCam;
    private CinemachineFreeLook thirdPersonFreeLook;
    private CinemachineFreeLook combatFreeLook;
    private CinemachineFreeLook topDownFreeLook;
    private Dictionary<CinemachineFreeLook, CinemachineFreeLook.Orbit[]> originalOrbitSettings = new Dictionary<CinemachineFreeLook, CinemachineFreeLook.Orbit[]>();

    [Header("Camera Movement")]
    public float edgeScrollSpeed = 10f;
    public float edgeScrollBoundary = 25f;

    public float rotationSpeed;

    public Transform combatLookAt;

    [Header("Orbit Settings")]
    public float baseTopRigHeight; // Base height of the TopRig orbit
    public float baseMiddleRigHeight; // Base height of the MiddleRig orbit
    public float baseBottomRigHeight; // Base height of the BottomRig orbit

    [Header("Scale Settings")]
    public float maxScaleFactor = 5f; // Maximum scale factor for heights
    public float minScaleFactor = 1f; // Minimum scale factor for heights
    public float scaleSpeed = 0.5f; // Speed at which to scale heights

    public CameraStyle currentStyle;
    public enum CameraStyle
    {
        Basic,
        Combat,
        Topdown
    }
    private Dictionary<CameraStyle, CinemachineFreeLook> styleToCameraMap = new Dictionary<CameraStyle, CinemachineFreeLook>();

    [Header("Cursor")]
    public bool cursorLock = false;

    [Header("PauseMenuEvent")]
    public bool pauseMenu = false;

    // Cinemachine FreeLook settings
    public CinemachineFreeLook freeLookCam;

    private void Awake()
    {
        //InitializeCursorSettings();
        InitializeCinemachineFreeLook();
        InitializeOrbitSettings();
    }

    private void Start()
    {
        InitializeFreeLookCam();
        //if (freeLookCam == null) freeLookCam
    }

    private void InitializeFreeLookCam()
    {
        // Initialize Cinemachine FreeLook component
        if (freeLookCam == null)
        {
            freeLookCam = styleToCameraMap[currentStyle];
            if (freeLookCam == null)
            {
                Debug.LogError("CinemachineFreeLook component not found for current camera style.");
                return;
            }
            ApplyOriginalOrbitSettings(freeLookCam);
        }
    }

    private void InitializeOrbitSettings()
    {
        // Store original orbit settings for each camera
        StoreOriginalOrbitSettings(thirdPersonFreeLook);
        StoreOriginalOrbitSettings(combatFreeLook);
        StoreOriginalOrbitSettings(topDownFreeLook);
    }

    private void InitializeOrbitSettings(CinemachineFreeLook freeLook)
    {
        if (freeLook == null)
        {
            Debug.LogWarning("CinemachineFreeLook is null.");
            return;
        }

        // Initialize orbit settings
        for (int i = 0; i < freeLook.m_Orbits.Length; i++)
        {
            switch (i)
            {
                case 0: // TopRig
                    freeLook.m_Orbits[i].m_Height = baseTopRigHeight;
                    freeLook.m_Orbits[i].m_Radius = 0f; // Adjust radius if needed
                    break;
                case 1: // MiddleRig
                    freeLook.m_Orbits[i].m_Height = baseMiddleRigHeight;
                    freeLook.m_Orbits[i].m_Radius = 0f; // Adjust radius if needed
                    break;
                case 2: // BottomRig
                    // Adjust as needed
                    break;
            }
        }
    }

    private void InitializeCinemachineFreeLook()
    {
        // Cache CinemachineFreeLook components
        thirdPersonFreeLook = InitializeCinemachineFreeLook(thirdPersonCam);
        combatFreeLook = InitializeCinemachineFreeLook(combatCam);
        topDownFreeLook = InitializeCinemachineFreeLook(topDownCam);

        // Populate the style to camera mapping (ensure each style is added only once)
        if (!styleToCameraMap.ContainsKey(CameraStyle.Basic) && thirdPersonFreeLook != null)
            styleToCameraMap.Add(CameraStyle.Basic, thirdPersonFreeLook);

        if (!styleToCameraMap.ContainsKey(CameraStyle.Combat) && combatFreeLook != null)
            styleToCameraMap.Add(CameraStyle.Combat, combatFreeLook);

        if (!styleToCameraMap.ContainsKey(CameraStyle.Topdown) && topDownFreeLook != null)
            styleToCameraMap.Add(CameraStyle.Topdown, topDownFreeLook);
    }

    private CinemachineFreeLook InitializeCinemachineFreeLook(GameObject camObject)
    {
        if (camObject != null)
        {
            CinemachineFreeLook freeLook = camObject.GetComponent<CinemachineFreeLook>();
            if (freeLook != null)
            {
                return freeLook;
            }
            else
            {
                Debug.LogError($"CinemachineFreeLook component not found on {camObject.name} GameObject.");
            }
        }
        else
        {
            Debug.LogError($"GameObject {camObject.name} not found.");
        }
        return null;
    }

    private void Update()
    {
        // Check if pauseMenu is active
        if (pauseMenu)
        {
            // Disable Cinemachine FreeLook input
            DisableCinemachineInput(freeLookCam);
            return;
        }
        else
        {
            // Re-enable Cinemachine FreeLook input
            EnableCinemachineInput(freeLookCam);
        }

        HandleSwitchCameraStyles();

        // Handle orbit scaling based on input scroll
        //HandleOrbitScaling();

        // Rotate camera orientation based on player movement or target
        //RotateOrientation();

        // Handle edge scrolling for camera movement
        //HandleEdgeScrolling();
    }

    private void DisableCinemachineInput(CinemachineFreeLook freeLook)
    {
        if (freeLook != null)
        {
            freeLook.m_XAxis.m_InputAxisName = ""; // Clear input axis name to disable horizontal movement
            freeLook.m_YAxis.m_InputAxisName = ""; // Clear input axis name to disable vertical movement
        }
    }

    private void EnableCinemachineInput(CinemachineFreeLook freeLook)
    {
        if (freeLook != null)
        {
            freeLook.m_XAxis.m_InputAxisName = "Mouse X"; // Reset input axis name to enable horizontal movement
            freeLook.m_YAxis.m_InputAxisName = "Mouse Y"; // Reset input axis name to enable vertical movement
        }
    }

    private void HandleSwitchCameraStyles()
    {
        // Switch camera styles based on input
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SwitchCameraStyle(CameraStyle.Basic);
            ApplyOriginalOrbitSettings(freeLookCam);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            SwitchCameraStyle(CameraStyle.Combat);
            ApplyOriginalOrbitSettings(freeLookCam);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) 
        {
            SwitchCameraStyle(CameraStyle.Topdown);
            ApplyOriginalOrbitSettings(freeLookCam);
        }
    }
    private void HandleOrbitScaling()
    {   
        // Handle orbit scaling based on input scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            // Calculate new scale factor
            float scaleFactor = Mathf.Clamp(freeLookCam.m_Orbits[0].m_Height / baseTopRigHeight, minScaleFactor, maxScaleFactor);
            //float scaleFactor = Mathf.Clamp(6 / 6, 1, 5);
            Debug.Log("freeLookCam.m_Orbits[0].m_Height is " + freeLookCam.m_Orbits[0].m_Height + " baseTopRigHeight is " + baseTopRigHeight);
            Debug.Log("scaleFactor is " + scaleFactor);

            // Update scale factor based on scroll input
            scaleFactor += scroll * scaleSpeed;
            Debug.Log("scroll is " + scroll + " scaleSpeed is " + scaleSpeed);
            Debug.Log("scaleFactor now is " + scaleFactor);
            Debug.Log("minScaleFactor is " + minScaleFactor + " maxScaleFactor is " + maxScaleFactor);

            scaleFactor = Mathf.Clamp(scaleFactor, minScaleFactor, maxScaleFactor);
            Debug.Log("Clamped scaleFactor is " + scaleFactor);
            // Update the heights of TopRig and MiddleRig orbits
            UpdateOrbitScale(scaleFactor);
        }
    }

    private void FixedUpdate()
    {   
        if (pauseMenu) return;
        //HandleSwitchCameraStyles();
        HandleOrbitScaling();
        
        RotateOrientation();

    }

    private void RotateOrientation()
    {
        if (agent!=null) 
        {
            if (agent.enabled)
            {
                // Rotate orientation
                Vector3 viewDire = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
                orientation.forward = viewDire.normalized;

                if (lineRenderer != null && lineRenderer.positionCount > 0)
                {
                    // Get the position of the last point in the line renderer (assumes straight line)
                    //Vector3 lineEndpoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
                    Vector3 lineEndpoint = lineRenderer.GetPosition(1);
                    Debug.Log("lineEndpoint is " +lineEndpoint);

                    // Calculate direction vector from player to line endpoint
                    Vector3 dirToLineEndpoint = lineEndpoint - player.position;

                    Debug.Log("dirToLineEndpoint is " +dirToLineEndpoint);

                    // Project dirToLineEndpoint onto the XZ plane (ignore Y axis)
                    Vector3 forwardDirection = new Vector3(dirToLineEndpoint.x, 0f, dirToLineEndpoint.z).normalized;
                    //Vector3 rightDirection = new Vector3(dirToLineEndpoint.z, 0f, -dirToLineEndpoint.x).normalized;

                    // Create a rotation towards the line endpoint using forward and right directions
                    Quaternion targetRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
                    Debug.Log("targetRotation is " +targetRotation);

                    // Smoothly rotate playerModel towards the target rotation
                    playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRotation, Time.deltaTime * rotationSpeed);

                    /* // Adjust playerModel forward direction to point towards the line endpoint
                    if (dirToLineEndpoint != Vector3.zero)
                        playerModel.forward = Vector3.Slerp(playerModel.forward, dirToLineEndpoint.normalized, Time.deltaTime * rotation Speed);*/
                }

                return;
            }
            else
            {
                // agent is disable
            }
        }
        // Rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // Rotate player object based on current camera style
        if (currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.Topdown)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
                playerModel.forward = Vector3.Slerp(playerModel.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
        else if (currentStyle == CameraStyle.Combat)
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerModel.forward = dirToCombatLookAt.normalized;
        }
    }

    private void HandleEdgeScrolling()
    {
        // Get mouse position in screen coordinates
        Vector3 mousePosition = Input.mousePosition;

        // Convert mouse position to world coordinates
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));

        // Calculate edge scroll based on screen edges
        float edgeScrollSpeedThisFrame = edgeScrollSpeed * Time.deltaTime;
        Vector3 moveDirection = Vector3.zero;

        if (mousePosition.x < edgeScrollBoundary)
        {
            moveDirection += Vector3.left;
        }
        else if (mousePosition.x > Screen.width - edgeScrollBoundary)
        {
            moveDirection += Vector3.right;
        }

        if (mousePosition.y < edgeScrollBoundary)
        {
            moveDirection += Vector3.back;
        }
        else if (mousePosition.y > Screen.height - edgeScrollBoundary)
        {
            moveDirection += Vector3.forward;
        }

        // Normalize move direction to prevent faster diagonal movement
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
            moveDirection *= edgeScrollSpeedThisFrame;

            // Apply movement relative to camera orientation
            moveDirection = Camera.main.transform.TransformDirection(moveDirection);

            // Adjust camera position
            transform.position += moveDirection;
        }
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        // Deactivate all cameras first
        thirdPersonCam.SetActive(false);
        combatCam.SetActive(false);
        topDownCam.SetActive(false);

        // Activate the chosen camera style
        if (newStyle == CameraStyle.Basic)
        {
            thirdPersonCam.SetActive(true);
            freeLookCam = thirdPersonFreeLook;
        }
        else if (newStyle == CameraStyle.Combat)
        {
            combatCam.SetActive(true);
            freeLookCam = combatFreeLook;
        }
        else if (newStyle == CameraStyle.Topdown)
        {
            topDownCam.SetActive(true);
            freeLookCam = topDownFreeLook;
        }

        // Update current style
        currentStyle = newStyle;
    }

    private void UpdateOrbitScale(float scaleFactor)
    {
        Debug.Log("UpdateOrbitScale(float scaleFactor) is " + scaleFactor);
        // Calculate new heights based on scale factor
        float newTopRigHeight = baseTopRigHeight * scaleFactor;
        float newMiddleRigHeight = baseMiddleRigHeight * scaleFactor;

        // Update the Cinemachine FreeLook rig heights
        if (freeLookCam != null)
        {
            freeLookCam.m_Orbits[0].m_Height = newTopRigHeight; // Adjust TopRig height
            freeLookCam.m_Orbits[1].m_Height = newMiddleRigHeight; // Adjust MiddleRig height
        }

        // Log for debugging
        Debug.Log("Updated scale factor to: " + scaleFactor);
        Debug.Log("TopRig Height: " + (freeLookCam != null ? freeLookCam.m_Orbits[0].m_Height : 0f));
        Debug.Log("MiddleRig Height: " + (freeLookCam != null ? freeLookCam.m_Orbits[1].m_Height : 0f));
    }

    private void StoreOriginalOrbitSettings(CinemachineFreeLook freeLook)
    {
        if (freeLook != null)
        {
            CinemachineFreeLook.Orbit[] orbits = new CinemachineFreeLook.Orbit[freeLook.m_Orbits.Length];
            for (int i = 0; i < freeLook.m_Orbits.Length; i++)
            {
                orbits[i] = new CinemachineFreeLook.Orbit
                {
                    m_Height = freeLook.m_Orbits[i].m_Height,
                    m_Radius = freeLook.m_Orbits[i].m_Radius
                };
            }
            originalOrbitSettings.Add(freeLook, orbits);
        }
    }

    public void ApplyOriginalOrbitSettings(CinemachineFreeLook freeLook)
    {
        if (originalOrbitSettings.ContainsKey(freeLook))
        {
            CinemachineFreeLook.Orbit[] orbits = originalOrbitSettings[freeLook];
            for (int i = 0; i < orbits.Length; i++)
            {
                freeLook.m_Orbits[i].m_Height = orbits[i].m_Height;
                freeLook.m_Orbits[i].m_Radius = orbits[i].m_Radius;
            }

            // Update base heights based on restored orbits
            baseTopRigHeight = freeLook.m_Orbits[0].m_Height;
            baseMiddleRigHeight = freeLook.m_Orbits[1].m_Height;
            baseBottomRigHeight = freeLook.m_Orbits[2].m_Height;
        }
        else
        {
            Debug.LogError("Orbit settings for this CinemachineFreeLook are not stored.");
        }
    }

    private void InitializeCursorSettings()
    {
        // Initialize cursor lock state
        Cursor.lockState = cursorLock ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !cursorLock;
    }
}
