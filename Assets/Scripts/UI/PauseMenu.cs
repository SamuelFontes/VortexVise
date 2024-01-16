using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;
    private VisualElement _root;

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

        b = CreateButton("Options");
        b.clicked += UISystem.Instance.ShowHidePauseMenu;
        div.Add(b);

        b = CreateButton("Change Level");
        b.clicked += RenderLevelLoader;
        div.Add(b);

        b = CreateButton("Quit");
        b.clicked += QuitGame;

        div.Add(b);


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
    void QuitGame() 
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    void RenderLevelLoader()
    {

        _root = _document.rootVisualElement;

        _root.Clear();

        _root.styleSheets.Add(_styleSheet);

        var titleLabel = new Label("Pause");
        titleLabel.AddToClassList("title");
        
        _root.Add(titleLabel);

        var div = new VisualElement();
        div.AddToClassList("div");


        var maps = MapLoaderSystem.Instance.GetMapList(Gamemode.DeathMatch);
        foreach(var map in maps)
        {
            var b = CreateButton(map.name);
            b.clicked += () => MapLoaderSystem.Instance.LoadMap(map);
            div.Add(b);
        }

        var back = CreateButton("Go Back");
        back.clicked += Generate;
        div.Add(back);

        _root.Add(div);
    }
}
