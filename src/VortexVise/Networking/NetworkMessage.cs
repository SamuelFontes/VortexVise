using VortexVise.Core.Enums;
using VortexVise.Models;
using VortexVise.States;

namespace VortexVise.Networking;

public class NetworkMessage
{
    public Guid OwnerId { get; set; } = Guid.Empty;
    public NetworkMessageType MessageType { get; set; }
    public GameState? GameState { get; set; }
    public List<InputState> InputStates { get; set; } = [];
    public List<PlayerProfile> PlayerProfiles { get; set; } = [];
}
