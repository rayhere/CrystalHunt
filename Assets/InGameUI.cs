using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;


public class InGameUI : MonoBehaviour
{
    [Header("References")]
    public GameObject playerGameObject; // Reference to the Player GameObject
    public WASDController pm; // Reference to the WASDController component on the Player GameObject
    public ClickToMove ctm;
    public ThirdPersonCam tpc;

    //public VisualTreeAsset uiTree;

    public UIDocument _document;

    private Label text_speed;
    private Label text_mode;
    private Label text_cursorStatus;
    private Label text_crystalcollectStatus;

    private Button _pauseButton;
    private Button _resumeButton;
    private Button _quitButton;
    private Button _yesButton;
    private Button _noButton;

    //private TextField text_mode;

    private bool cursorLocked = true;
    private bool pauseMenuVisible = false; // Track visibility of PauseMenu
    private int pauseStage = 0;

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

            // "ResumeButton" is the Button name you created in UI Builder
            _resumeButton = _document.rootVisualElement.Q("ResumeButton") as Button;
            _resumeButton.RegisterCallback<ClickEvent>(OnResumeClick);

            // "QuitButton" is the Button name you created in UI Builder
            _quitButton = _document.rootVisualElement.Q("QuitButton") as Button;
            _quitButton.RegisterCallback<ClickEvent>(OnQuitClick);

            // "YesButton" is the Button name you created in UI Builder
            _yesButton = _document.rootVisualElement.Q("YesButton") as Button;
            _yesButton.RegisterCallback<ClickEvent>(OnYesClick);

            // "NoButton" is the Button name you created in UI Builder
            _noButton = _document.rootVisualElement.Q("NoButton") as Button;
            _noButton.RegisterCallback<ClickEvent>(OnNoClick);
        }

        if (_pauseButton == null)
        {
            Debug.LogError("PauseButton not found!");
        }
        else
        {
            Debug.Log("PauseButton found!");
        }

        if (_resumeButton == null)
        {
            Debug.LogError("ResumeButton not found!");
        }
        else
        {
            Debug.Log("ResumeButton found!");
        }

        if (_quitButton == null)
        {
            Debug.LogError("QuitButton not found!");
        }
        else
        {
            Debug.Log("QuitButton found!");
        }

        if (_yesButton == null)
        {
            Debug.LogError("YesButton not found!");
        }
        else
        {
            Debug.Log("YesButton found!");
        }

        if (_noButton == null)
        {
            Debug.LogError("NoButton not found!");
        }
        else
        {
            Debug.Log("NoButton found!");
        }

        // Initially hide the PauseMenu
        HideUIElement("PauseMenu");
        //HideUIElement("MidContainer");
        HideUIElement("MidContainer2");
    }

    void Start()
    {
        // Find the text box by name
        // text_speed = _document.rootVisualElement.Q<TextField>("BottomLeftLabel");

        // Get the references to the UI labels
        // pm = GetComponent<WASDController>();
        text_speed = _document.rootVisualElement.Q<Label>("BottomLeftLabel");
        text_mode = _document.rootVisualElement.Q<Label>("BottomRightLabel");
        text_cursorStatus = _document.rootVisualElement.Q<Label>("CursorStatusLabel");
        text_crystalcollectStatus = _document.rootVisualElement.Q<Label>("CrystalCollectLabel");

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

        // Lock the cursor initially
        LockCursor();

    }

    // Update is called once per frame
    void Update()
    {
        // Start the coroutine to log messages every 5 seconds
        // StartCoroutine(LogMessageRoutine(5f));

        CheckInput();
    }

    void FixedUpdate ()
    {
        UpdateUI();
        
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
            // Lock only for WASDMovement active
            LockCursor();
            if (pm != null) pm.pauseMenu = false;
            if (tpc != null) tpc.pauseMenu = false;
            if (ctm != null) ctm.pauseMenu = false;
        }
        else
        {
            ShowUIElement("PauseMenu");
            ShowUIElement("MidContainer1");
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
        ShowUIElement("PauseMenu");
        //HideUIElement("MidContainerEmpty");
        ShowUIElement("MidContainer1");
        Debug.Log(" cursorLocked : " + cursorLocked);
        pauseMenuVisible = true;
    }

    private void OnResumeClick(ClickEvent evt)
    {
        Debug.Log("You press the Resume Button");
        HideUIElement("PauseMenu");
        //HideUIElement("MidContainer1");
        //ShowUIElement("MidContainerEmpty");
        pauseMenuVisible = false;
    }

    private void OnQuitClick(ClickEvent evt)
    {
        Debug.Log("You press the Quit Button");
        HideUIElement("MidContainer1");
        ShowUIElement("MidContainer2");
        //ActivateSelector("MidContainer", "hide");
        pauseStage = 1;
    }

    private void OnYesClick(ClickEvent evt)
    {
        Debug.Log("You press the Yes Button");
        SceneManager.LoadScene("MainMenu");
        // HideUIElement("MidContainer");
        // ShowUIElement("MidContainer2");
        //ActivateSelector("MidContainer", "hide");
    }

    private void OnNoClick(ClickEvent evt)
    {
        Debug.Log("You press the No Button");
        HideUIElement("MidContainer2");
        ShowUIElement("MidContainer1");
        //ActivateSelector("MidContainer", "hide");
        pauseStage = 0;
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

            // Log a debug message
            //Debug.Log("Debug message every 5 seconds.");
        }
    }

    // Method to activate a specific USS selector
    public void ActivateSelector(string elementName, string selector)
    {
        VisualElement visualElement = _document.rootVisualElement.Q<VisualElement>(elementName);

        // if (visualElement != null)
        // {
        //     // Set the display style to block (show the element)
        //     visualElement.style.display = DisplayStyle.Flex;
        // }

        visualElement.AddToClassList(selector); // Add the CSS class corresponding to the selector
    }

    // Method to deactivate a specific USS selector
    public void DeactivateSelector(string elementName, string selector)
    {
        VisualElement visualElement = _document.rootVisualElement.Q<VisualElement>(elementName);

        // if (visualElement != null)
        // {
        //     // Set the display style to none (hide the element)
        //     visualElement.style.display = DisplayStyle.None;
        // }

        visualElement.RemoveFromClassList(selector); // Remove the CSS class corresponding to the selector
    }
    
    void CheckGameDataObject()
    {
        // Check if the PersistentData object does not exist in the scene
        if (GameObject.Find("GameData") == null)
        {
            // Load the MainMenu scene
            SceneManager.LoadScene("MainMenu");
        }
    }
}
