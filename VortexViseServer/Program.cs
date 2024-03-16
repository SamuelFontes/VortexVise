using System.Net;
using System.Net.Sockets;
using System.Text;
using VortexVise.GameGlobals;
using VortexVise.Logic;
using VortexVise.States;

GameCore.IsServer = true;

Console.WriteLine("VortexVise Server Started!");
IPEndPoint ipep = new IPEndPoint(IPAddress.Any, GameCore.NetworkPort);
UdpClient newsock = new UdpClient(ipep);
Console.WriteLine("Server running on port "+GameCore.NetworkPort);

Console.WriteLine("Waiting for a client...");

IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

int tickrate = GameCore.GameTickRate;
double deltaTime = 1d / tickrate;
double currentTime = 0;
var lastTime = currentTime;


List<Player> players = new List<Player>();
//List<GameState> gameStates = new List<GameState>();
GameState lastState = new();
lastState.CurrentTime = currentTime;
lastState.Gravity = 1800;
//gameStates.Add(lastState);
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

                if(existingPlayer != null)
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
                // TODO: New player joined, do the thing
                Console.WriteLine("Player Connected");
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
