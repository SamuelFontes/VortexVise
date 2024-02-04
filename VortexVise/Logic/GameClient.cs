using Raylib_cs;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using VortexVise.States;

namespace VortexVise.Logic
{
    public class GameClient
    {
        string ip = "localhost";
        int port = 9050;
        public bool IsConnected = false;
        private UdpClient _udpClient = new UdpClient(11000);
        public GameState LastServerState = new GameState();
        public double LastSimulatedTime = 0;
        public void Connect()
        {

            // This constructor arbitrarily assigns the local port number.
            try
            {
                _udpClient.Connect(ip, port);
                IsConnected = true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _udpClient.Close();
                IsConnected = false;
            }
        }

        public void Disconnect()
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

        public bool SendState(GameState state)
        {
            bool wasSent = false;
            try
            {
                // Sends a message to the host to which you have connected.
                string json = state.SerializeState();
                Byte[] sendBytes = Encoding.ASCII.GetBytes(json);

                _udpClient.Send(sendBytes, sendBytes.Length);


                wasSent = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return wasSent;
        }
        public bool SendInput(InputState input, Guid playerId, double time)
        {
            bool wasSent = false;
            try
            {
                // Sends a message to the host to which you have connected.
                string json = GameState.SerializeInput(input, playerId, time);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(json);

                _udpClient.Send(sendBytes, sendBytes.Length);


                wasSent = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return wasSent;
        }
        public void GetState()
        {
            while (true)
            {
                try
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    // Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = _udpClient.Receive(ref RemoteIpEndPoint);
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
        public int GetPing()
        {
            return (int)((Raylib.GetTime() - LastSimulatedTime) * 10);
        }
    }
}
