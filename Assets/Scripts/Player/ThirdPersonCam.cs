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
    public GameObject firstPersonCam;
    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject topDownCam;
    public GameObject thirdPersonOmniscientCam;
    private CinemachineVirtualCamera firstPersonVirtualCamera;
    private CinemachineFreeLook thirdPersonFreeLook;
    private CinemachineFreeLook combatFreeLook;
    private CinemachineFreeLook topDownFreeLook;
    private CinemachineFreeLook thirdPersonOmniscientFreeLook;
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
        FirstPerson,
        ThirdPerson,
        Combat,
        Topdown,
        ThirdPersonOmniscient
    }
    private Dictionary<CameraStyle, object> styleToCameraMap = new Dictionary<CameraStyle, object>();

    [Header("Cursor")]
    public bool cursorLock = false;

    [Header("PauseMenuEvent")]
    public bool pauseMenu = false;

    // Cinemachine FreeLook settings
    public CinemachineFreeLook freeLookCam;

    private void Awake()
    {
        //InitializeCursorSettings();
        InitializeCinemachineComponents();
        InitializeOrbitSettings();
    }

    private void Start()
    {
        currentStyle = CameraStyle.ThirdPerson;
        InitializeFreeLookCam();
        //if (freeLookCam == null) freeLookCam
        Debug.Log("CameraStyle currentStyle is " + currentStyle);
    }

    private void InitializeFreeLookCam()
    {
        if (!styleToCameraMap.ContainsKey(currentStyle))
        {
            Debug.LogError($"Camera style {currentStyle} not found in styleToCameraMap.");
            return;
        }

        object camObject = styleToCameraMap[currentStyle];
        if (camObject is CinemachineFreeLook freeLook)
        {
            freeLookCam = freeLook;
            ApplyOriginalOrbitSettings(freeLookCam);
        }
        else if (camObject is CinemachineVirtualCamera virtualCamera)
        {
            // Handle initialization specific to CinemachineVirtualCamera if needed
            Debug.Log($"Handling {currentStyle} as a CinemachineVirtualCamera.");
        }
        else
        {
            Debug.LogError($"Unexpected camera type found for style {currentStyle}: {camObject.GetType()}.");
        }
        
        //SwitchCameraStyle(CameraStyle.ThirdPerson);
        //ApplyOriginalOrbitSettings(freeLookCam);
    }

    private void InitializeOrbitSettings()
    {
        // Store original orbit settings for each camera ***IN ORDER***
        StoreOriginalOrbitSettings(thirdPersonFreeLook);
        StoreOriginalOrbitSettings(combatFreeLook);
        StoreOriginalOrbitSettings(topDownFreeLook);
        StoreOriginalOrbitSettings(thirdPersonOmniscientFreeLook);
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
                    freeLook.m_Orbits[i].m_Height = baseBottomRigHeight;
                    freeLook.m_Orbits[i].m_Radius = 0f; // Adjust radius if needed
                    break;
            }
        }
    }

    private void InitializeCinemachineComponents()
    {
        // Initialize CinemachineVirtualCamera component
        firstPersonVirtualCamera = InitializeCinemachineVirtualCamera(firstPersonCam);

        // Initialize  CinemachineFreeLook components
        thirdPersonFreeLook = InitializeCinemachineFreeLook(thirdPersonCam);
        combatFreeLook = InitializeCinemachineFreeLook(combatCam);
        topDownFreeLook = InitializeCinemachineFreeLook(topDownCam);
        thirdPersonOmniscientFreeLook = InitializeCinemachineFreeLook(thirdPersonOmniscientCam);

        // Populate the style to camera mapping (ensure each style is added only once)
        if (firstPersonVirtualCamera != null)
        styleToCameraMap.Add(CameraStyle.FirstPerson, firstPersonVirtualCamera);

        if (!styleToCameraMap.ContainsKey(CameraStyle.ThirdPerson) && thirdPersonFreeLook != null)
            styleToCameraMap.Add(CameraStyle.ThirdPerson, thirdPersonFreeLook);

        if (!styleToCameraMap.ContainsKey(CameraStyle.Combat) && combatFreeLook != null)
            styleToCameraMap.Add(CameraStyle.Combat, combatFreeLook);

        if (!styleToCameraMap.ContainsKey(CameraStyle.Topdown) && topDownFreeLook != null)
            styleToCameraMap.Add(CameraStyle.Topdown, topDownFreeLook);

        if (!styleToCameraMap.ContainsKey(CameraStyle.ThirdPersonOmniscient) && thirdPersonOmniscientFreeLook != null)
            styleToCameraMap.Add(CameraStyle.ThirdPersonOmniscient, thirdPersonOmniscientFreeLook);
    }

    private CinemachineVirtualCamera InitializeCinemachineVirtualCamera(GameObject camObject)
    {
        if (camObject != null)
        {
            CinemachineVirtualCamera virtualCamera = camObject.GetComponent<CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                return virtualCamera;
            }
            else
            {
                Debug.LogError($"CinemachineVirtualCamera component not found on {camObject.name} GameObject.");
            }
        }
        else
        {
            Debug.LogError($"GameObject {camObject.name} not found.");
        }
        return null;
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

        // Handle omniscient camera specific behavior
        //HandleOmniscientCamera();

        // Handle edge scrolling for camera movement based on current camera style
        if (currentStyle == CameraStyle.ThirdPersonOmniscient)
        {
            HandleOmniscientEdgeScrolling();
        }
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
            SwitchCameraStyle(CameraStyle.FirstPerson);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            SwitchCameraStyle(CameraStyle.ThirdPerson);
            ApplyOriginalOrbitSettings(freeLookCam);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) 
        {
            SwitchCameraStyle(CameraStyle.Combat);
            ApplyOriginalOrbitSettings(freeLookCam);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) 
        {
            SwitchCameraStyle(CameraStyle.Topdown);
            ApplyOriginalOrbitSettings(freeLookCam);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) // Handle switch to omniscient camera
        {
            SwitchCameraStyle(CameraStyle.ThirdPersonOmniscient);
            ApplyOriginalOrbitSettings(freeLookCam);
            StartCoroutine(HandleOmniscientCamera());
        }
    }

    private IEnumerator HandleOmniscientCamera()
    {
        // Check if omniscient camera style is active
        if (currentStyle == CameraStyle.ThirdPersonOmniscient)
        {
            // Handle edge scrolling for omniscient camera
            HandleOmniscientEdgeScrolling();

            // Lock axis control for omniscient camera
            DisableCinemachineInput(freeLookCam);
            Debug.Log("DisableCinemachineInput");

            // Make omniscient camera look at player
            if (freeLookCam != null && player != null)
            {
                freeLookCam.Follow = player;
                freeLookCam.LookAt = player;
            }

            // Wait for one frame
            yield return null;

            if (freeLookCam != null && player != null)
            {
                freeLookCam.Follow = null;
                freeLookCam.LookAt = null;
            }
        }
        else
        {
            // If not omniscient style, enable axis control
            EnableCinemachineInput(freeLookCam);
        }
    }

    private void HandleOmniscientEdgeScrolling()
    {
        // Get mouse position in screen coordinates
        Vector3 mousePosition = Input.mousePosition;

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

            // Apply movement to the camera's position
            thirdPersonOmniscientFreeLook.transform.position += moveDirection;
        }
    }


    private void HandleOmniscientEdgeScrollingOld()
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
        /* if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
            moveDirection *= edgeScrollSpeedThisFrame;

            // Apply movement relative to camera orientation
            moveDirection = Camera.main.transform.TransformDirection(moveDirection);

            // Adjust camera position for omniscient camera only
            if (freeLookCam == thirdPersonOmniscientFreeLook)
            {
                transform.position += moveDirection;
            }
        } */

        // Normalize move direction to prevent faster diagonal movement
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
            moveDirection *= edgeScrollSpeedThisFrame;

            // Apply movement relative to camera orientation
            //transform.position += moveDirection;
            thirdPersonOmniscientFreeLook.transform.position += moveDirection;
        }
    }

    private void HandleOrbitScaling()
    {   
        if (currentStyle == CameraStyle.FirstPerson) return;

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
        //HandleOrbitScaling(); // bug
        
        RotateOrientation();

    }

    private void RotateOrientation()
    {

        if (currentStyle == CameraStyle.FirstPerson)
        {
            // Calculate direction from firstPersonCam position to playerModel position
            Vector3 viewDirection = playerModel.position - firstPersonCam.transform.position;

            // Preserve current z and x rotation of playerModel
            Quaternion currentRotation = playerModel.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(viewDirection);

            // Preserve z and x rotation
            targetRotation.eulerAngles = new Vector3(currentRotation.eulerAngles.x, targetRotation.eulerAngles.y, currentRotation.eulerAngles.z);

            // Apply rotation with preservation
            playerModel.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);


            return;
        }
        else
        {
            // For other camera styles (ThirdPerson, Combat, Topdown), handle rotation differently
            // Depending on your game logic, you may want to handle these cases separately.
            // Example: Follow target, look at target, etc.
        }
        
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
        if (currentStyle == CameraStyle.ThirdPerson || currentStyle == CameraStyle.Topdown)
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
        firstPersonCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        combatCam.SetActive(false);
        topDownCam.SetActive(false);
        thirdPersonOmniscientCam.SetActive(false);

        // Activate the chosen camera style
        if (styleToCameraMap.TryGetValue(newStyle, out object camObject))
        {
            if (camObject is CinemachineFreeLook freeLook)
            {
                freeLook.gameObject.SetActive(true);
                freeLookCam = freeLook;
            }
            else if (camObject is CinemachineVirtualCamera virtualCam)
            {
                virtualCam.gameObject.SetActive(true);
                // Handle Virtual Camera specific settings or assignments
            }
        }
        else
        {
            Debug.LogError($"No camera component found for style {newStyle}");
            return;
        }
        /* if (newStyle == CameraStyle.FirstPerson)
        {
            firstPersonCam.SetActive(true);
        }
        else if (newStyle == CameraStyle.ThirdPerson)
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
        else if (newStyle == CameraStyle.ThirdPersonOmniscient)
        {
            thirdPersonOmniscientCam.SetActive(true);
            freeLookCam = thirdPersonOmniscientFreeLook;
        } */

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

    public void ApplyOriginalOrbitSettingsOld(CinemachineFreeLook freeLook)
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

    private void ApplyOriginalOrbitSettings(CinemachineFreeLook freeLook)
    {
        if (originalOrbitSettings.TryGetValue(freeLook, out CinemachineFreeLook.Orbit[] orbits))
        {
            for (int i = 0; i < freeLook.m_Orbits.Length && i < orbits.Length; i++)
            {
                freeLook.m_Orbits[i].m_Height = orbits[i].m_Height;
                freeLook.m_Orbits[i].m_Radius = orbits[i].m_Radius;
            }
        }
    }

    private void InitializeCursorSettings()
    {
        // Initialize cursor lock state
        Cursor.lockState = cursorLock ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !cursorLock;
    }
}
