namespace SmartGCC.Models;

public sealed class ParsedError
{
    public string FileName { get; set; } = string.Empty;

    public int LineNumber { get; set; }

    public int ColumnNumber { get; set; }

    public ErrorType Type { get; set; }

    public string RawMessage { get; set; } = string.Empty;

    public string? SourceLine { get; set; }

    public string? PreviousLine { get; set; }

    public string? NextLine { get; set; }

    public string ErrorCode { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string Translation { get; set; } = string.Empty;

    public ErrorDefinition? Definition { get; set; }
}
