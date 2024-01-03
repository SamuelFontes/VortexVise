using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public GameObject playerCamera;
    public GameObject mapLoaderObject;
    private MapLoader mapLoader;
    // Start is called before the first frame update
    void Start()
    {
        // Setup game start menu
        SetupGameData();
        mapLoader = mapLoaderObject.GetComponent<MapLoader>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnPlayerJoined(PlayerInput playerInput)
    {
        // TODO: Improve this when menu exist
        var playerObject = playerInput.gameObject;
        var player = playerObject.GetComponent<Player>();
        var crosshair = playerObject.transform.GetChild(1);

        var camera = Instantiate(playerCamera);
        camera.GetComponent<PlayerCamera>().target = crosshair.transform;

        player.camera = camera;

        AddLocalPlayer(player);
    }

    //TODO: Create add menu player, after exits menu add localplayer
    private void AddLocalPlayer(Player player)
    {
        GameState.LocalPlayers.Add(player);

        SetupCameras();
        if (GameState.Gamemode == Gamemode.MainMenu)
            StartDeathMatch(); // FIXME: The gamemode shouldn't be changed when adding player, it should be set on the menu

        SetupPlayerTeam(player);
    }
    public void RemoveLocalPlayer(Player player)
    {
        var p = GameState.LocalPlayers.Where(x => x.Id == player.Id).FirstOrDefault();
        GameState.LocalPlayers.Remove(p);
        SetupCameras();
        RemovePlayerFromTeam(player);
    }

    private void SetupCameras()
    {
        // TODO: Maybe add a single camera mode on multiplayer when players are close
        // Reorder players, mouse and keyboard should aways be player one. Otherwise mouse aiming will go all funky
        // FIXME: This is not working, fix to work on any position, the mouse is all weird and shit
        for(int i = 0; i < GameState.LocalPlayers.Count; i++)
        {
            var player = GameState.LocalPlayers[i];
            if(player.gameObject.GetComponent<PlayerInput>().currentControlScheme == "MouseAndKeyboard")
            {
                if (i == 0)
                    break;

                GameState.LocalPlayers.RemoveAt(i);
                GameState.LocalPlayers.Insert(0,player);
                break;
            }
        }

        var cameraDistance = 15;
        // Check number of players
        switch (GameState.GetNumberOfLocalPlayers())
        {
            //TODO: improve this crap
            case 0:
                throw new Exception("Can't setup camera if there are no players");
            case 1:
                // Setup audio listener, it should have only one
                GameState.LocalPlayers[0].camera.GetComponent<AudioListener>().enabled = true;
                GameState.LocalPlayers[0].camera.GetComponent<Camera>().rect = new Rect(0f, 0f, 1f, 1f); 
                GameState.LocalPlayers[0].camera.GetComponent<Camera>().orthographicSize = cameraDistance;
                break;
            case 2:
                cameraDistance = 20;
                GameState.LocalPlayers[0].camera.GetComponent<Camera>().rect = new Rect(0f, 0f, 0.5f, 1f); 
                GameState.LocalPlayers[0].camera.GetComponent<Camera>().orthographicSize = cameraDistance;
                GameState.LocalPlayers[0].camera.GetComponent<AudioListener>().enabled = true;
                GameState.LocalPlayers[1].camera.GetComponent<Camera>().rect = new Rect(0.5f, 0f, 0.5f, 1f); 
                GameState.LocalPlayers[1].camera.GetComponent<Camera>().orthographicSize = cameraDistance;
                GameState.LocalPlayers[1].camera.GetComponent<AudioListener>().enabled = false;
                break; 
            case 3: 
                //TODO: Test this shit please
                cameraDistance = 20;
                GameState.LocalPlayers[0].camera.GetComponent<Camera>().rect = new Rect(0f, 0f, 0.5f, 1f); // This man has half of the screen, it's good to be player one
                GameState.LocalPlayers[0].camera.GetComponent<Camera>().orthographicSize = cameraDistance;
                GameState.LocalPlayers[0].camera.GetComponent<AudioListener>().enabled = true;
                GameState.LocalPlayers[1].camera.GetComponent<Camera>().rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                GameState.LocalPlayers[1].camera.GetComponent<Camera>().orthographicSize = cameraDistance;
                GameState.LocalPlayers[1].camera.GetComponent<AudioListener>().enabled = false;
                GameState.LocalPlayers[2].camera.GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                GameState.LocalPlayers[2].camera.GetComponent<Camera>().orthographicSize = cameraDistance;
                GameState.LocalPlayers[2].camera.GetComponent<AudioListener>().enabled = false;
                break;
            case 4:
                //TODO: Test this shit please
                cameraDistance = 20;
                GameState.LocalPlayers[0].camera.GetComponent<Camera>().rect = new Rect(0f, 0f, 0.5f, 0.5f); 
                GameState.LocalPlayers[0].camera.GetComponent<Camera>().orthographicSize = cameraDistance;
                GameState.LocalPlayers[0].camera.GetComponent<AudioListener>().enabled = true;
                GameState.LocalPlayers[1].camera.GetComponent<Camera>().rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                GameState.LocalPlayers[1].camera.GetComponent<Camera>().orthographicSize = cameraDistance;
                GameState.LocalPlayers[1].camera.GetComponent<AudioListener>().enabled = false;
                GameState.LocalPlayers[2].camera.GetComponent<Camera>().rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                GameState.LocalPlayers[2].camera.GetComponent<Camera>().orthographicSize = cameraDistance;
                GameState.LocalPlayers[2].camera.GetComponent<AudioListener>().enabled = false;
                GameState.LocalPlayers[3].camera.GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                GameState.LocalPlayers[3].camera.GetComponent<Camera>().orthographicSize = cameraDistance;
                GameState.LocalPlayers[3].camera.GetComponent<AudioListener>().enabled = false;
                break;
            default: 
                // TODO: Setup generic camera that zooms out to fit all players
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
        GameState.Gamemode = Gamemode.DeathMatch;
        mapLoader.LoadRandomMap(Gamemode.DeathMatch);
    }

    private void SetupPlayerTeam(Player player)
    {
        switch (GameState.Gamemode)
        {
            case Gamemode.MainMenu:
                break;
            case Gamemode.DeathMatch:
                AutoBalancePlayer(player);
                break;
            case Gamemode.Mission:
                SetPlayerTeam(player, Teams.TeamOne);
                break;
        }
    }

    private void SetupGameData() 
    {
        GameState.Teams.Add(new Team { TeamLayer = Teams.TeamOne, NumberOfActors = 0 });
        GameState.Teams.Add(new Team { TeamLayer = Teams.TeamTwo, NumberOfActors = 0 });
        GameState.Teams.Add(new Team { TeamLayer = Teams.TeamThree, NumberOfActors = 0 });
        GameState.Teams.Add(new Team { TeamLayer = Teams.TeamFour, NumberOfActors = 0 });
        GameState.Teams.Add(new Team { TeamLayer = Teams.TeamFive, NumberOfActors = 0 });
    }

    private void SetPlayerTeam(Player player, Teams teamLayer)
    {
        // Set the layer on unity
        player.gameObject.layer = (int)teamLayer;

        // Save on gamestate
        var t = GameState.Teams.Where(_ => _.TeamLayer == teamLayer).FirstOrDefault();
        t.TeamLayer = teamLayer;
        t.NumberOfActors++;
        //TODO: add team layer to gamestate player object

    }

    private void AutoBalancePlayer(Player player)
    {
        // This should be used only on modes where the player can join a random team
        var leastPlayers = GameState.Teams.Min(x => x.NumberOfActors);
        var bestTeamToJoin = GameState.Teams.Where(_ => _.NumberOfActors == leastPlayers).FirstOrDefault(); // Any team with less players will do

        SetPlayerTeam(player, bestTeamToJoin.TeamLayer);
    }

    private void RemovePlayerFromTeam(Player player)
    {
        var layer = player.gameObject.layer; 
        var team = GameState.Teams.Where(t => (int)t.TeamLayer == layer).FirstOrDefault();
        team.NumberOfActors--;
    }
}
