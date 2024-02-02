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
MapLogic.LoadMap("SkyArchipelago", true);
PlayerLogic.Init(true);
while (true)
{

    // receive everyones input
    byte[] data = newsock.Receive(ref sender);

    Console.WriteLine("Message received from {0}:", sender.ToString());
    string receivedData = Encoding.ASCII.GetString(data, 0, data.Length);
    Console.WriteLine(receivedData);
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
        };
        players.Add(p);
        // TODO: New player joined, do the thing
        lastState.PlayerStates.Add(new(playerId));
        // TODO: make it handle player disconnect 
    }
    if(p.Time < receivedTime)
    {
        p.Input = input;
        p.Time = receivedTime;
    }




    currentTime = watch.Elapsed.TotalNanoseconds;
    var time = currentTime - lastTime;
    if (time > deltaTime)
    {
        // TODO: Simulate new gamestate
        state = GameLogic.SimulateState(lastState, currentTime, playerId, (float)deltaTime, true);
        lastTime = currentTime;
    }

    // Send simulation to everyone
    // The response always should be the last simulated state every, so if a new state wasn't simulated it will send the last one
    var response = state.SerializeState();
    Console.WriteLine(response);

    byte[] responseData = Encoding.ASCII.GetBytes(response);
    newsock.Send(responseData, responseData.Length, sender);
}
class Player
{
    public Guid Id;
    public InputState Input;
    public double Time;
};
