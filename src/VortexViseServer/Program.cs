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
List<GameLobby> lobbies = [];

app.MapGet("/", () =>
{
    return "VortexViseServer";
});

app.MapGet("/list", () =>
{
    var json = JsonSerializer.Serialize(lobbies, SourceGenerationContext.Default.ListGameLobby);
    return json;
});

app.MapPost("/host", (string serializedProfile) =>
{
    var profile = JsonSerializer.Deserialize(serializedProfile, SourceGenerationContext.Default.PlayerProfile); ;
    if (profile == null) return "Can't deserialize profile";
    var lobby = new GameLobby(profile);
    lobbies.Add(lobby);
    return JsonSerializer.Serialize(lobby, SourceGenerationContext.Default.GameLobby); ;
});

app.MapPut("/join", (Guid lobbyId, string serializedProfile) =>
{
    var profile = JsonSerializer.Deserialize(serializedProfile, SourceGenerationContext.Default.PlayerProfile); ;
    if (profile == null) return "Can't deserialize profile";
    var lobby = lobbies.FirstOrDefault(x => x.Id == lobbyId);
    if (lobby == null) return "Lobby not found";
    lobby.Players.Add(profile);
    return JsonSerializer.Serialize(lobby, SourceGenerationContext.Default.GameLobby); ;
});

app.MapGet("/GetLobby", (Guid lobbyId) =>
{
    var lobby = lobbies.FirstOrDefault(x => x.Id == lobbyId);
    if (lobby == null) return "Lobby not found";
    return JsonSerializer.Serialize(lobby, SourceGenerationContext.Default.GameLobby); ;
});

app.UseResponseCompression();


app.Run();

// Do the socket thing
var port = 4090;
GameCore.IsServer = true;
Console.WriteLine("VortexVise Server Started!");
IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
UdpClient newsock = new UdpClient(ipep);
Console.WriteLine("Server running on port " + port);


Console.WriteLine("BeforeSender");
IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
Console.WriteLine("AfterSender");

int tickrate = GameCore.GameTickRate;
double deltaTime = 1d / tickrate;
double currentTime = 0;
var lastTime = currentTime;

/*
List<Player> players = new List<Player>();
//List<GameState> gameStates = new List<GameState>();
GameState lastState = new();
lastState.CurrentTime = currentTime;
lastState.Gravity = 1800;
//gameStates.Add(lastState);
var watch = System.Diagnostics.Stopwatch.StartNew();


// Start receiving networking packets
Thread myThread = new Thread(new ThreadStart(ReceivePlayerPackets));
myThread.Start();

while (true)
{
    currentTime = watch.Elapsed.TotalSeconds;
    var time = currentTime - lastTime;
    if (time > deltaTime)
    {
        // Simulate new game state
        foreach (var lastPlayerState in lastState.PlayerStates)
        {
            var player = players.FirstOrDefault(_ => _.Id == lastPlayerState.Id);
            if (player == null) continue;
            lastPlayerState.Input = player.Input;
        }
        state = GameLogic.SimulateState(lastState, currentTime, Guid.Empty, (float)deltaTime, true);
        lastTime = currentTime;
        lastState = state;

        // Send simulation to everyone
        // The response always should be the last simulated state every, so if a new state wasn't simulated it will send the last one
        Thread sendThread = new Thread(new ThreadStart(() =>
        {
            SendState(state);
        }));
        sendThread.Start();
    }

}
void SendState(GameState state)
{
    var response = state.SerializeState();

    byte[] responseData = Encoding.ASCII.GetBytes(response);
    List<Task> tasks = new List<Task>();
    foreach (var player in players)
    {
        tasks.Add(
            ((Func<Task>)(async () =>
            {
                newsock.Send(responseData, responseData.Length, player.Sender);
            }))());
    }
    Task.WaitAll(tasks.ToArray());

}

void ReceivePlayerPackets()
{
    while (true)
    {
        try
        {
            // receive players input
            byte[] data = newsock.Receive(ref sender);

            string receivedData = Encoding.ASCII.GetString(data, 0, data.Length);
            //Console.WriteLine(receivedData);

            // Should read the input, simulate the state, return the simulated state
            (Guid playerId, InputState input, double receivedTime) = GameState.DeserializeInput(receivedData);

            var p = players.FirstOrDefault(_ => _.Id == playerId);
            if (p == null)
            {
                var existingPlayer = players
                    .FirstOrDefault(x => x.Sender.Address.ToString() == sender.Address.ToString());

                if (existingPlayer != null)
                {
                    players.Remove(existingPlayer);
                    // I suppose this is never going to be null, since the player actually entered the server that might be already a state
                    // so let's just remove it.
                    var playerLastState = lastState.PlayerStates
                        .FirstOrDefault(x => x.Id == existingPlayer.Id);
                    lastState.PlayerStates.Remove(playerLastState);
                }

                p = new Player()
                {
                    Id = playerId,
                    Input = input,
                    Time = receivedTime,
                    Sender = sender,
                };
                players.Add(p);
                Console.WriteLine("Player Connected");
                lastState.PlayerStates.Add(new(playerId));
            }
            if (p.Time < receivedTime)
            {
                p.Input = input;
                p.Time = receivedTime;
            }

        }
        catch
        {

        }

    }
}
class PlayerClient
{
    public PlayerProfile Profile { get; set; }
    public IPEndPoint Sender { get; set; }
}
*/
