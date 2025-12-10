using Gex.Core;
using Microsoft.Extensions.Logging;

namespace Gex.Cli;

/// <summary>
/// Command handler for tokenizing input.
/// </summary>
public sealed class TokenizeCommandHandler : ICommandHandler
{
    private readonly ILogger<TokenizeCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizeCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public TokenizeCommandHandler(ILogger<TokenizeCommandHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public string CommandName => "tokenize";

    /// <inheritdoc/>
    public string Description => "Tokenizes the input expression";

    /// <inheritdoc/>
    public Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        if (args.Length == 0)
        {
            _logger.LogError("No input provided");
            Console.WriteLine("Usage: gex tokenize <expression>");
            return Task.FromResult(1);
        }

        var input = string.Join(" ", args);
        _logger.LogInformation("Tokenizing: {Input}", input);

        var tokenizer = new Tokenizer(input);
        var tokens = tokenizer.Tokenize().ToList();

        Console.WriteLine($"Tokens ({tokens.Count}):");
        foreach (var token in tokens)
        {
            Console.WriteLine($"  {token.Kind,-20} '{token.Text}' at ({token.Line}:{token.Column})");
        }

        return Task.FromResult(0);
    }
}
