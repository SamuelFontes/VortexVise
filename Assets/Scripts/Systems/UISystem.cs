using UnityEngine;
using UnityEngine.InputSystem;
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
        PauseGame();

    }

    void PauseGame()
    {
        // TODO: change this to the UI scheme
        // Disable controls
        foreach(var player in GameState.Instance.LocalPlayers)
        {
            //player.GetComponent<PlayerInput>().enabled = !_paused;
            if(_paused)
                player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
            else
                player.GetComponent<PlayerInput>().SwitchCurrentActionMap("ActorCombat");
        }
        //GetComponent<PlayerInputManager>().enabled = !_paused;

        // Pause game
        if (_paused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }
}
