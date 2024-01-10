using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private PlayerCamera _playerCameraPrefab;
    private MapLoaderSystem _mapLoader;
    private WeaponSystem _weaponSystem;

    // Start is called before the first frame update
    void Start()
    {
        _mapLoader = GetComponent<MapLoaderSystem>();
        _weaponSystem = GetComponent<WeaponSystem>();   
    }

    // Update is called once per frame
    void Update()
    {
        Utils.UpdateGamepadRumble();
    }
    void OnPlayerJoined(PlayerInput playerInput)
    {
        // TODO: Improve this when main menu exist
        var playerObject = playerInput.gameObject;
        var player = playerObject.GetComponent<Player>();
        var crosshair = playerObject.transform.GetChild(1);

        var camera = Instantiate(_playerCameraPrefab);
        camera.SetTarget(crosshair.transform);

        player.SetPlayerCamera(camera);

        AddLocalPlayer(player);
        _weaponSystem.GetWeaponByName(player.GetComponent<CombatBehaviour>(), "Pistol"); //TODO: remove
    }

    private void AddLocalPlayer(Player player)
    {

        GameState.Instance.LocalPlayers.Add(player);

        SetupCameras();
        if (GameState.Instance.Gamemode == Gamemode.MainMenu)
            StartDeathMatch(); // FIXME: The gamemode shouldn't be changed when adding player, it should be set on the menu

        SetupPlayerTeam(player);
        SetupMousePlayer(player, true);
    }

    private void SetupMousePlayer(Player player, bool isConnecting)
    {
        if(player.gameObject.GetComponent<PlayerInput>().currentControlScheme == "MouseAndKeyboard")
        {
           Cursor.visible = !isConnecting; 
        }
    }
    public void RemoveLocalPlayer(Player player)
    {
        var p = GameState.Instance.LocalPlayers.Where(x => x.Id == player.Id).FirstOrDefault();
        GameState.Instance.LocalPlayers.Remove(p);
        SetupCameras();
        GameState.Instance.RemovePlayerFromTeam(player);
        SetupMousePlayer(player, true);
    }

    private void SetupCameras()
    {
        var cameraDistance = 15;
        // Check number of players
        switch (GameState.Instance.GetNumberOfLocalPlayers())
        {
            case 0:
                throw new Exception("Can't setup camera if there are no players");
            case 1:
                GameState.Instance.LocalPlayers[0].Camera.SetupCamera(new Rect(0f, 0f, 1f, 1f), cameraDistance, true);
                break;
            case 2:
                cameraDistance = 20;
                GameState.Instance.LocalPlayers[0].Camera.SetupCamera(new Rect(0f, 0f, 0.5f, 1f), cameraDistance, true);
                GameState.Instance.LocalPlayers[1].Camera.SetupCamera(new Rect(0.5f, 0f, 0.5f, 1f), cameraDistance, false);
                break; 
            case 3: 
                cameraDistance = 20;
                GameState.Instance.LocalPlayers[0].Camera.SetupCamera(new Rect(0f, 0f, 0.5f, 1f), cameraDistance, true); // This man has half of the screen, it's good to be player one
                GameState.Instance.LocalPlayers[1].Camera.SetupCamera(new Rect(0.5f, 0f, 0.5f, 0.5f), cameraDistance, false);
                GameState.Instance.LocalPlayers[2].Camera.SetupCamera(new Rect(0.5f, 0.5f, 0.5f, 0.5f), cameraDistance, false);
                break;
            case 4:
                cameraDistance = 20;
                GameState.Instance.LocalPlayers[0].Camera.SetupCamera(new Rect(0f, 0f, 0.5f, 0.5f), cameraDistance, true); // This man has half of the screen, it's good to be player one
                GameState.Instance.LocalPlayers[1].Camera.SetupCamera(new Rect(0.5f, 0f, 0.5f, 0.5f), cameraDistance, false);
                GameState.Instance.LocalPlayers[2].Camera.SetupCamera(new Rect(0f, 0.5f, 0.5f, 0.5f), cameraDistance, false);
                GameState.Instance.LocalPlayers[3].Camera.SetupCamera(new Rect(0.5f, 0.5f, 0.5f, 0.5f), cameraDistance, false);
                break;
            default: 
                break;
        }

    }
    
    private void StartDeathMatch()
    {
        // FIXME: This just deletes everything from the menu, make this better
        var menuThings = GameObject.FindGameObjectsWithTag("MainMenu");
        foreach (var thing in menuThings)
        {
            UnityEngine.Object.Destroy(thing);
        }
        GameState.Instance.SetGamemode(Gamemode.DeathMatch);
        _mapLoader.LoadRandomMap(Gamemode.DeathMatch);
    }

    private void SetupPlayerTeam(Player player)
    {
        switch (GameState.Instance.Gamemode)
        {
            case Gamemode.MainMenu:
                break;
            case Gamemode.DeathMatch:
                GameState.Instance.AutoBalancePlayer(player);
                break;
            case Gamemode.Mission:
                GameState.Instance.SetPlayerTeam(player, TeamLayer.TeamOne);
                break;
        }
    }

}
