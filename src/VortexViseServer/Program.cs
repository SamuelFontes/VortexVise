using System.Net;
using System.Net.Sockets;
using System.Text;

TcpListener server = new TcpListener(IPAddress.Any, 9999);
// we set our IP address as server's address, and we also set the port: 9999
Console.WriteLine("Starting server");

server.Start();  // this will start the server

// TODO: Initialize game simulation
// TODO: thread to receive the messages, use what was received completely, one thread for each player??
// for every frame get all inputs
// place inputs in correct ticks
// simulate new state by simulating from the oldest input received
// send new game states
while (true)   //we wait for a connection
{
    TcpClient client = server.AcceptTcpClient();  //if a connection exists, the server will accept it

    NetworkStream ns = client.GetStream(); //networkstream is used to send/receive messages

    ns.Write(Encoding.Unicode.GetBytes(@"hello world"));     //sending the message

    while (client.Connected)  //while the client is connected, we look for incoming messages
    {
        string test = "";
        byte[] msg = new byte[1024];     //the messages arrive as byte array
        ns.ReadAsync(msg);
        var responseData = System.Text.Encoding.Unicode.GetString(msg, 0, msg.Length);
        test += responseData;


        Console.WriteLine("Received: {0}", test);
    }
}
