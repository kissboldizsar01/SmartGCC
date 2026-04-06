namespace SmartGCC.Models;

public sealed class ProcessConfig
{
    public string[] GccArgs { get; init; } = [];

    public bool RawMode { get; init; }
}
