using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UIDocuments")]
    public UIDocument inGameUIDocument;
    public UIDocument pauseMenuUIDocument;
    public UIDocument gameOverUIDocument;

    [Header("Player References")]
    public GameObject playerGameObject; // Reference to the Player GameObject
    private WASDController pm; // Reference to the WASDController component on the Player GameObject

    [Header("UI Elements")]
    // Labels
    public Label text_speed;
    public Label text_mode;
    public Label text_cursorStatus;
    public Label text_crystalcollectStatus;

    // Buttons
    private Button _pauseButton;
    private Button _resumeButton;
    private Button _quitButton;
    private Button _yesButton;
    private Button _noButton;

    [Header("Gameplay Variables")]
    private bool cursorLocked = true;
    private const string CrystalCountKey = "CrystalCount";

    private VisualElement currentUI;

    void Start()
    {
        InitializeUIElements(inGameUIDocument);
        // Initialize with in-game UI as default
        ShowInGameUI();

        
    }

    public void ShowInGameUI()
    {
        ActivateUI(inGameUIDocument);
    }

    public void ShowPauseMenuUI()
    {
        ActivateUI(pauseMenuUIDocument);
    }

    public void ShowGameOverUI()
    {
        ActivateUI(gameOverUIDocument);
    }

    private void ActivateUI(UIDocument uiDocument)
    {
        // Deactivate current UI if any
        DeactivateCurrentUI();

        // Activate new UI
        currentUI = uiDocument?.rootVisualElement;
        if (currentUI != null)
        {
            currentUI.style.display = DisplayStyle.Flex; 
        }

        // Activate new UI
        currentUI = uiDocument?.rootVisualElement;
        if (currentUI != null)
        {
            currentUI.style.display = DisplayStyle.Flex;
            
        }
    }

    private void DeactivateCurrentUI()
    {
        if (currentUI != null)
        {
            currentUI.style.display = DisplayStyle.None;
        }
    }

    private void InitializeUIElements(UIDocument uiDocument)
    {
        // Get the labels from the UIDocument
        text_speed = uiDocument.rootVisualElement.Q<Label>("BottomLeftLabel");
        text_mode = uiDocument.rootVisualElement.Q<Label>("BottomRightLabel");
        text_cursorStatus = uiDocument.rootVisualElement.Q<Label>("CursorStatusLabel");
        text_crystalcollectStatus = uiDocument.rootVisualElement.Q<Label>("CrystalCollectLabel");

        // Example: Initialize buttons and their callbacks
        _pauseButton = uiDocument.rootVisualElement.Q<Button>("PauseButton");
        if (_pauseButton != null)
        {
            _pauseButton.RegisterCallback<ClickEvent>(evt => OnPauseButtonClick());
        }

        _resumeButton = uiDocument.rootVisualElement.Q<Button>("ResumeButton");
        if (_resumeButton != null)
        {
            _resumeButton.RegisterCallback<ClickEvent>(evt => OnResumeButtonClick());
        }

        _quitButton = uiDocument.rootVisualElement.Q<Button>("QuitButton");
        if (_quitButton != null)
        {
            _quitButton.RegisterCallback<ClickEvent>(evt => OnQuitButtonClick());
        }

        _yesButton = uiDocument.rootVisualElement.Q<Button>("YesButton");
        if (_yesButton != null)
        {
            _yesButton.RegisterCallback<ClickEvent>(evt => OnYesButtonClick());
        }

        _noButton = uiDocument.rootVisualElement.Q<Button>("NoButton");
        if (_noButton != null)
        {
            _noButton.RegisterCallback<ClickEvent>(evt => OnNoButtonClick());
        }

        // Get the WASDController component from the Player GameObject
        pm = playerGameObject.GetComponent<WASDController>();
        if (pm == null)
        {
            Debug.LogError("WASDController component not found on the Player GameObject!");
        }

        // Lock the cursor initially
        LockCursor();
    }

    void Update()
    {
        //UpdateUI();
        //CheckInput();
    }

    void UpdateUI()
    {
        UpdatePlayerMovementStatus();
        UpdateCursorStatus();
        UpdateCrystalCollectStatus();
    }

    private void UpdatePlayerMovementStatus()
    {
        if (pm != null)
        {
            text_speed.text = pm.GetTextSpeed();
            text_mode.text = pm.GetTextMode();
        }
    }

    private void UpdateCursorStatus()
    {
        text_cursorStatus.text = ("cursorLockState: " + UnityEngine.Cursor.lockState);
    }

    private void UpdateCrystalCollectStatus()
    {
        int currentCount = PlayerPrefs.GetInt(CrystalCountKey, 0);
        text_crystalcollectStatus.text = ("Crystal: " + currentCount);
    }

    private void CheckInput()
    {
        // Check for the Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed");
            // Toggle cursor lock state
            if (cursorLocked)
            {
                UnlockCursor();
            }
            else
            {
                LockCursor();
            }
        }
    }

    private void LockCursor()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        cursorLocked = true;
    }

    private void UnlockCursor()
    {
        cursorLocked = false;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    private void OnPauseButtonClick()
    {
        Debug.Log("Pause Button Clicked!");
        // Implement pause logic here
    }

    private void OnResumeButtonClick()
    {
        Debug.Log("Resume Button Clicked!");
        // Implement resume logic here
    }

    private void OnQuitButtonClick()
    {
        Debug.Log("Quit Button Clicked!");
        // Implement quit logic here
    }

    private void OnYesButtonClick()
    {
        Debug.Log("Yes Button Clicked!");
        // Implement yes logic here
    }

    private void OnNoButtonClick()
    {
        Debug.Log("No Button Clicked!");
        // Implement no logic here
    }
}
