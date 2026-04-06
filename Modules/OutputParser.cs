using SmartGCC.Models;
using System.Text.RegularExpressions;

namespace SmartGCC.Modules;

public sealed class OutputParser
{
    private static readonly Regex GccLinePattern = new(
        "^(?<file>.+?):(?<line>\\d+):(?<col>\\d+):\\s+(?<type>error|warning|fatal error):\\s+(?<message>.*)$",
        RegexOptions.Compiled);

    private static readonly Regex WarningSuffixPattern = new(
        "\\s+\\[-W(?:error=)?[^\\]]+\\]\\s*$",
        RegexOptions.Compiled);

    private static readonly Regex Cc1LinePattern = new(
        "^cc1(?:\\.exe)?\\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Parses GCC stderr output into structured error records.
    /// </summary>
    /// <param name="stderr">The raw GCC standard error output.</param>
    /// <returns>
    /// A parser result containing matched errors and unmatched lines.
    /// If input is empty or no lines match the GCC pattern, an empty result is returned.
    /// </returns>
    public ParserResult Parse(string stderr)
    {
        if (string.IsNullOrWhiteSpace(stderr))
        {
            return new ParserResult();
        }

        var result = new ParserResult();
        using var reader = new StringReader(stderr);
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            if (IsCc1Line(line))
            {
                result.UnmatchedLines.Add(line);
                continue;
            }

            var match = GccLinePattern.Match(line);
            if (!match.Success)
            {
                result.UnmatchedLines.Add(line);
                continue;
            }

            var cleanedMessage = StripWarningSuffix(match.Groups["message"].Value);

            var parsed = new ParsedError
            {
                FileName = match.Groups["file"].Value,
                FilePath = match.Groups["file"].Value,
                LineNumber = ParsePositiveInt(match.Groups["line"].Value),
                ColumnNumber = ParsePositiveInt(match.Groups["col"].Value),
                Type = ParseErrorType(match.Groups["type"].Value),
                RawMessage = cleanedMessage,
                Message = cleanedMessage
            };

            result.Errors.Add(parsed);
        }

        if (result.Errors.Count > 0)
        {
            PopulateSourceContext(result.Errors);
        }

        return result;
    }

    private static void PopulateSourceContext(IReadOnlyList<ParsedError> errors)
    {
        var fileCache = new Dictionary<string, string[]?>(StringComparer.OrdinalIgnoreCase);

        foreach (var fileName in errors.Select(e => e.FileName).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                continue;
            }

            try
            {
                fileCache[fileName] = File.Exists(fileName) ? File.ReadAllLines(fileName) : null;
            }
            catch
            {
                fileCache[fileName] = null;
            }
        }

        foreach (var error in errors)
        {
            if (!fileCache.TryGetValue(error.FileName, out var lines) || lines is null || lines.Length == 0)
            {
                continue;
            }

            var currentIndex = error.LineNumber - 1;
            if (currentIndex >= 0 && currentIndex < lines.Length)
            {
                error.SourceLine = lines[currentIndex];
            }

            var previousIndex = error.LineNumber - 2;
            if (previousIndex >= 0 && previousIndex < lines.Length)
            {
                error.PreviousLine = lines[previousIndex];
            }

            var nextIndex = error.LineNumber;
            if (nextIndex >= 0 && nextIndex < lines.Length)
            {
                error.NextLine = lines[nextIndex];
            }
        }
    }

    private static int ParsePositiveInt(string value)
    {
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : 0;
    }

    private static ErrorType ParseErrorType(string value)
    {
        return value switch
        {
            "warning" => ErrorType.Warning,
            "fatal error" => ErrorType.FatalError,
            _ => ErrorType.Error
        };
    }

    private static bool IsCc1Line(string line)
    {
        return Cc1LinePattern.IsMatch(line);
    }

    private static string StripWarningSuffix(string message)
    {
        return WarningSuffixPattern.Replace(message, string.Empty).TrimEnd();
    }
}
