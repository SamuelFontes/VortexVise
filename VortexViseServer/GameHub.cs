﻿using Microsoft.AspNetCore.SignalR;
using VortexVise.Models;
using VortexVise.States;

namespace VortexViseServer;

public class GameHub : Hub
{
    public List<GameMatch> matches = new List<GameMatch>();

    public async Task JoinGame(Guid id)
    {
        var match = matches.FirstOrDefault(match => match.Id == id);
        if (match == null) await Clients.Caller.SendAsync("JoinGame", false);
        if (match.Players.Count >= match.MaxPlayers) await Clients.Caller.SendAsync("JoinGame", false);

        match.Players.Add(Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, match.Id.ToString());
        await Clients.Caller.SendAsync("JoinGame", true);
    }

    public async Task CreateGame()
    {
        var game = new GameMatch();
        game.MatchOwner = Context.ConnectionId;
        game.MaxPlayers = 8;
        game.Players.Add(Context.ConnectionId);
        game.Id = Guid.NewGuid();
        matches.Add(game);
        await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());
        await Clients.Caller.SendAsync("CreateGame", game);
    }

    public async Task ListGames()
    {
        await Clients.Caller.SendAsync("ListGames", matches);
    }

    public async Task SendState(GameState state, Guid matchId)
    {
        var match = matches.FirstOrDefault(match => match.Id == matchId);
        if (match == null || match.MatchOwner != Context.ConnectionId) return;

        await Clients.Group(match.Id.ToString()).SendAsync("GameState", state);
    }

    public async Task SendInput(InputState input, Guid matchId)
    {
        var match = matches.FirstOrDefault(match => match.Id == matchId);
        if (match == null || match.Players.Contains(Context.ConnectionId)) return;
        input.Owner = Context.ConnectionId;

        await Clients.Group(matchId.ToString()).SendAsync("SendInput",input);
    }
}