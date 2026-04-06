namespace SmartGCC.Models;

public sealed class ProcessResult
{
    public int ExitCode { get; init; }

    public string Stdout { get; init; } = string.Empty;

    public string Stderr { get; init; } = string.Empty;
}
