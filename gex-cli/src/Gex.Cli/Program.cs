using Gex.Cli;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configure services
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

builder.Services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddSingleton<IHelpProvider, HelpProvider>();
builder.Services.AddSingleton<IDiagnosticsProvider, DiagnosticsProvider>();

// Register command handlers
builder.Services.AddTransient<ICommandHandler, ParseCommandHandler>();
builder.Services.AddTransient<ICommandHandler, TokenizeCommandHandler>();
builder.Services.AddTransient<ICommandHandler, BindCommandHandler>();
builder.Services.AddTransient<ICommandHandler, HelpCommandHandler>();
builder.Services.AddTransient<ICommandHandler, VersionCommandHandler>();

var host = builder.Build();

// Run the application
var dispatcher = host.Services.GetRequiredService<ICommandDispatcher>();
var exitCode = await dispatcher.DispatchAsync(args);

return exitCode;
