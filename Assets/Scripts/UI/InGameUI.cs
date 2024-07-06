using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;
using System;


public class InGameUI : MonoBehaviour
{
    [Header("References")]
    public GameObject playerGameObject; // Reference to the Player GameObject
    public WASDController pm; // Reference to the WASDController component on the Player GameObject
    public ClickToMove ctm;
    public ThirdPersonCam tpc;
    public MovementSwitcher ms;
    public DarknessStatsSO playerStats; // Reference to the ScriptableObject
    public bool playerAlive = true;
    public SupporterController[] sc;

    public UIDocument _document;

    private Label text_speed;
    private Label text_mode;
    private Label text_cursorStatus;
    private Label text_crystalcollectStatus;
    private Label text_controlMode;
    private Label text_cameraType;
    private Label text_supporterMode;

    private Button _pauseButton;
    private Button _pmResumeButton;
    private Button _pmQuitButton;
    private Button _pmYesButton;
    private Button _pmNoButton;
    private Button _goTryAgainButton;
    private Button _goQuitButton;
    private Button _goYesButton;
    private Button _goNoButton;

    private ProgressBar healthProgressBar;
    private ProgressBar manaProgressBar;
    private ProgressBar staminaProgressBar;

    private bool cursorLocked = false;
    private bool pauseMenuVisible = false; // Track visibility of PauseMenu
    public int pauseStage = 0;
    public int gameOverStage = -1;

    private bool isPaused = false;
    private float previousTimeScale;

    private const string CrystalCountKey = "CrystalCount";

    private void Awake()
    {
        CheckGameDataObject();
        _document = GetComponent<UIDocument>();
        if (_document == null)
        {
            Debug.LogError("UIDocument component not found!");
        }
        else
        {
            Debug.Log("UIDocument component found!");
        }

        // Get the WASDController component from the Player GameObject
        pm = playerGameObject.GetComponent<WASDController>();
        
        if (pm == null)
        {
            Debug.LogError("WASDController component not found on the Player GameObject!");
        }
        else
        {
            Debug.Log("WASDController component found!");
        }

        InitializeUIElements();
    }

    private void InitializeUIElements()
    {
        if (_document != null) 
        {
            // "PauseButton" is the Button name you created in UI Builder
            _pauseButton = _document.rootVisualElement.Q("PauseButton") as Button;
            _pauseButton.RegisterCallback<ClickEvent>(OnPauseClick);

            // "pmResumeButton" is the Button name you created in UI Builder
            _pmResumeButton = _document.rootVisualElement.Q("pmResumeButton") as Button;
            _pmResumeButton.RegisterCallback<ClickEvent>(OnPMResumeClick);

            // "pmQuitButton" is the Button name you created in UI Builder
            _pmQuitButton = _document.rootVisualElement.Q("pmQuitButton") as Button;
            _pmQuitButton.RegisterCallback<ClickEvent>(OnPMQuitClick);

            // "pmYesButton" is the Button name you created in UI Builder
            _pmYesButton = _document.rootVisualElement.Q("pmYesButton") as Button;
            _pmYesButton.RegisterCallback<ClickEvent>(OnPMYesClick);

            // "pmNoButton" is the Button name you created in UI Builder
            _pmNoButton = _document.rootVisualElement.Q("pmNoButton") as Button;
            _pmNoButton.RegisterCallback<ClickEvent>(OnPMNoClick);

            // "TryAgainButton" is the Button name you created in UI Builder
            _goTryAgainButton = _document.rootVisualElement.Q("goTryAgainButton") as Button;
            _goTryAgainButton.RegisterCallback<ClickEvent>(OnGOTryAgainClick);

            // "goQuitButton" is the Button name you created in UI Builder
            _goQuitButton = _document.rootVisualElement.Q("goQuitButton") as Button;
            _goQuitButton.RegisterCallback<ClickEvent>(OnGOQuitClick);

            // "goYesButton" is the Button name you created in UI Builder
            _goYesButton = _document.rootVisualElement.Q("goYesButton") as Button;
            _goYesButton.RegisterCallback<ClickEvent>(OnGOYesClick);

            // "goNoButton" is the Button name you created in UI Builder
            _goNoButton = _document.rootVisualElement.Q("goNoButton") as Button;
            _goNoButton.RegisterCallback<ClickEvent>(OnGONoClick);

            healthProgressBar = _document.rootVisualElement.Q<ProgressBar>("healthProgressBar");
            manaProgressBar = _document.rootVisualElement.Q<ProgressBar>("manaProgressBar");
            staminaProgressBar = _document.rootVisualElement.Q<ProgressBar>("staminaProgressBar");
        }

        if (_pauseButton == null)
        {
            Debug.LogError("PauseButton not found!");
        }
        else
        {
            Debug.Log("PauseButton found!");
        }

        if (_pmResumeButton == null)
        {
            Debug.LogError("pmResumeButton not found!");
        }
        else
        {
            Debug.Log("pmResumeButton found!");
        }

        if (_pmQuitButton == null)
        {
            Debug.LogError("pmQuitButton not found!");
        }
        else
        {
            Debug.Log("pmQuitButton found!");
        }

        if (_pmYesButton == null)
        {
            Debug.LogError("pmYesButton not found!");
        }
        else
        {
            Debug.Log("pmYesButton found!");
        }

        if (_pmNoButton == null)
        {
            Debug.LogError("pmNoButton not found!");
        }
        else
        {
            Debug.Log("pmNoButton found!");
        }

        if (_goTryAgainButton == null)
        {
            Debug.LogError("goTryAgainButton not found!");
        }
        else
        {
            Debug.Log("goTryAgainButton found!");
        }

        if (_goQuitButton == null)
        {
            Debug.LogError("goQuitButton not found!");
        }
        else
        {
            Debug.Log("goQuitButton found!");
        }

        if (_goYesButton == null)
        {
            Debug.LogError("goYesButton not found!");
        }
        else
        {
            Debug.Log("goYesButton found!");
        }

        if (_goNoButton == null)
        {
            Debug.LogError("goNoButton not found!");
        }
        else
        {
            Debug.Log("goNoButton found!");
        }

        if (healthProgressBar == null)
        {
            Debug.LogError("healthProgressBar not found!");
        }
        else
        {
            Debug.Log("healthProgressBar found!");
        }

        if (manaProgressBar == null)
        {
            Debug.LogError("manaProgressBar not found!");
        }
        else
        {
            Debug.Log("manaProgressBar found!");
        }

        if (staminaProgressBar == null)
        {
            Debug.LogError("staminaProgressBar not found!");
        }
        else
        {
            Debug.Log("staminaProgressBar found!");
        }
        
        // Initially hide the PauseMenu
        HideUIElement("PauseMenu");
        HideUIElement("pmMidContainer1");
        // Initially hide the GameOverMenu
        HideUIElement("GameOverMenu");
        HideUIElement("goMidContainer1");
    }

    void Start()
    {
        // Find the text box by name
        // Get the references to the UI labels
        text_speed = _document.rootVisualElement.Q<Label>("BottomLeftLabel");
        text_mode = _document.rootVisualElement.Q<Label>("BottomRightLabel");
        text_cursorStatus = _document.rootVisualElement.Q<Label>("CursorStatusLabel");
        text_crystalcollectStatus = _document.rootVisualElement.Q<Label>("CrystalCollectLabel");
        text_controlMode = _document.rootVisualElement.Q<Label>("ControlModeLabel");
        text_cameraType = _document.rootVisualElement.Q<Label>("CameraTypeLabel");
        text_supporterMode = _document.rootVisualElement.Q<Label>("SupporterModeLabel");

        if (text_speed == null)
        {
            Debug.LogError("BottomLeftLabel not found!");
        }
        else
        {
            Debug.Log("BottomLeftLabel found!");
        }

        if (text_mode == null)
        {
            Debug.LogError("BottomRightLabel not found!");
        }
        else
        {
            Debug.Log("BottomRightLabel found!");
        }

        if (text_cursorStatus == null)
        {
            Debug.LogError("CursorStatusLabel not found!");
        }
        else
        {
            Debug.Log("CursorStatusLabel found!");
        }

        if (text_crystalcollectStatus == null)
        {
            Debug.LogError("CrystalCollectLabel not found!");
        }
        else
        {
            Debug.Log("CrystalCollectLabel found!");
        }

        if (text_controlMode == null)
        {
            Debug.LogError("ControlModeLabel not found!");
        }
        else
        {
            Debug.Log("ControlModeLabel found!");
        }

        if (text_cameraType == null)
        {
            Debug.LogError("CameraTypeLabel not found!");
        }
        else
        {
            Debug.Log("CameraTypeLabel found!");
        }

        if (text_supporterMode == null)
        {
            Debug.LogError("SupporterModeLabel not found!");
        }
        else
        {
            Debug.Log("SupporterModeLabel found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAlive) CheckInput();
    }

    void FixedUpdate ()
    {
        UpdateUI();
        if (!playerAlive && gameOverStage == -1)
        {
            gameOverStage = 0;
            PersistentData.Instance.SetGameOverStage(true);
            HideUIElement("PauseMenu");
            HideUIElement("pmMidContainer1");
            ShowUIElement("GameOverMenu");
            ShowUIElement("goMidContainer");
            UnlockCursor();
        }
    }

    void UpdateUI()
    {
        UpdatePlayerMovementStatus();
        UpdateCursorStatus();
        UpdateCrystalCollectStatus();
        UpdateControlMode();
        UpdateCameraType();
        UpdateHealthProgressBar();
        UpdateManaProgressBar();
        UpdateStaminaProgressBar();
        UpdateSupporterStatus();
    }

    void UpdateHealthProgressBar()
    {
        // Update the ProgressBar value based on player HP
        float currentPlayerHP = playerStats.currentHP;
        float maxHP = playerStats.maxHP;

        // Calculate the value to set on the progress bar
        float normalizedHP = currentPlayerHP / maxHP;

        // Set the value of the slider (progress bar)
        healthProgressBar.value = playerStats.currentHP;
        healthProgressBar.highValue = playerStats.maxHP;
    }

    void UpdateManaProgressBar()
    {
        float currentPlayerMP = playerStats.currentMP;
        float maxMP = playerStats.maxMP;

        // Calculate the value to set on the progress bar
        float normalizedMP = currentPlayerMP / maxMP;

        // Set the value of the slider (progress bar)
        manaProgressBar.value = playerStats.currentMP;
        manaProgressBar.highValue = playerStats.maxMP;
    }

    void UpdateStaminaProgressBar()
    {
        float currentPlayerSP = playerStats.currentSP;
        float maxSP = playerStats.maxSP;

        // Calculate the value to set on the progress bar
        float normalizedSP = currentPlayerSP / maxSP;

        // Set the value of the slider (progress bar)
        staminaProgressBar.value = playerStats.currentSP;
        staminaProgressBar.highValue = playerStats.maxSP;
    }

    void UpdateControlMode()
    {
        if (pm != null)
        {
            text_controlMode.text = ms.GetTextMode();
        }
    }

    void UpdateCameraType()
    {
        if (pm != null)
        {
            text_cameraType.text = tpc.GetTextMode();
        }
    }

    private void UpdatePlayerMovementStatus()
    {
        if (pm != null)
        {
            text_speed.text = pm.GetTextSpeed();
            text_mode.text = pm.GetTextMode();
        }
    }

    private void UpdateSupporterStatus()
    {
        // UpdateBehavior
        // Initialize an empty string to accumulate text
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string textMode = "";
        // Iterate through each SupporterController in the array
        foreach (SupporterController supporter in sc)
        {
            if (supporter != null)
            {
                if (supporter.isAware == true)
                {
                    if (supporter.isAvoidTarget == true)
                    {
                        textMode = "Avoid Target";
                    }
                    else
                    {
                        textMode = "Chase Target";
                    }
                }
                else if (supporter.isSearchingItem == true)
                {
                    textMode = "Searching Item";
                }
                else
                {
                    textMode = "Patrol";
                }
                // Append the game object name and text speed to the StringBuilder with a new line
                sb.AppendLine(supporter.gameObject.name + ": " + textMode);
            }
        }

        // Set the text of text_supporterMode to the accumulated string
        text_supporterMode.text = sb.ToString();
        //text_supporterMode.text = textMode;
    }

    private void UpdateCursorStatus()
    {
        text_cursorStatus.text = ("cursorLockState: " + UnityEngine.Cursor.lockState);
    }

    private void UpdateCrystalCollectStatus()
    {
        int currentCount = PlayerPrefs.GetInt(CrystalCountKey, 0);
        text_crystalcollectStatus.text = ("Crystal: " + PersistentData.Instance.GetCrystalCount());
    }

    private void CheckInput()
    {
        // Check for the Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed");
            // Toggle PauseMenu visibility
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        if (pauseMenuVisible)
        {
            if (pauseStage !=0) return;
            HideUIElement("PauseMenu");
            ShowUIElement("BodyContainer");
            TogglePause();
            // Lock only for WASDMovement active
            if (pm.isActive) LockCursor();
            if (pm != null) pm.pauseMenu = false;
            if (tpc != null) tpc.pauseMenu = false;
            if (ctm != null) ctm.pauseMenu = false;
        }
        else
        {
            // Make Sure Resume ClickEvent do the same
            HideUIElement("BodyContainer");
            ShowUIElement("PauseMenu");
            ShowUIElement("pmMidContainer");
            TogglePause();
            UnlockCursor();
            if (pm != null) pm.pauseMenu = true;
            if (tpc != null) tpc.pauseMenu = true;
            if (ctm != null) ctm.pauseMenu = true;
        }
        pauseMenuVisible = !pauseMenuVisible;
    }
    
    private void LockCursor()
    {
        Debug.Log("LockCursor method");
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        cursorLocked = true;
    }

    private void UnlockCursor()
    {
        Debug.Log("UnlockCursor method");
        cursorLocked = false;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }
    
    private void OnPauseClick(ClickEvent evt)
    {
        Debug.Log("You press the Pause Button");
        TogglePauseMenu();
    }

    private void OnPMResumeClick(ClickEvent evt)
    {
        Debug.Log("You press the pmResume Button");
        TogglePauseMenu();
    }

    private void OnPMQuitClick(ClickEvent evt)
    {
        Debug.Log("You press the pmQuit Button");
        HideUIElement("pmMidContainer");
        ShowUIElement("pmMidContainer1");
        pauseStage = 1;
    }

    private void OnPMYesClick(ClickEvent evt)
    {
        Debug.Log("You press the pmYes Button");
        PersistentData.Instance.SaveData();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnPMNoClick(ClickEvent evt)
    {
        Debug.Log("You press the pmNo Button");
        HideUIElement("pmMidContainer1");
        ShowUIElement("pmMidContainer");
        pauseStage = 0;
    }

    private void OnGOTryAgainClick(ClickEvent evt)
    {
        Debug.Log("You press the goTryAgain Button");
        gameOverStage = 0;
        PersistentData.Instance.ResetPlayer();
        // Reload Scene
        // Get the current active scene's name
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Reload the current scene
        SceneManager.LoadScene(currentSceneName);
    }

    private void OnGOQuitClick(ClickEvent evt)
    {
        Debug.Log("You press the goQuit Button");
        HideUIElement("goMidContainer");
        ShowUIElement("goMidContainer1");
        //ActivateSelector("MidContainer", "hide");
        gameOverStage = 1;
    }

    private void OnGOYesClick(ClickEvent evt)
    {
        Debug.Log("You press the goYes Button");
        PersistentData.Instance.SaveData();
        PersistentData.Instance.SetGameOverStage(false); // reset
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");

    }

    private void OnGONoClick(ClickEvent evt)
    {
        Debug.Log("You press the goNo Button");
        HideUIElement("goMidContainer1");
        ShowUIElement("goMidContainer");
        gameOverStage = 0;
    }


    void ShowUIElement(string elementName)
    {
        Debug.Log("ShowUIElement : " + elementName);
        // Find the element you want to show
        VisualElement elementToShow = _document.rootVisualElement.Q<VisualElement>(elementName);

        if (elementToShow != null)
        {
            // Set the display style to block (show the element)
            elementToShow.style.display = DisplayStyle.Flex;
        }
    }

    void HideUIElement(string elementName)
    {
        Debug.Log("HideUIElement : " + elementName);
        // Find the element you want to hide
        VisualElement elementToHide = _document.rootVisualElement.Q<VisualElement>(elementName);

        if (elementToHide != null)
        {
            // Set the display style to none (hide the element)
            elementToHide.style.display = DisplayStyle.None;
        }
    }

    private IEnumerator LogMessageRoutine(float waitTime)
    {
        while (true)
        {
            // Wait for 5 seconds
            yield return new WaitForSeconds(waitTime);
        }
    }
    
    void CheckGameDataObject()
    {
        // Check if the PersistentData object does not exist in the scene
        if (GameObject.Find("GameData") == null)
        {
            UnlockCursor();
            // Load the MainMenu scene
            SceneManager.LoadScene("MainMenu");
        }
    }

    void TogglePause()
    {
        if (isPaused)
        {
            Time.timeScale = previousTimeScale; // Resume previous time scale
            //Time.timeScale = 1f; // Set time scale to 1 (normal speed)
            isPaused = false;
        }
        else
        {
            previousTimeScale = Time.timeScale; // Store current time scale
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
}
