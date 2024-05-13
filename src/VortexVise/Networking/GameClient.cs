using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using VortexVise.GameGlobals;
using VortexVise.Models;
using VortexVise.States;
using VortexVise.Utilities;

namespace VortexVise.Networking;

// TODO: Rebuild this using SignalR
public static class GameClient
{
    static public bool IsConnected = false;
    public static bool IsConnecting = false;
    private static UdpClient _udpClient = new(11000);
    static public GameState LastServerState = new();
    static public double LastSimulatedTime = 0;
    static public long Ping = -1;
    public static HttpClient httpClient = new();
    public static MasterServer Server = new();
    public static GameLobby? CurrentLobby;
    public static List<GameLobby>? AvailableLobbies = [];
    static public async Task Connect(MasterServer server)
    {

        try
        {
            IsConnecting = true;
            Server = server;
            var serverResponse = await httpClient.GetAsync(server.ServerURL);
            string serverMessage = await serverResponse.Content.ReadAsStringAsync();
            if (serverMessage != "VortexViseServer") throw new Exception("Can't connect to server");

            _udpClient.Dispose();
            _udpClient = new(11000);
            _udpClient.Connect(server.ServerUDP, server.ServerUDPPort);
            IsConnected = true;
            UpdatePing();
            Thread getPingThread = new(new ThreadStart(GetServerInfo));
            getPingThread.Start();

        }
        catch (Exception e)
        {
            IsConnecting = false;
            Console.WriteLine(e.ToString());
            _udpClient.Close();
            IsConnected = false;
        }
    }

    static public void Disconnect()
    {
        try
        {
            IsConnecting = false;
            _udpClient.Close();
            IsConnected = false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            IsConnected = false;
        }

    }

    static public void CreateLobby()
    {
        if (!IsConnected) return;

        List<PlayerProfile> list = Utils.GetAllLocalPlayerProfiles();

        string serializedProfiles = JsonSerializer.Serialize(list, SourceGenerationContext.Default.ListPlayerProfile);

        var response = httpClient.PostAsync(Server.ServerURL + $"/host?serializedProfiles={serializedProfiles}", null).Result;
        var serializedLobby = response.Content.ReadAsStream();
        CurrentLobby = JsonSerializer.Deserialize(serializedLobby, SourceGenerationContext.Default.GameLobby);
    }

    public static void ListLobbies()
    {
        if (!IsConnected) return;

        var serverResponse = httpClient.GetAsync(Server.ServerURL + "/list").Result.Content.ReadAsStream();
        AvailableLobbies = JsonSerializer.Deserialize(serverResponse, SourceGenerationContext.Default.ListGameLobby);
    }

    public static void JoinLobby(Guid lobbyId)
    {
        if (!IsConnected) return;
        List<PlayerProfile> list = Utils.GetAllLocalPlayerProfiles();

        string serializedProfiles = JsonSerializer.Serialize(list, SourceGenerationContext.Default.ListPlayerProfile); ;

        var response = httpClient.PostAsync(Server.ServerURL + $"/join?serializedProfiles={serializedProfiles}&lobbyId={lobbyId}", null).Result;
        var serializedLobby = response.Content.ReadAsStream();
        CurrentLobby = JsonSerializer.Deserialize(serializedLobby, SourceGenerationContext.Default.GameLobby);

    }

    public static void GetLobbyInfo()
    {
        if (!IsConnected) return;
        if (CurrentLobby == null) return;

        var serverResponse = httpClient.GetAsync(Server.ServerURL + $"/GetLobby?lobbyId={CurrentLobby.Id}").Result.Content.ReadAsStream();
        CurrentLobby = JsonSerializer.Deserialize(serverResponse, SourceGenerationContext.Default.GameLobby);
    }

    static public bool SendState(GameState state)
    {
        bool wasSent = false;
        try
        {
            // Sends a message to the host to which you have connected.
            string json = GameStateSerializer.SerializeState(state);
            byte[] sendBytes = Encoding.ASCII.GetBytes(json);

            _udpClient.Send(sendBytes, sendBytes.Length);


            wasSent = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        return wasSent;
    }
    static public bool SendInput(InputState input, Guid playerId, ulong tick)
    {
        bool wasSent = false;
        try
        {
            // Sends a message to the host to which you have connected.
            string json = GameStateSerializer.SerializeInput(input, playerId, tick);
            byte[] sendBytes = Encoding.ASCII.GetBytes(json);

            _udpClient.Send(sendBytes, sendBytes.Length);


            wasSent = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        return wasSent;
    }

    static public void GetState()
    {
        while (true)
        {
            if (!IsConnected) break;
            try
            {
                //IPEndPoint object will allow us to read datagrams sent from any source.
                IPEndPoint RemoteIpEndPoint = new(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                byte[] receiveBytes = _udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                Console.WriteLine(returnData);

                // Uses the IPEndPoint object to determine which of these two hosts responded.
                var state = GameStateSerializer.DeserializeState(returnData);
                if (state.CurrentTime > LastSimulatedTime)
                    LastServerState = state;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
    static public async void GetServerInfo()
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));

        while (await timer.WaitForNextTickAsync())
        {
            if (!IsConnected) break;
            UpdatePing();
            if (CurrentLobby == null) ListLobbies();
            else GetLobbyInfo();
        }
    }

    static void UpdatePing()
    {
        try
        {
            Ping ping = new();
            PingReply reply = ping.Send(Server.ServerUDP, 1000);
            Ping = reply.RoundtripTime;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
