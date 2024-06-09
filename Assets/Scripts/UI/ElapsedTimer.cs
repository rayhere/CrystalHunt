using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElapsedTimer : MonoBehaviour
{
    [Header("References")]
    private UIDocument _document; 

    [SerializeField]
    float elapsedTime;

    private Label text_elapsedTimer;

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
    }

    private void Start()
    {
        elapsedTime = PersistentData.Instance.GetElapsedTime();

        text_elapsedTimer = _document.rootVisualElement.Q<Label>("TimerLabel");
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        UpdateElapsedTimer();
    }

    void UpdateElapsedTimer()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        text_elapsedTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
