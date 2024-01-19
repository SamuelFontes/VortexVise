using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

IPHostEntry host = Dns.GetHostEntry("localhost");
IPAddress ipAddress = host.AddressList[0];
IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

// Create a Socket that will use Tcp protocol
Socket tcpListener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
// A Socket must be associated with an endpoint using the Bind method
tcpListener.Bind(localEndPoint);
// Specify how many requests a Socket can listen before it gives Server busy response.
// We will listen 10 requests at a time
tcpListener.Listen(10);

Console.WriteLine("Waiting for a connection...");
Socket handler = tcpListener.Accept();

// Incoming data from the client.
string data = null;
byte[] bytes = null;

while (true)
{
    bytes = new byte[1024];
    int bytesRec = handler.Receive(bytes);
    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
    if (data.IndexOf("<EOF>") > -1)
    {
        break;
    }
}

Console.WriteLine("Text received : {0}", data);

byte[] msg = Encoding.ASCII.GetBytes(data);
handler.Send(msg);
handler.Shutdown(SocketShutdown.Both);
handler.Close();
