using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using VortexVise.States;

namespace VortexViseServer
{
    internal class GameServer
    {
        public void Run()
        {
            Console.WriteLine("VortexVise Server Started!");
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
            UdpClient newsock = new UdpClient(ipep);

            Console.WriteLine("Waiting for a client...");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);



            while (true)
            {
                byte[] data = newsock.Receive(ref sender);

                Console.WriteLine("Message received from {0}:", sender.ToString());
                string receivedData = Encoding.ASCII.GetString(data, 0, data.Length);
                Console.WriteLine(receivedData);

                var state = GameState.DeserializeState(receivedData);
                state.Gravity = 69;

                var response = state.SerializeState();
                Console.WriteLine(response);

                byte[] responseData = Encoding.ASCII.GetBytes(response);
                newsock.Send(responseData, responseData.Length, sender);
            }

        }

    }
}
