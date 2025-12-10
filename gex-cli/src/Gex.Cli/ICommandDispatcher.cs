namespace Gex.Cli;

/// <summary>
/// Dispatches commands to the appropriate handlers.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Dispatches the command with the specified arguments.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The exit code.</returns>
    Task<int> DispatchAsync(string[] args, CancellationToken cancellationToken = default);
}
