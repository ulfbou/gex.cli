using Gex.Core;

namespace Gex.Cli;

/// <summary>
/// Default implementation of the diagnostics provider.
/// </summary>
public sealed class DiagnosticsProvider : IDiagnosticsProvider
{
    /// <inheritdoc/>
    public void DisplayParseErrors(IReadOnlyList<ParseError> errors)
    {
        if (errors.Count == 0)
        {
            return;
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Parse errors ({errors.Count}):");
        Console.ResetColor();

        foreach (var error in errors)
        {
            Console.WriteLine($"  Line {error.Line}, Column {error.Column}: {error.Message}");
        }

        Console.WriteLine();
    }

    /// <inheritdoc/>
    public void DisplayBindingErrors(IReadOnlyList<BindingError> errors)
    {
        if (errors.Count == 0)
        {
            return;
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Binding errors ({errors.Count}):");
        Console.ResetColor();

        foreach (var error in errors)
        {
            Console.WriteLine($"  Line {error.Line}, Column {error.Column}: {error.Message}");
        }

        Console.WriteLine();
    }
}
