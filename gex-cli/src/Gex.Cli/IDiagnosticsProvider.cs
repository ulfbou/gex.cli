using Gex.Core;

namespace Gex.Cli;

/// <summary>
/// Provides diagnostic information for parsing and binding errors.
/// </summary>
public interface IDiagnosticsProvider
{
    /// <summary>
    /// Displays parse errors.
    /// </summary>
    /// <param name="errors">The parse errors.</param>
    void DisplayParseErrors(IReadOnlyList<ParseError> errors);

    /// <summary>
    /// Displays binding errors.
    /// </summary>
    /// <param name="errors">The binding errors.</param>
    void DisplayBindingErrors(IReadOnlyList<BindingError> errors);
}
