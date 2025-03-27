using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private UIDocument _uiDocument;
    private Button _button;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        //_button = _uiDocument.rootVisualElement.Q("StartButton");
        //_button.RegisterCallback<ClickEvent>(OnPlayGameClick);

    }
    private void OnPlayGameClick()
    {
       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
