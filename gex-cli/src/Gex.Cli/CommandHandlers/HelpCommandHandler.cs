using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gex.Cli;

/// <summary>
/// Command handler for displaying help information.
/// </summary>
public sealed class HelpCommandHandler : ICommandHandler
{
    private readonly ILogger<HelpCommandHandler> _logger;
    private readonly IHelpProvider _helpProvider;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="helpProvider">The help provider.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public HelpCommandHandler(
        ILogger<HelpCommandHandler> logger,
        IHelpProvider helpProvider,
        IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _helpProvider = helpProvider ?? throw new ArgumentNullException(nameof(helpProvider));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public string CommandName => "help";

    /// <inheritdoc/>
    public string Description => "Displays help information";

    /// <inheritdoc/>
    public Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var handlers = _serviceProvider.GetServices<ICommandHandler>();

        if (args.Length == 0)
        {
            _helpProvider.ShowHelp(handlers);
            return Task.FromResult(0);
        }

        var commandName = args[0].ToLowerInvariant();
        var handler = handlers.FirstOrDefault(h => h.CommandName.Equals(commandName, StringComparison.OrdinalIgnoreCase));

        if (handler == null)
        {
            _logger.LogError("Unknown command: {Command}", commandName);
            _helpProvider.ShowHelp(handlers);
            return Task.FromResult(1);
        }

        _helpProvider.ShowCommandHelp(handler);
        return Task.FromResult(0);
    }
}
