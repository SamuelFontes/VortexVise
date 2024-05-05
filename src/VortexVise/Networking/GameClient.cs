using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using VortexVise.GameGlobals;
using VortexVise.Models;
using VortexVise.States;

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
            //Thread getPingThread = new(new ThreadStart(GetServerLatency));
            //getPingThread.Start();

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
    static public bool SendInput(InputState input, Guid playerId, double time)
    {
        bool wasSent = false;
        try
        {
            // Sends a message to the host to which you have connected.
            string json = GameStateSerializer.SerializeInput(input, playerId, time);
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
    static public async void GetServerLatency()
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));

        while (await timer.WaitForNextTickAsync())
        {
            //Business logic
            UpdatePing();
        }
    }
    static void UpdatePing()
    {
        if (IsConnected)
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
