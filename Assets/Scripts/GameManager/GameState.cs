using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }
    public Gamemode Gamemode {  get; private set; }
    public List<Player> LocalPlayers { get; private set; }
    public List<Team> MatchTeams { get; private set; } 
    public Map CurrentMap { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        Gamemode = Gamemode.MainMenu;
        LocalPlayers = new List<Player>(); 
        MatchTeams = new List<Team>();
        LoadTeamsData();
   }

    public int GetNumberOfLocalPlayers()
    {
        return LocalPlayers.Count;
    }
    public void SetGamemode(Gamemode gamemode)
    {
        Gamemode = gamemode;
    }

    public void AddLocalPlayer(Player player)
    {
        LocalPlayers.Add(player);
    }

    private void LoadTeamsData()
    {
        MatchTeams.Add(new Team(Teams.TeamOne));
        MatchTeams.Add(new Team (Teams.TeamTwo));
        MatchTeams.Add(new Team (Teams.TeamThree));
        MatchTeams.Add(new Team (Teams.TeamFour));
        MatchTeams.Add(new Team (Teams.TeamFive));
    }
    public void SetPlayerTeam(Player player, Teams teamLayer)
    {
        // Set the layer on unity
        player.gameObject.layer = (int)teamLayer;

        // Save on gamestate
        var team = MatchTeams.Where(_ => _.TeamLayer == teamLayer).FirstOrDefault();
        team.AddActorToTeam();

    }

    public void AutoBalancePlayer(Player player)
    {
        // This should be used only on modes where the player can join a random team
        var leastPlayers = MatchTeams.Min(x => x.NumberOfActors);
        var bestTeamToJoin = MatchTeams.Where(_ => _.NumberOfActors == leastPlayers).FirstOrDefault(); // Any team with less players will do

        SetPlayerTeam(player, bestTeamToJoin.TeamLayer);
    }

    public void RemovePlayerFromTeam(Player player)
    {
        var layer = player.gameObject.layer; 
        var team = MatchTeams.Where(t => (int)t.TeamLayer == layer).FirstOrDefault();
        team.RemoveActorFromTeam();
    }

    public void SetCurrentMap(Map map)
    {
        CurrentMap = map;
    }
}
