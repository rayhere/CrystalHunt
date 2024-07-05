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

    private Label text_topCrystalCount;
    private Label text_totalCrystalsCollected;
    private const string TopCrystalCountKey = "TopCrystalCount";
    private const string TotalCrystalsCollectedKey = "TotalCrystalsCollected";

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

        _menuButtons = _document.rootVisualElement.Query<Button>().ToList();
        foreach (var button in _menuButtons)
        {
            button.RegisterCallback<ClickEvent>(OnAllButtonsClick);
        }

        text_topCrystalCount = _document.rootVisualElement.Q<Label>("TopCrystalCountLabel");
        text_totalCrystalsCollected = _document.rootVisualElement.Q<Label>("TotalCrystalsCollectedLabel");

        if (text_topCrystalCount == null)
        {
            Debug.LogError("TopCrystalCountLabel not found!");
        }
        else
        {
            Debug.Log("TopCrystalCountLabel found!");
        }

        if (text_totalCrystalsCollected == null)
        {
            Debug.LogError("TotalCrystalsColletedLabel not found!");
        }
        else
        {
            Debug.Log("TotalCrystalsColletedLabel found!");
        }

        if (PlayerPrefs.HasKey(TopCrystalCountKey))
        {
            int topCrystals = PlayerPrefs.GetInt(TopCrystalCountKey);
            if (text_topCrystalCount != null)
            {
                text_topCrystalCount.text = "Top Crystal Count:\n" + topCrystals.ToString() + " Crystals";
                Debug.Log("TopCrystalCountLabel is found topCrystals is " + topCrystals);
            }
            else
            {
                Debug.LogWarning("TopCrystalCountLabel not found in UI.");
            }
        }
        else
        {
            // Handle case where the key doesn't exist (optional)
            Debug.LogWarning("TopCrystalCountKey not found in PlayerPrefs.");
        }

        if (PlayerPrefs.HasKey(TotalCrystalsCollectedKey))
        {
            int totalCrystals = PlayerPrefs.GetInt(TotalCrystalsCollectedKey);
            if (text_totalCrystalsCollected != null)
            {
                text_totalCrystalsCollected.text = "Total Crystals Collected:\n" + totalCrystals.ToString() + " Crystals";
                Debug.Log("TotalCrystalsCollectedLabel is found topCrystals is " + totalCrystals);
            }
            else
            {
                Debug.LogWarning("TotalCrystalsCollectedLabel not found in UI.");
            }
        }
        else
        {
            // Handle case where the key doesn't exist (optional)
            Debug.LogWarning("TotalCrystalsCollectedKey not found in PlayerPrefs.");
        }
    }

    private void OnDisable()
    {
        _button.UnregisterCallback<ClickEvent>(OnStartClick);
        _button2.UnregisterCallback<ClickEvent>(OnLoadClick);
        
        foreach (var button in _menuButtons)
        {
            button.UnregisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    private void OnStartClick(ClickEvent evt)
    {
        Debug.Log("You press the Start Button");
        PersistentData.Instance.ResetPlayer();
        SceneManager.LoadScene("Level1");
    }

    private void OnLoadClick(ClickEvent evt)
    {
        Debug.Log("You press the Load Button");
        PersistentData.Instance.LoadData();
        SceneManager.LoadScene("Level1");
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
