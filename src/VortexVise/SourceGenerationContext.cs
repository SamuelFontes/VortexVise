using System.Numerics;
using System.Text.Json.Serialization;
using VortexVise.Models;
using VortexVise.States;

namespace VortexVise;
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(SerializableReplay))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
