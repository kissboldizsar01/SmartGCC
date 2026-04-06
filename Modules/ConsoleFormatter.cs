using SmartGCC.Models;

namespace SmartGCC.Modules;

public sealed class ConsoleFormatter
{
    private static readonly bool UseAsciiFallback = !SupportsEmojiOutput();

    /// <summary>
    /// Renders enriched parsed errors and unmatched GCC lines to the console.
    /// </summary>
    /// <param name="errors">The translated and enriched errors to render.</param>
    /// <param name="unmatchedLines">The GCC output lines that did not match the parser pattern.</param>
    public void Render(IReadOnlyList<ParsedError> errors, IReadOnlyList<string> unmatchedLines)
    {
        WriteSummary(errors, unmatchedLines, rawMode: false, rawStderr: string.Empty);
    }

    /// <summary>
    /// Renders enriched parsed errors to the console using colored Hungarian output.
    /// </summary>
    /// <param name="errors">The translated and enriched errors to render.</param>
    /// <param name="unmatchedLines">The GCC output lines that did not match the parser pattern.</param>
    /// <param name="rawMode">When <see langword="true"/>, all formatting is skipped and <paramref name="rawStderr"/> is printed directly.</param>
    /// <param name="rawStderr">The original GCC stderr output.</param>
    public void WriteSummary(IReadOnlyList<ParsedError> errors, IReadOnlyList<string> unmatchedLines, bool rawMode, string rawStderr)
    {
        try
        {
            if (rawMode)
            {
                Console.Write(rawStderr);
                return;
            }

            var errorCount = 0;
            var warningCount = 0;

            for (var i = 0; i < errors.Count; i++)
            {
                var error = errors[i];
                var isWarning = error.Type == ErrorType.Warning;
                if (isWarning)
                {
                    warningCount++;
                }
                else
                {
                    errorCount++;
                }

                WriteHeader(error);
                Console.WriteLine();
                WriteLocation(error);
                Console.WriteLine();
                WriteCodeSnippet(error);
                WriteExplanation(error);
                Console.WriteLine();
                WriteSuggestion(error);

                if (i < errors.Count - 1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine();
                    Console.WriteLine("────────────────");
                    Console.WriteLine();
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine($"Összesen {errorCount} hiba és {warningCount} figyelmeztetés.");

            if (unmatchedLines.Count > 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                var infoPrefix = UseAsciiFallback ? "[Info]" : "ℹ️";
                Console.WriteLine($"{infoPrefix} Egyéb GCC üzenetek:");

                foreach (var line in unmatchedLines)
                {
                    Console.WriteLine(line);
                }
            }
        }
        finally
        {
            Console.ResetColor();
        }
    }

    private static void WriteHeader(ParsedError error)
    {
        var isWarning = error.Type == ErrorType.Warning;
        var label = isWarning
            ? (UseAsciiFallback ? "[FIGYELMEZTETÉS]" : "⚠️ FIGYELMEZTETÉS")
            : (UseAsciiFallback ? "[HIBA]" : "❌ HIBA");
        var title = error.Definition?.FriendlyTitle;
        if (string.IsNullOrWhiteSpace(title))
        {
            title = "Ismeretlen fordítási hiba";
        }

        Console.ForegroundColor = isWarning ? ConsoleColor.Yellow : ConsoleColor.Red;
        Console.WriteLine($"{label}: {title}");
    }

    private static void WriteLocation(ParsedError error)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        var prefix = UseAsciiFallback ? "[Helyszín]" : "📍 Helyszín";
        Console.WriteLine($"{prefix}: {error.FileName} ({error.LineNumber}. sor, {error.ColumnNumber}. oszlop)");
    }

    private static void WriteCodeSnippet(ParsedError error)
    {
        if (error.SourceLine is null)
        {
            return;
        }

        Console.ForegroundColor = ConsoleColor.White;
        var prefix = UseAsciiFallback ? "[Kód]" : "💻 Érintett kód";
        Console.WriteLine($"{prefix}:");

        if (!string.IsNullOrWhiteSpace(error.PreviousLine) && error.LineNumber > 1)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"   {error.LineNumber - 1} | {error.PreviousLine}");
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($">  {error.LineNumber} | {error.SourceLine}");

        if (!string.IsNullOrWhiteSpace(error.NextLine))
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"   {error.LineNumber + 1} | {error.NextLine}");
        }

        Console.WriteLine();
    }

    private static void WriteExplanation(ParsedError error)
    {
        var explanation = error.Definition?.Explanation;
        if (string.IsNullOrWhiteSpace(explanation))
        {
            explanation = "A GCC egy olyan hibát jelzett, amelyhez még nincs magyarázat.";
        }

        Console.ForegroundColor = ConsoleColor.White;
        var prefix = UseAsciiFallback ? "[Magyarázat]" : "💡 Magyarázat";
        Console.WriteLine($"{prefix}:");
        Console.WriteLine(explanation);
    }

    private static void WriteSuggestion(ParsedError error)
    {
        var suggestion = error.Definition?.Suggestion;
        if (string.IsNullOrWhiteSpace(suggestion))
        {
            suggestion = "Ellenőrizd a GCC eredeti üzenetét és keress rá online.";
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        var prefix = UseAsciiFallback ? "[Javaslat]" : "🔧 Javaslat";
        Console.WriteLine($"{prefix}:");
        Console.WriteLine(suggestion);
    }

    private static bool SupportsEmojiOutput()
    {
        var encoding = Console.OutputEncoding;
        if (encoding.CodePage != 65001)
        {
            return false;
        }

        return RoundTrips(encoding, "❌")
            && RoundTrips(encoding, "⚠️")
            && RoundTrips(encoding, "📍")
            && RoundTrips(encoding, "💻")
            && RoundTrips(encoding, "💡")
            && RoundTrips(encoding, "🔧")
            && RoundTrips(encoding, "ℹ️");
    }

    private static bool RoundTrips(System.Text.Encoding encoding, string value)
    {
        var bytes = encoding.GetBytes(value);
        var decoded = encoding.GetString(bytes);
        return string.Equals(decoded, value, StringComparison.Ordinal);
    }
}
