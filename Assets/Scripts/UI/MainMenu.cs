using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private UIDocument _document;

    private Button _button, _button2;

    private List<Button> _menuButtons = new List<Button>();

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _document = GetComponent<UIDocument>();
        // "StartButton" is the Button name you created in UI Builder

        // Find and register callbacks for each button in the UI document
        _button = _document.rootVisualElement.Q("StartButton") as Button;
         if (_button != null)
        {
            _button.RegisterCallback<ClickEvent>(OnStartClick);
        }

        _button2 = _document.rootVisualElement.Q("LoadButton") as Button;
        if (_button2 != null)
        {
            _button2.RegisterCallback<ClickEvent>(OnLoadClick);
        }

/*         _menuButtons = _document.rootVisualElement.Query<Button>().ToList();
        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].RegisterCallback<ClickEvent>(OnButtonClick);
        } */
        // Get all buttons from the UI document and register callbacks for each
        _menuButtons = _document.rootVisualElement.Query<Button>().ToList();
        foreach (var button in _menuButtons)
        {
            button.RegisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    private void OnDisable()
    {
        _button.UnregisterCallback<ClickEvent>(OnStartClick);
        _button2.UnregisterCallback<ClickEvent>(OnLoadClick);
        
/*         for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].UnregisterCallback<ClickEvent>(OnButtonClick);
        } */
        // Unregister callbacks for all buttons when the script is disabled
        foreach (var button in _menuButtons)
        {
            button.UnregisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    private void OnStartClick(ClickEvent evt)
    {
        Debug.Log("You press the Start Button");
        SceneManager.LoadScene("Level1");
    }

    private void OnLoadClick(ClickEvent evt)
    {
        Debug.Log("You press the Load Button");
        SceneManager.LoadScene("Level2");
    }

    private void OnOptionsClick(ClickEvent evt)
    {
        Debug.Log("You press the Options Button");
    }

    private void OnCreditClick(ClickEvent evt)
    {
        Debug.Log("You press the Credit Button");
    }

    private void OnQuitClick(ClickEvent evt)
    {
        Debug.Log("You press the Quit Button");
    }

    private void OnAllButtonsClick(ClickEvent evt)
    {
        Debug.Log("Play Button Sound");
        _audioSource.Play();
    }
}
