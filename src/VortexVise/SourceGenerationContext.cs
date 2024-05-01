using System.Numerics;
using System.Text.Json.Serialization;
using VortexVise.Models;
using VortexVise.States;

namespace VortexVise;
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<GameState>))]
[JsonSerializable(typeof(List<MasterServer>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
