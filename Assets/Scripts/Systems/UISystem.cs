using UnityEngine;
using UnityEngine.UIElements;

public class UISystem : MonoBehaviour
{
    public static UISystem Instance { get; private set; }
    [SerializeField] private UIDocument _uiDocument;
    private PauseMenu _pauseMenu;
    private bool _paused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        _pauseMenu = _uiDocument.GetComponent<PauseMenu>();
    }

    public void ShowHidePauseMenu()
    {
        if (!_paused)
        {
            _paused = true;
            _pauseMenu.ShowMenu();
        }
        else
        {
            _paused = false;
            _pauseMenu.HideMenu();
        }

    }
}
