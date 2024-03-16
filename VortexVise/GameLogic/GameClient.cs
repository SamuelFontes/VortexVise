using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using VortexVise.GameGlobals;
using VortexVise.States;

namespace VortexVise.Logic;

public static class GameClient
{
    static public bool IsConnected = false;
    static private UdpClient _udpClient = new UdpClient(11000);
    static public GameState LastServerState = new GameState();
    static public double LastSimulatedTime = 0;
    static public long Ping = 0;
    static public bool Connect()
    {

        // This constructor arbitrarily assigns the local port number.
        try
        {
            _udpClient.Connect(GameCore.ServerIPAddress, GameCore.NetworkPort);
            IsConnected = true;
            UpdatePing();
            Thread getPingThread = new Thread(new ThreadStart(GetServerLatency));
            getPingThread.Start();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            _udpClient.Close();
            IsConnected = false;
        }
        return IsConnected;
    }

    static public void Disconnect()
    {
        try
        {
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
            string json = state.SerializeState();
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
            string json = GameState.SerializeInput(input, playerId, time);
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
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                byte[] receiveBytes = _udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                Console.WriteLine(returnData);

                // Uses the IPEndPoint object to determine which of these two hosts responded.
                var state = GameState.DeserializeState(returnData);
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
        Ping ping = new Ping();
        PingReply reply = ping.Send(GameCore.ServerIPAddress, 1000);
        Ping = reply.RoundtripTime;
    }
}
