using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using VortexVise.States;
using Raylib_cs;// NEEDS TO BE HERE
using System.Numerics;// NEEDS TO BE HERE
using VortexVise.Utilities;// NEEDS TO BE HERE

namespace VortexVise.Logic
{
    public class GameClient
    {
        private UdpClient _udpClient = new UdpClient(11000);
        public void Connect()
        {

            // This constructor arbitrarily assigns the local port number.
            try
            {
                _udpClient.Connect("localhost", 9050);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _udpClient.Close();
            }
        }

        public void Disconnect()
        {
            try
            {
                _udpClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public bool SendState(GameState state)
        {
            bool wasSent = false;
            try
            {
                // Sends a message to the host to which you have connected.
                state.PrepareSerialization();
                string json = JsonSerializer.Serialize(state);
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
        public GameState? GetState()
        {
            GameState? state = null;
            try
            {
                //IPEndPoint object will allow us to read datagrams sent from any source.
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = _udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);

                // Uses the IPEndPoint object to determine which of these two hosts responded.
                Console.WriteLine("This is the message you received " +
                                             returnData.ToString());
                Console.WriteLine("This message was sent from " +
                                            RemoteIpEndPoint.Address.ToString() +
                                            " on their port number " +
                                            RemoteIpEndPoint.Port.ToString());

                state = JsonSerializer.Deserialize<GameState>(returnData);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return state;
        }
    }
}
