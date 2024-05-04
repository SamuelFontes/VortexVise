using System.Numerics;
using System.Text.Json.Serialization;
using VortexVise.Models;
using VortexVise.States;

namespace VortexVise;
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<GameState>))]
[JsonSerializable(typeof(List<MasterServer>))]
[JsonSerializable(typeof(List<GameLobby>))]
public partial class SourceGenerationContext : JsonSerializerContext
{
}
