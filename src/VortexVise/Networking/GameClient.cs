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

public static class GameClient
{
    static public bool IsConnected = false;
    public static bool IsConnecting = false;
    public static string IP = "127.0.0.1";
    public static int Port = 8080;
    private static TcpClient? _client;
    static public GameState LastServerState = new();
    static public ulong LastTickSimluated = 0;
    static public long Ping = -1;
    public static HttpClient httpClient = new();
    public static GameLobby? CurrentLobby;
    public static List<GameLobby>? AvailableLobbies = [];
    public static bool IsHost { get { return CurrentLobby?.Owner.Id == GameCore.PlayerOneProfile.Id; } }
    static public void Connect(string ip)
    {
        try
        {
            var ipAndPort = ip.Split(":");
            IP = ipAndPort[0];
            Port = Convert.ToInt32(ipAndPort[1]);
            IsConnecting = true;
            _client = new TcpClient();
            _client.Connect(IP, Port);
            LastTickSimluated = 0;
            IsConnected = true;
            Thread getState = new(new ThreadStart(GetState));
            getState.Start();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            Disconnect();
        }
    }

    static public void Disconnect()
    {
        try
        {
            IsConnecting = false;
            IsConnected = false;
            _client?.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            IsConnected = false;
        }

    }

    static public bool SendState(GameState state)
    {
        bool wasSent = false;
        try
        {
            // Sends a message to the host to which you have connected.
            string json = GameStateSerializer.SerializeState(state);
            byte[] sendBytes = Encoding.ASCII.GetBytes(json);

            //_udpClient.Send(sendBytes, sendBytes.Length);


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

            //_udpClient.Send(sendBytes, sendBytes.Length);


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
        while (IsConnected)
        {
            try
            {
                //IPEndPoint object will allow us to read datagrams sent from any source.
                IPEndPoint RemoteIpEndPoint = new(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                //byte[] receiveBytes = _udpClient.Receive(ref RemoteIpEndPoint);
                //string returnData = Encoding.ASCII.GetString(receiveBytes);
                //Console.WriteLine(returnData);

                // Uses the IPEndPoint object to determine which of these two hosts responded.
                //var state = GameStateSerializer.DeserializeState(returnData);
                //if (state.Tick > LastTickSimluated)
                //{
                //LastServerState = state;
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
}
