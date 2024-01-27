using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
                Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));

                string response = "Welcome to my test server";
                byte[] responseData = Encoding.ASCII.GetBytes(response);
                newsock.Send(responseData, responseData.Length, sender);
            }

        }

    }
}
