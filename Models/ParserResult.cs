namespace SmartGCC.Models;

/// <summary>
/// Represents the result of parsing GCC stderr output.
/// </summary>
public sealed class ParserResult
{
    /// <summary>
    /// Gets the parsed error and warning entries.
    /// </summary>
    public List<ParsedError> Errors { get; } = [];

    /// <summary>
    /// Gets the lines that did not match the expected GCC format.
    /// </summary>
    public List<string> UnmatchedLines { get; } = [];
}
