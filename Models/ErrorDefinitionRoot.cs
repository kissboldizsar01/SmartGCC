using System.Text.Json.Serialization;

namespace SmartGCC.Models;

public sealed class ErrorDefinitionRoot
{
    [JsonPropertyName("errors")]
    public List<ErrorDefinition> Errors { get; init; } = [];
}
