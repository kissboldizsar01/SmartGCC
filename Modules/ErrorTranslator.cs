using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using SmartGCC.Models;

namespace SmartGCC.Modules;

public sealed class ErrorTranslator
{
    private static readonly ErrorDefinition FallbackDefinition = new()
    {
        Id = "unknown-gcc-error",
        MatchPattern = ".*",
        FriendlyTitle = "Ismeretlen fordítási hiba",
        Explanation = "A GCC egy olyan hibát jelzett, amelyhez még nincs magyarázat.",
        Suggestion = "Ellenőrizd a GCC eredeti üzenetét és keress rá online.",
        AppliesTo = "both"
    };

    private readonly IReadOnlyList<CompiledErrorDefinition> _definitions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorTranslator"/> class.
    /// </summary>
    public ErrorTranslator()
    {
        _definitions = LoadDefinitions();
    }

    /// <summary>
    /// Translates parsed errors using embedded error definitions.
    /// </summary>
    /// <param name="errors">The parsed errors.</param>
    /// <returns>An enriched list of parsed errors containing matched definitions.</returns>
    public List<ParsedError> Enrich(IReadOnlyList<ParsedError> errors)
    {
        return Translate(errors);
    }

    /// <summary>
    /// Translates parsed errors using embedded error definitions.
    /// </summary>
    /// <param name="errors">The parsed errors.</param>
    /// <returns>An enriched list of parsed errors containing matched definitions.</returns>
    public List<ParsedError> Translate(IReadOnlyList<ParsedError> errors)
    {
        var translated = new List<ParsedError>(errors.Count);

        foreach (var error in errors)
        {
            var definition = MatchDefinition(error);

            var copy = new ParsedError
            {
                FileName = error.FileName,
                LineNumber = error.LineNumber,
                ColumnNumber = error.ColumnNumber,
                Type = error.Type,
                RawMessage = error.RawMessage,
                SourceLine = error.SourceLine,
                PreviousLine = error.PreviousLine,
                NextLine = error.NextLine,
                ErrorCode = definition.Id,
                Message = string.IsNullOrWhiteSpace(error.RawMessage) ? error.Message : error.RawMessage,
                FilePath = error.FilePath,
                Translation = definition.Explanation,
                Definition = definition
            };

            translated.Add(copy);
        }

        return translated;
    }

    private ErrorDefinition MatchDefinition(ParsedError error)
    {
        var rawMessage = error.RawMessage ?? string.Empty;
        var normalizedRawMessage = NormalizeForMatching(rawMessage);

        foreach (var definition in _definitions)
        {
            if (!AppliesTo(definition.Definition.AppliesTo, error.Type))
            {
                continue;
            }

            if (definition.Pattern.IsMatch(rawMessage) || definition.Pattern.IsMatch(normalizedRawMessage))
            {
                return definition.Definition;
            }
        }

        return FallbackDefinition;
    }

    private static bool AppliesTo(string appliesTo, ErrorType type)
    {
        var normalizedAppliesTo = appliesTo.Trim();

        if (string.Equals(normalizedAppliesTo, "both", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return type switch
        {
            ErrorType.Warning => string.Equals(normalizedAppliesTo, "warning", StringComparison.OrdinalIgnoreCase),
            _ => string.Equals(normalizedAppliesTo, "error", StringComparison.OrdinalIgnoreCase)
        };
    }

    private static IReadOnlyList<CompiledErrorDefinition> LoadDefinitions()
    {
        const string resourceName = "SmartGCC.Resources.errors.json";
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            return [];
        }

        var root = JsonSerializer.Deserialize(stream, SmartGccJsonContext.Default.ErrorDefinitionRoot);
        var definitions = root?.Errors ?? [];

        var compiled = new List<CompiledErrorDefinition>(definitions.Count);
        foreach (var definition in definitions)
        {
            if (string.IsNullOrWhiteSpace(definition.MatchPattern))
            {
                continue;
            }

            try
            {
                var pattern = new Regex(definition.MatchPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                compiled.Add(new CompiledErrorDefinition(definition, pattern));
            }
            catch (ArgumentException)
            {
            }
        }

        return compiled;
    }

    private static string NormalizeForMatching(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return string.Empty;
        }

        return message
            .Replace('‘', '\'')
            .Replace('’', '\'')
            .Replace('`', '\'')
            .Replace('“', '"')
            .Replace('”', '"');
    }

    private sealed record CompiledErrorDefinition(ErrorDefinition Definition, Regex Pattern);
}
