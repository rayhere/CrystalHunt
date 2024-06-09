using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]

    private UIDocument _document;

    private Button _button;
    
    private List<Button> _pausemenuButtons = new List<Button>();

    private AudioSource _audioSource;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        if (_document == null)
        {
            Debug.LogError("UIDocument component not found!");
        }
        else
        {
            Debug.Log("UIDocument component found!");
        }

        _audioSource = GetComponent<AudioSource>();

        // "ResumeButton" is the Button name you created in UI Builder
        _button = _document.rootVisualElement.Q("ResumeButton") as Button;
        _button.RegisterCallback<ClickEvent>(OnResumeClick);

        // "QuitButton" is the Button name you created in UI Builder
        _button = _document.rootVisualElement.Q("QuitButton") as Button;
        _button.RegisterCallback<ClickEvent>(OnQuitClick);



    }

    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate ()
    {
        UpdateUI();
    }

    void UpdateUI()
    {


    }

    private void OnDisable()
    {
        _button.UnregisterCallback<ClickEvent>(OnResumeClick);
        
        for (int i = 0; i < _pausemenuButtons.Count; i++)
        {
            _pausemenuButtons[i].UnregisterCallback<ClickEvent>(OnAllButonsClick);
        }
    }

    private void OnResumeClick(ClickEvent evt)
    {
        Debug.Log("You press the Resume Button");
    }

    private void OnQuitClick(ClickEvent evt)
    {
        Debug.Log("You press the Give Up Button");
    }

    private void OnAllButonsClick(ClickEvent evt)
    {
        Debug.Log("Play Button Sound");
        _audioSource.Play();
    }
}
