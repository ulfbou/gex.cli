# Gex CLI

A command-line interface for expression parsing, binding, and code generation.

## Overview

Gex CLI is a .NET-based tool that provides expression tokenization, parsing, and semantic binding capabilities. It includes source generators for creating static binders from annotated POCOs, making it extensible through custom handlers and types.

## Project Structure

```
gex-cli/
├── src/
│   ├── Gex.Core/           # Core library (tokenizer, parser, binder)
│   ├── Gex.Cli/            # CLI application (entry point, dispatch, help, diagnostics)
│   └── Gex.Generators/     # Roslyn source generators for static binders
├── tests/
│   └── Gex.Tests/          # Unit and integration tests
├── benchmarks/
│   └── Gex.Benchmarks/     # Performance benchmarks (allocations, speed)
├── props/                  # Shared build configuration
├── Directory.Build.props   # Shared MSBuild properties
├── Gex.sln                 # Solution file
└── README.md               # This file
```

## Features

### Core Library (Gex.Core)

- **Tokenizer**: Converts input text into a stream of tokens
- **Parser**: Builds abstract syntax trees from token streams
- **Binder**: Performs semantic analysis and type binding

### CLI Application (Gex.Cli)

- **Command Dispatcher**: Routes commands to appropriate handlers
- **Help System**: Provides usage information and command documentation
- **Diagnostics**: Displays parse and binding errors with line/column information
- **Extensible**: Add custom commands by implementing `ICommandHandler`

### Source Generators (Gex.Generators)

- Generates static binders for classes annotated with `[GenerateBinder]`
- Creates `Bind` and `ToDictionary` methods automatically
- Enables data binding without reflection overhead

## Target Frameworks

- **Primary**: .NET 8.0 (default)
- **Optional**: .NET 10.0 (when available)

## Configuration

The solution is configured with:

- **Nullable reference types**: Enabled for all projects
- **Dependency Injection**: Using Microsoft.Extensions.DependencyInjection
- **AOT Compilation**: Enabled for the CLI project
- **Trimming**: Enabled for reduced deployment size
- **Documentation**: XML documentation generated for all public APIs

## Building

Build the entire solution:

```bash
dotnet build gex-cli/Gex.sln
```

Build for a specific framework:

```bash
dotnet build gex-cli/Gex.sln -f net8.0
dotnet build gex-cli/Gex.sln -f net10.0
```

Build with Release configuration:

```bash
dotnet build gex-cli/Gex.sln -c Release
```

## Testing

Run all tests:

```bash
dotnet test gex-cli/Gex.sln
```

Run tests with coverage:

```bash
dotnet test gex-cli/Gex.sln --collect:"XPlat Code Coverage"
```

Run tests for a specific framework:

```bash
dotnet test gex-cli/Gex.sln -f net8.0
```

## Benchmarking

Run all benchmarks:

```bash
dotnet run --project gex-cli/benchmarks/Gex.Benchmarks/Gex.Benchmarks.csproj -c Release
```

Run specific benchmark:

```bash
dotnet run --project gex-cli/benchmarks/Gex.Benchmarks/Gex.Benchmarks.csproj -c Release -- --filter *Tokenizer*
```

## Usage

### Basic Commands

Display help:

```bash
gex help
```

Tokenize an expression:

```bash
gex tokenize "1 + 2 * 3"
```

Parse an expression:

```bash
gex parse "a + b * c"
```

Bind an expression:

```bash
gex bind "1 + 2"
```

Display version:

```bash
gex version
```

### Available Commands

- `tokenize <expression>` - Tokenizes the input expression
- `parse <expression>` - Parses the input expression into a syntax tree
- `bind <expression>` - Binds the input expression with type information
- `help [command]` - Displays help information
- `version` - Displays version information

## Extending Gex CLI

### Adding a Custom Command Handler

1. Create a class that implements `ICommandHandler`:

```csharp
public class MyCommandHandler : ICommandHandler
{
    public string CommandName => "mycommand";
    public string Description => "Does something custom";

    public Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken)
    {
        // Your implementation here
        return Task.FromResult(0);
    }
}
```

2. Register it in `Program.cs`:

```csharp
builder.Services.AddTransient<ICommandHandler, MyCommandHandler>();
```

### Using the Source Generator

1. Annotate your POCO with `[GenerateBinder]`:

```csharp
using Gex.Core;

[GenerateBinder]
public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}
```

2. The generator creates `PersonBinder` with static methods:

```csharp
var data = new Dictionary<string, object?>
{
    ["Name"] = "John",
    ["Age"] = 30
};

var person = PersonBinder.Bind(data);
var dict = PersonBinder.ToDictionary(person);
```

## CI/CD

The solution includes GitHub Actions workflows for:

- **Build**: Compiles the solution on multiple platforms
- **Test**: Runs unit and integration tests
- **Benchmark**: Runs performance benchmarks
- **Release**: Creates packages and releases

## Performance

The implementation is optimized for:

- **Low allocations**: Minimizes heap allocations during parsing
- **Fast parsing**: Efficient recursive descent parser
- **Type safety**: Strong typing throughout the pipeline
- **AOT compatibility**: Can be compiled with Native AOT

Benchmark results can be found in the `BenchmarkDotNet.Artifacts` directory after running benchmarks.

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Please ensure:

- All tests pass
- Code follows existing style conventions
- XML documentation is provided for public APIs
- Benchmarks show no significant regressions
