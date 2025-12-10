namespace Gex.Cli;

/// <summary>
/// Represents a command handler.
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Gets the name of the command this handler supports.
    /// </summary>
    string CommandName { get; }

    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes the command with the specified arguments.
    /// </summary>
    /// <param name="args">The command arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The exit code.</returns>
    Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default);
}
