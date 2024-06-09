using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class InGameUI : MonoBehaviour
{
    [Header("References")]
    public GameObject playerGameObject; // Reference to the Player GameObject
    public WASDController pm; // Reference to the WASDController component on the Player GameObject

    //public VisualTreeAsset uiTree;

    private UIDocument _document;

    private Label text_speed;
    private Label text_mode;

    //private TextField text_mode;

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
    }

    void Start()
    {
        // Find the text box by name
        // text_speed = _document.rootVisualElement.Q<TextField>("BottomLeftLabel");

        // Get the references to the UI labels
        // pm = GetComponent<WASDController>();
        text_speed = _document.rootVisualElement.Q<Label>("BottomLeftLabel");
        text_mode = _document.rootVisualElement.Q<Label>("BottomRightLabel");

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


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate ()
    {
        updateUI();
    }

    void updateUI()
    {
        // text_speed.value = ToString(pm.getTextMode());
        //string textMode = pm.GetTextMode();

        //text_speed.text = pm.GetTextSpeed();
        //text_speed.text = ("LMAO");
        //text_mode.text = textMode;

        //text_speed.text = "New Speed Value";
        //text_mode.text = "New Mode Value";

        if (pm != null)
        {
            text_speed.text = pm.GetTextSpeed();
            text_mode.text = pm.GetTextMode();
        }

    }



}
