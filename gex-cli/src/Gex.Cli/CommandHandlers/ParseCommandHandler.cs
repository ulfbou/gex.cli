using Gex.Core;
using Microsoft.Extensions.Logging;

namespace Gex.Cli;

/// <summary>
/// Command handler for parsing input.
/// </summary>
public sealed class ParseCommandHandler : ICommandHandler
{
    private readonly ILogger<ParseCommandHandler> _logger;
    private readonly IDiagnosticsProvider _diagnostics;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="diagnostics">The diagnostics provider.</param>
    public ParseCommandHandler(
        ILogger<ParseCommandHandler> logger,
        IDiagnosticsProvider diagnostics)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
    }

    /// <inheritdoc/>
    public string CommandName => "parse";

    /// <inheritdoc/>
    public string Description => "Parses the input expression into a syntax tree";

    /// <inheritdoc/>
    public Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        if (args.Length == 0)
        {
            _logger.LogError("No input provided");
            Console.WriteLine("Usage: gex parse <expression>");
            return Task.FromResult(1);
        }

        var input = string.Join(" ", args);
        _logger.LogInformation("Parsing: {Input}", input);

        var tokenizer = new Tokenizer(input);
        var tokens = tokenizer.Tokenize().ToList();

        var parser = new Parser(tokens);
        var expression = parser.Parse();

        if (parser.HasErrors)
        {
            _diagnostics.DisplayParseErrors(parser.Errors);
            return Task.FromResult(1);
        }

        if (expression != null)
        {
            Console.WriteLine("Parse successful!");
            Console.WriteLine($"Expression type: {expression.GetType().Name}");
            Console.WriteLine($"Expression: {expression}");
        }
        else
        {
            Console.WriteLine("Parse failed: no expression produced");
            return Task.FromResult(1);
        }

        return Task.FromResult(0);
    }
}
