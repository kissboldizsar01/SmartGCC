using SmartGCC.Modules;

namespace SmartGCC;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var argumentHandler = new ArgumentHandler();
        var processRunner = new ProcessRunner();
        var outputParser = new OutputParser();
        var errorTranslator = new ErrorTranslator();
        var consoleFormatter = new ConsoleFormatter();

        var config = argumentHandler.Handle(args);
        var processResult = await processRunner.RunAsync(config);

        if (config.RawMode)
        {
            Console.Write(processResult.Stderr);
            return processResult.ExitCode;
        }

        if (processResult.ExitCode != 0)
        {
            var parserResult = outputParser.Parse(processResult.Stderr);
            var diagnostics = parserResult.Errors;

            if (diagnostics.Count > 0)
            {
                var enrichedErrors = errorTranslator.Enrich(diagnostics);
                consoleFormatter.Render(enrichedErrors, parserResult.UnmatchedLines);
                return processResult.ExitCode;
            }

            Console.Write(processResult.Stderr);
            return processResult.ExitCode;
        }

        var successParserResult = outputParser.Parse(processResult.Stderr);
        if (successParserResult.Errors.Count > 0)
        {
            var enrichedWarnings = errorTranslator.Enrich(successParserResult.Errors);
            consoleFormatter.Render(enrichedWarnings, successParserResult.UnmatchedLines);
            return 0;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Fordítás sikeres!");
        Console.ResetColor();
        return 0;
    }
}
