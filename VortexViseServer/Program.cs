using Raylib_cs;
using System.Net;
using System.Net.Sockets;
using System.Text;
using VortexVise.States;


Console.WriteLine("VortexVise Server Started!");
IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
UdpClient newsock = new UdpClient(ipep);

Console.WriteLine("Waiting for a client...");

IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

int tickrate = 64;
double deltaTime = 1d / tickrate;
double currentTime = Raylib.GetTime();
var lastTime = currentTime;


while (true)
{
    currentTime = Raylib.GetTime();
    var time = currentTime - lastTime;
    
    // receive everyones input
    byte[] data = newsock.Receive(ref sender);

    Console.WriteLine("Message received from {0}:", sender.ToString());
    string receivedData = Encoding.ASCII.GetString(data, 0, data.Length);
    Console.WriteLine(receivedData);

    if(time > deltaTime)
    {
        lastTime = currentTime;
    }
    // simulate state
    var state = GameState.DeserializeState(receivedData);

    var response = state.SerializeState();
    Console.WriteLine(response);

    byte[] responseData = Encoding.ASCII.GetBytes(response);
    newsock.Send(responseData, responseData.Length, sender);
}
