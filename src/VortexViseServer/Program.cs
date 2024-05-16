using Microsoft.AspNetCore.ResponseCompression;
using VortexVise.Models;
using VortexVise;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;
using VortexVise.GameGlobals;
using VortexVise.Logic;
using VortexVise.States;
using VortexViseServer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});

// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
GameAssets.Gameplay.LoadMapsHash();

app.MapGet("/", () =>
{
    return "VortexViseServer";
});

app.MapGet("/list", () =>
{
    var json = JsonSerializer.Serialize(GameServer.Lobbies, SourceGenerationContext.Default.ListGameLobby);
    return json;
});

app.MapPost("/host", (string serializedProfiles) =>
{
    List<PlayerProfile>? profiles = JsonSerializer.Deserialize(serializedProfiles, SourceGenerationContext.Default.ListPlayerProfile);
    if (profiles == null || profiles.Count == 0) throw new Exception("Can't deserialize profile");
    var lobby = GameServer.Lobbies.FirstOrDefault(x => x.Players.Count + profiles.Count <= x.MaxPlayers);
    if (lobby == null)
    {
        lobby = new GameLobby(profiles[0]);
        lobby.CurrentMap = GameAssets.Gameplay.Maps.OrderBy(x => Guid.NewGuid()).First();
    }
    foreach (var profile in profiles)
    {
        if (!lobby.Players.Any(x => x.Id == profile.Id)) lobby.Players.Add(profile);
    }

    GameServer.Lobbies.Add(lobby);
    return JsonSerializer.Serialize(lobby, SourceGenerationContext.Default.GameLobby);
});

app.MapPost("/join", (Guid lobbyId, string serializedProfiles) =>
{
    List<PlayerProfile>? profiles = JsonSerializer.Deserialize(serializedProfiles, SourceGenerationContext.Default.ListPlayerProfile); ;
    if (profiles == null || profiles.Count == 0) throw new Exception("Can't deserialize profile");

    var lobby = GameServer.Lobbies.FirstOrDefault(x => x.Id == lobbyId);
    if (lobby == null) return "Lobby not found";
    foreach (var profile in profiles)
    {
        lobby.Players.Add(profile);
    }
    return JsonSerializer.Serialize(lobby, SourceGenerationContext.Default.GameLobby); ;
});

app.MapGet("/GetLobby", (Guid lobbyId) =>
{
    var lobby = GameServer.Lobbies.FirstOrDefault(x => x.Id == lobbyId);
    if (lobby == null) return "Lobby not found";
    return JsonSerializer.Serialize(lobby, SourceGenerationContext.Default.GameLobby); ;
});

app.UseResponseCompression();


new Thread(new ThreadStart(GameServer.Run)).Start();
app.Run();

