using VortexVise.Core.Enums;
using VortexVise.Core.Models;
using VortexVise.Core.States;
using VortexVise.Desktop.States;

namespace VortexVise.Desktop.Networking;

public class NetworkMessage
{
    public Guid OwnerId { get; set; } = Guid.Empty;
    public NetworkMessageType MessageType { get; set; }
    public GameState? GameState { get; set; }
    public List<InputState> InputStates { get; set; } = [];
    public List<PlayerProfile> PlayerProfiles { get; set; } = [];
}
