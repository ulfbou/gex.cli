using Microsoft.Extensions.Logging;

namespace Gex.Cli;

/// <summary>
/// Default implementation of the command dispatcher.
/// </summary>
public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IEnumerable<ICommandHandler> _handlers;
    private readonly ILogger<CommandDispatcher> _logger;
    private readonly IHelpProvider _helpProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandDispatcher"/> class.
    /// </summary>
    /// <param name="handlers">The command handlers.</param>
    /// <param name="helpProvider">The help provider.</param>
    /// <param name="logger">The logger.</param>
    public CommandDispatcher(
        IEnumerable<ICommandHandler> handlers,
        IHelpProvider helpProvider,
        ILogger<CommandDispatcher> logger)
    {
        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        _helpProvider = helpProvider ?? throw new ArgumentNullException(nameof(helpProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<int> DispatchAsync(string[] args, CancellationToken cancellationToken = default)
    {
        try
        {
            if (args.Length == 0)
            {
                _helpProvider.ShowHelp(_handlers);
                return 0;
            }

            var commandName = args[0].ToLowerInvariant();
            var handler = _handlers.FirstOrDefault(h => h.CommandName.Equals(commandName, StringComparison.OrdinalIgnoreCase));

            if (handler == null)
            {
                _logger.LogError("Unknown command: {Command}", commandName);
                _helpProvider.ShowHelp(_handlers);
                return 1;
            }

            var commandArgs = args.Skip(1).ToArray();
            return await handler.ExecuteAsync(commandArgs, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while dispatching command");
            return 1;
        }
    }
}
