using Gex.Core;
using Microsoft.Extensions.Logging;

namespace Gex.Cli;

/// <summary>
/// Command handler for binding expressions.
/// </summary>
public sealed class BindCommandHandler : ICommandHandler
{
    private readonly ILogger<BindCommandHandler> _logger;
    private readonly IDiagnosticsProvider _diagnostics;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="diagnostics">The diagnostics provider.</param>
    public BindCommandHandler(
        ILogger<BindCommandHandler> logger,
        IDiagnosticsProvider diagnostics)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
    }

    /// <inheritdoc/>
    public string CommandName => "bind";

    /// <inheritdoc/>
    public string Description => "Binds the input expression with type information";

    /// <inheritdoc/>
    public Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        if (args.Length == 0)
        {
            _logger.LogError("No input provided");
            Console.WriteLine("Usage: gex bind <expression>");
            return Task.FromResult(1);
        }

        var input = string.Join(" ", args);
        _logger.LogInformation("Binding: {Input}", input);

        var tokenizer = new Tokenizer(input);
        var tokens = tokenizer.Tokenize().ToList();

        var parser = new Parser(tokens);
        var expression = parser.Parse();

        if (parser.HasErrors)
        {
            _diagnostics.DisplayParseErrors(parser.Errors);
            return Task.FromResult(1);
        }

        var binder = new Binder();
        var boundExpression = binder.BindExpression(expression);

        if (binder.HasErrors)
        {
            _diagnostics.DisplayBindingErrors(binder.Errors);
            return Task.FromResult(1);
        }

        if (boundExpression != null)
        {
            Console.WriteLine("Binding successful!");
            Console.WriteLine($"Bound expression type: {boundExpression.GetType().Name}");
            Console.WriteLine($"Result type: {boundExpression.Type.Name}");
            Console.WriteLine($"Bound expression: {boundExpression}");
        }
        else
        {
            Console.WriteLine("Binding failed: no expression produced");
            return Task.FromResult(1);
        }

        return Task.FromResult(0);
    }
}
