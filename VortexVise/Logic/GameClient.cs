using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using VortexVise.States;

namespace VortexVise.Logic
{
    public class GameClient
    {
        string ip = "192.168.1.166";
        public bool IsConnected = false;
        private UdpClient _udpClient = new UdpClient(11000);
        public GameState LastServerState = new GameState();
        public void Connect()
        {

            // This constructor arbitrarily assigns the local port number.
            try
            {
                _udpClient.Connect(ip, 9050);
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
                Console.WriteLine(json);
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

                    // Uses the IPEndPoint object to determine which of these two hosts responded.
                    LastServerState = GameState.DeserializeState(returnData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
        }
    }
}
