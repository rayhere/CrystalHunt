using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private UIDocument _document;

    private Button _button;

    private List<Button> _menuButtons = new List<Button>();

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _document = GetComponent<UIDocument>();
        // "StartButton" is the Button name you created in UI Builder
        _button = _document.rootVisualElement.Q("StartButton") as Button;
        _button.RegisterCallback<ClickEvent>(OnStartClick);

        _menuButtons = _document.rootVisualElement.Query<Button>().ToList();
        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].RegisterCallback<ClickEvent>(OnAllButonsClick);
        }
    }

    private void OnDisable()
    {
        _button.UnregisterCallback<ClickEvent>(OnStartClick);
        
        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].UnregisterCallback<ClickEvent>(OnAllButonsClick);
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

    private void OnAllButonsClick(ClickEvent evt)
    {
        Debug.Log("Play Button Sound");
        _audioSource.Play();
    }
}
