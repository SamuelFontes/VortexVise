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
    // Start is called before the first frame update
    void Start()
    {
        // Setup game start menu
        SetupGameData();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //TODO: Create add menu player, after exits menu add localplayer
    public static void AddLocalPlayer(Player player)
    {
        GameState.localPlayers.Add(player);

        SetupCameras();
        if (GameState.gamemode == Gamemode.MainMenu)
            StartDeathMatch(); // FIXME: The gamemode shouldn't be changed when adding player, it should be set on the menu

        SetupPlayerTeam(player);
    }
    public static void RemoveLocalPlayer(Player player)
    {
        var p = GameState.localPlayers.Where(x => x.Id == player.Id).FirstOrDefault();
        GameState.localPlayers.Remove(p);
        SetupCameras();
        RemovePlayerFromTeam(player);
    }

    private static void SetupCameras()
    {
        // TODO: Maybe add a single camera mode on multiplayer when players are close
        // Reorder players, mouse and keyboard should aways be player one. Otherwise mouse aiming will go all funky
        for(int i = 0; i < GameState.localPlayers.Count; i++)
        {
            var player = GameState.localPlayers[i];
            var playerObject = GameObject.Find(player.Id);
            if(playerObject.GetComponent<PlayerInput>().currentControlScheme == "MouseAndKeyboard")
            {
                if (i == 0)
                    break;

                GameState.localPlayers.RemoveAt(i);
                GameState.localPlayers.Insert(0,player);
                break;
            }
        }

        // Check number of players
        switch (GameState.GetNumberOfLocalPlayers())
        {
            case 0:
                throw new Exception("Can't setup camera if there are no players");
            case 1:
                GameObject.Find(GameState.localPlayers[0].CameraId).GetComponent<CameraScript>().SetupCamera(new Rect(0f, 0f, 1f, 1f), 12, true);
                break;
            case 2:
                GameObject.Find(GameState.localPlayers[0].CameraId).GetComponent<CameraScript>().SetupCamera(new Rect(0f, 0f, 0.5f, 1f), 20, false);
                GameObject.Find(GameState.localPlayers[1].CameraId).GetComponent<CameraScript>().SetupCamera(new Rect(0.5f, 0f, 0.5f, 1f), 20, false);
                break; 
            case 3: 
                //TODO: Test this shit please
                
                //GameState.localPlayers[0].Camera.GetComponent<Camera>().rect = new Rect(0f, 0f, 0.5f, 1f); // This man has half of the screen, it's good to be player one
                //GameState.localPlayers[0].camera.GetComponent<Camera>().orthographicSize = 20;
                //GameState.localPlayers[1].camera.GetComponent<Camera>().rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                //GameState.localPlayers[1].camera.GetComponent<Camera>().orthographicSize = 20;
                //GameState.localPlayers[2].camera.GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                //GameState.localPlayers[2].camera.GetComponent<Camera>().orthographicSize = 20;
                break;
            case 4:
                //TODO: Test this shit please
                //GameState.localPlayers[0].camera.GetComponent<Camera>().rect = new Rect(0f, 0f, 0.5f, 0.5f); 
                //GameState.localPlayers[0].camera.GetComponent<Camera>().orthographicSize = 20;
                //GameState.localPlayers[1].camera.GetComponent<Camera>().rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                //GameState.localPlayers[1].camera.GetComponent<Camera>().orthographicSize = 20;
                //GameState.localPlayers[2].camera.GetComponent<Camera>().rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                //GameState.localPlayers[2].camera.GetComponent<Camera>().orthographicSize = 20;
                //GameState.localPlayers[3].camera.GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                //GameState.localPlayers[3].camera.GetComponent<Camera>().orthographicSize = 20;
                break;
            default: 
                // TODO: Setup generic camera that zooms out to fit all players
                break;
        }

    }
    
    private static void StartDeathMatch()
    {
        // FIXME: This just deletes everything from the menu, make this better
        var menuThings = GameObject.FindGameObjectsWithTag("MainMenu");
        foreach (var thing in menuThings)
        {
            UnityEngine.Object.Destroy(thing);
        }
        GameState.gamemode = Gamemode.DeathMatch;
    }

    private static void SetupPlayerTeam(Player player)
    {
        switch (GameState.gamemode)
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
        GameState.teams.Add(new Team { TeamLayer = Teams.TeamOne, NumberOfActors = 0 });
        GameState.teams.Add(new Team { TeamLayer = Teams.TeamTwo, NumberOfActors = 0 });
        GameState.teams.Add(new Team { TeamLayer = Teams.TeamThree, NumberOfActors = 0 });
        GameState.teams.Add(new Team { TeamLayer = Teams.TeamFour, NumberOfActors = 0 });
        GameState.teams.Add(new Team { TeamLayer = Teams.TeamFive, NumberOfActors = 0 });
    }

    private static void SetPlayerTeam(Player player, Teams teamLayer)
    {
        // Set the layer on unity
        GameObject.Find(player.Id).layer = (int)teamLayer;

        // Save on gamestate
        var t = GameState.teams.Where(_ => _.TeamLayer == teamLayer).FirstOrDefault();
        t.TeamLayer = teamLayer;
        t.NumberOfActors++;
        //TODO: add team layer to gamestate player object

    }

    private static void AutoBalancePlayer(Player player)
    {
        // This should be used only on modes where the player can join a random team
        var leastPlayers = GameState.teams.Min(x => x.NumberOfActors);
        var bestTeamToJoin = GameState.teams.Where(_ => _.NumberOfActors == leastPlayers).FirstOrDefault(); // Any team with less players will do

        SetPlayerTeam(player, bestTeamToJoin.TeamLayer);
    }

    private static void RemovePlayerFromTeam(Player player)
    {
        var layer = GameObject.Find(player.Id).layer; 
        var team = GameState.teams.Where(t => (int)t.TeamLayer == layer).FirstOrDefault();
        team.NumberOfActors--;
    }
}
