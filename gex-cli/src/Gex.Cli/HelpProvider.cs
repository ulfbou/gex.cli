namespace Gex.Cli;

/// <summary>
/// Default implementation of the help provider.
/// </summary>
public sealed class HelpProvider : IHelpProvider
{
    /// <inheritdoc/>
    public void ShowHelp(IEnumerable<ICommandHandler> handlers)
    {
        Console.WriteLine("Gex CLI - A command-line interface for expression parsing and binding");
        Console.WriteLine();
        Console.WriteLine("Usage: gex <command> [options]");
        Console.WriteLine();
        Console.WriteLine("Available commands:");
        Console.WriteLine();

        foreach (var handler in handlers.OrderBy(h => h.CommandName))
        {
            Console.WriteLine($"  {handler.CommandName,-15} {handler.Description}");
        }

        Console.WriteLine();
        Console.WriteLine("Use 'gex help <command>' for more information about a command.");
    }

    /// <inheritdoc/>
    public void ShowCommandHelp(ICommandHandler handler)
    {
        Console.WriteLine($"Command: {handler.CommandName}");
        Console.WriteLine();
        Console.WriteLine($"Description: {handler.Description}");
        Console.WriteLine();
    }
}
