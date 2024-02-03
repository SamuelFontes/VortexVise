using Raylib_cs;
using System.Net;
using System.Net.Sockets;
using System.Text;
using VortexVise.GameObjects;
using VortexVise.Logic;
using VortexVise.States;


Console.WriteLine("VortexVise Server Started!");
IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
UdpClient newsock = new UdpClient(ipep);

Console.WriteLine("Waiting for a client...");

IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

int tickrate = 64;
double deltaTime = 1d / tickrate;
double currentTime = Raylib.GetTime();
var lastTime = currentTime;


List<Player> players = new List<Player>();
List<GameState> gameStates = new List<GameState>();
GameState lastState = new();
lastState.CurrentTime = currentTime;
lastState.Gravity = 1800;
gameStates.Add(lastState);
var watch = System.Diagnostics.Stopwatch.StartNew();

GameState state = new GameState();
state = lastState;
MapLogic.LoadMap("SkyArchipelago", true);
PlayerLogic.Init(true);

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
            var player = players.FirstOrDefault(_ => _.Id == lastPlayerState.Id); // TODO: optimize this
            if (player == null) continue;
            lastPlayerState.Input = player.Input;
        }
        state = GameLogic.SimulateState(lastState, currentTime, Guid.Empty, (float)deltaTime, true);
        lastTime = currentTime;
        lastState = state;

        // Send simulation to everyone
        // The response always should be the last simulated state every, so if a new state wasn't simulated it will send the last one
        var response = state.SerializeState();

        byte[] responseData = Encoding.ASCII.GetBytes(response);
        foreach (var player in players)
        {
            newsock.Send(responseData, responseData.Length, player.Sender);
        }
    }

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

            // Should read the input, simulate the state, return the simulated state
            (Guid playerId, InputState input, double receivedTime) = GameState.DeserializeInput(receivedData);

            var p = players.FirstOrDefault(_ => _.Id == playerId);
            if (p == null)
            {
                p = new Player()
                {
                    Id = playerId,
                    Input = input,
                    Time = receivedTime,
                    Sender = sender,
                };
                players.Add(p);
                // TODO: New player joined, do the thing
                lastState.PlayerStates.Add(new(playerId));
                // TODO: make it handle player disconnect on timeout
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
class Player
{
    public Guid Id;
    public InputState Input;
    public double Time;
    public IPEndPoint Sender;
};
