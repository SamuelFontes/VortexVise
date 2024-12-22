// This is needed to be able to serialize using AOT compilation
using System.Text.Json.Serialization;
using VortexVise.Core.Models;
using VortexVise.Core.States;
using VortexVise.Desktop.Models;
using VortexVise.Desktop.States;

namespace VortexVise.Desktop;
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<GameState>))]
[JsonSerializable(typeof(List<GameLobby>))]
[JsonSerializable(typeof(List<Map>))]
[JsonSerializable(typeof(List<PlayerProfile>))]
[JsonSerializable(typeof(PlayerProfile))]
[JsonSerializable(typeof(InputState))]
public partial class SourceGenerationContext : JsonSerializerContext
{
}
