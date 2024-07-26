// This is needed to be able to serialize using AOT compilation
using System.Text.Json.Serialization;
using VortexVise.Models;
using VortexVise.States;

namespace VortexVise;
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<GameState>))]
[JsonSerializable(typeof(List<MasterServer>))]
[JsonSerializable(typeof(List<GameLobby>))]
[JsonSerializable(typeof(List<Map>))]
[JsonSerializable(typeof(List<PlayerProfile>))]
public partial class SourceGenerationContext : JsonSerializerContext
{
}
