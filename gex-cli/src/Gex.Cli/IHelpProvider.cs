namespace Gex.Cli;

/// <summary>
/// Provides help information for commands.
/// </summary>
public interface IHelpProvider
{
    /// <summary>
    /// Shows help information for all available commands.
    /// </summary>
    /// <param name="handlers">The command handlers.</param>
    void ShowHelp(IEnumerable<ICommandHandler> handlers);

    /// <summary>
    /// Shows help information for a specific command.
    /// </summary>
    /// <param name="handler">The command handler.</param>
    void ShowCommandHelp(ICommandHandler handler);
}
