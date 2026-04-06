using System.Text.Json.Serialization;

namespace SmartGCC.Models;

[JsonSerializable(typeof(ErrorDefinitionRoot))]
[JsonSourceGenerationOptions(WriteIndented = true)]
internal partial class SmartGccJsonContext : JsonSerializerContext
{
}
