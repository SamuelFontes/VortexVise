using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;
    private VisualElement _root;

    //private void Start()
    //{
        //ShowMenu();
    //}
//
    //private void OnValidate()
    //{
        //if (Application.isPlaying) return;
        //ShowMenu();
    //}

    void Generate()
    {

        _root = _document.rootVisualElement;

        _root.Clear();

        _root.styleSheets.Add(_styleSheet);

        var titleLabel = new Label("Pause");
        titleLabel.AddToClassList("title");
        
        _root.Add(titleLabel);

        var div = new VisualElement();
        div.AddToClassList("div");


        var b = CreateButton("Continue");
        b.clicked += UISystem.Instance.ShowHidePauseMenu;

        div.Add(b);
        div.Add(CreateButton("Options"));
        div.Add(CreateButton("Change Level"));
        div.Add(CreateButton("Quit"));


        _root.Add(div);
    }

    Button CreateButton(string text)
    {
        var button = new Button();
        button.AddToClassList("btn");
        button.text = text;
        return button;
    }
    public void HideMenu()
    {
        _root = _document.rootVisualElement;
        _root.Clear();
    }
    public void ShowMenu()
    {
        _root = _document.rootVisualElement;
        Generate();
    }
}
