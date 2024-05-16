using System.Net;
using VortexVise.Models;

namespace VortexViseServer;

public class PlayerClient
{
    public PlayerProfile Profile { get; set; }
    public GameLobby GameLobby { get; set; }
    public IPEndPoint Sender { get; set; }
}
