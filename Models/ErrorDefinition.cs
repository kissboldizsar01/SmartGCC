using System.Text.Json.Serialization;

namespace SmartGCC.Models;

public sealed class ErrorDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("matchPattern")]
    public string MatchPattern { get; set; } = string.Empty;

    [JsonPropertyName("friendlyTitle")]
    public string FriendlyTitle { get; set; } = string.Empty;

    [JsonPropertyName("explanation")]
    public string Explanation { get; set; } = string.Empty;

    [JsonPropertyName("suggestion")]
    public string Suggestion { get; set; } = string.Empty;

    [JsonPropertyName("appliesTo")]
    public string AppliesTo { get; set; } = "both";
}
