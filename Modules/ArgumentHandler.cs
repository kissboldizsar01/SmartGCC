using SmartGCC.Models;

namespace SmartGCC.Modules;

public sealed class ArgumentHandler
{
    /// <summary>
    /// Handles and validates SmartGCC command-line arguments.
    /// </summary>
    /// <param name="args">The raw command-line arguments from the application entry point.</param>
    /// <returns>A process configuration containing GCC arguments and raw mode setting.</returns>
    public ProcessConfig Handle(string[] args)
    {
        return Parse(args);
    }

    /// <summary>
    /// Parses and validates SmartGCC command-line arguments.
    /// </summary>
    /// <param name="args">The raw command-line arguments from the application entry point.</param>
    /// <returns>A process configuration containing GCC arguments and raw mode setting.</returns>
    public ProcessConfig Parse(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            Environment.Exit(1);
        }

        if (ContainsArg(args, "--help"))
        {
            PrintHelp();
            Environment.Exit(0);
        }

        if (ContainsArg(args, "--version"))
        {
            Console.WriteLine("SmartGCC v1.0.0");
            Environment.Exit(0);
        }

        var rawMode = ContainsArg(args, "--raw");
        var gccArgs = args
            .Where(a => !string.Equals(a, "--raw", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return new ProcessConfig
        {
            GccArgs = gccArgs,
            RawMode = rawMode
        };
    }

    private static bool ContainsArg(IEnumerable<string> args, string expected)
    {
        return args.Any(arg => string.Equals(arg, expected, StringComparison.OrdinalIgnoreCase));
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage: smartgcc [--raw] <gcc arguments>");
        Console.WriteLine("Try 'smartgcc --help' for more information.");
    }

    private static void PrintHelp()
    {
        Console.WriteLine("SmartGCC - GCC wrapper");
        Console.WriteLine("Usage: smartgcc [--raw] <gcc arguments>");
        Console.WriteLine("  --help     Show help information and exit.");
        Console.WriteLine("  --version  Print SmartGCC version and exit.");
        Console.WriteLine("  --raw      Enable passthrough mode.");
    }
}
