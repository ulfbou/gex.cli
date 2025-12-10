using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Gex.Cli;

/// <summary>
/// Command handler for displaying version information.
/// </summary>
public sealed class VersionCommandHandler : ICommandHandler
{
    private readonly ILogger<VersionCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public VersionCommandHandler(ILogger<VersionCommandHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public string CommandName => "version";

    /// <inheritdoc/>
    public string Description => "Displays version information";

    /// <inheritdoc/>
    public Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? version?.ToString() ?? "Unknown";

        Console.WriteLine($"Gex CLI version {informationalVersion}");
        Console.WriteLine($"Runtime: {Environment.Version}");
        Console.WriteLine($"OS: {Environment.OSVersion}");

        return Task.FromResult(0);
    }
}
