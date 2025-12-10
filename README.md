# gex.cli

A command-line interface for expression parsing, binding, and code generation.

## Project Structure

The main implementation is in the `/gex-cli` directory. See [gex-cli/README.md](gex-cli/README.md) for detailed documentation.

## Quick Start

```bash
# Build the solution
dotnet build gex-cli/Gex.sln

# Run tests
dotnet test gex-cli/Gex.sln

# Run the CLI
dotnet run --project gex-cli/src/Gex.Cli/Gex.Cli.csproj -- help
```

## Features

- **Tokenizer**: Lexical analysis of expressions
- **Parser**: Syntax tree construction
- **Binder**: Semantic analysis and type checking
- **Source Generators**: Automatic binder generation for POCOs
- **CLI**: Interactive command-line interface
- **Extensible**: Add custom commands and handlers via DI

## Documentation

For complete documentation, build instructions, and usage examples, see the [gex-cli README](gex-cli/README.md).