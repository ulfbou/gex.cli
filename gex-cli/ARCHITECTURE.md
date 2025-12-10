# Gex CLI Architecture

This document describes the architecture and design of the Gex CLI project.

## Overview

Gex CLI is a .NET-based expression parsing and binding tool with extensibility through source generators and custom command handlers.

## Project Structure

```
gex-cli/
├── src/
│   ├── Gex.Core/           # Core library
│   ├── Gex.Cli/            # CLI application
│   └── Gex.Generators/     # Source generators
├── tests/
│   └── Gex.Tests/          # Unit & integration tests
├── benchmarks/
│   └── Gex.Benchmarks/     # Performance benchmarks
├── props/                  # Build configuration
├── Directory.Build.props   # Shared MSBuild properties
└── Gex.sln                 # Solution file
```

## Core Components

### Gex.Core

The core library provides the fundamental parsing and binding functionality:

**Tokenizer** (`Tokenizer.cs`)
- Converts input text into a stream of tokens
- Handles whitespace, comments, identifiers, numbers, strings, and operators
- Tracks line and column information for error reporting

**Parser** (`Parser.cs`)
- Builds abstract syntax trees from token streams
- Implements recursive descent parsing with operator precedence
- Supports expressions: literals, identifiers, binary ops, unary ops, calls, member access

**Binder** (`Binder.cs`)
- Performs semantic analysis and type binding
- Maintains a symbol table for variables, functions, and types
- Validates type correctness and resolves symbols
- Reports binding errors with location information

### Gex.Cli

The CLI application provides a command-line interface:

**Program.cs**
- Entry point using .NET Generic Host
- Configures dependency injection
- Registers command handlers

**Command Dispatcher** (`CommandDispatcher.cs`)
- Routes commands to appropriate handlers
- Provides error handling and logging

**Command Handlers**
- `TokenizeCommandHandler`: Tokenize expressions
- `ParseCommandHandler`: Parse expressions into syntax trees
- `BindCommandHandler`: Bind expressions with type information
- `HelpCommandHandler`: Display help information
- `VersionCommandHandler`: Display version information

**Support Services**
- `HelpProvider`: Formats and displays help text
- `DiagnosticsProvider`: Formats and displays error messages

### Gex.Generators

Source generators for compile-time code generation:

**BinderGenerator** (`BinderGenerator.cs`)
- Roslyn-based incremental source generator
- Generates static binders for classes annotated with `[GenerateBinder]`
- Creates `Bind()` and `ToDictionary()` methods
- Targets netstandard2.0 for compatibility

## Design Patterns

### Dependency Injection

The CLI uses Microsoft.Extensions.DependencyInjection for IoC:

```csharp
builder.Services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddTransient<ICommandHandler, YourCommandHandler>();
```

Command handlers receive dependencies through constructor injection.

### Command Pattern

Command handlers implement `ICommandHandler` interface:

```csharp
public interface ICommandHandler
{
    string CommandName { get; }
    string Description { get; }
    Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken);
}
```

### Visitor Pattern

The binder uses a pattern-matching approach to traverse syntax trees:

```csharp
return node switch
{
    LiteralExpression literal => BindLiteralExpression(literal),
    BinaryExpression binary => BindBinaryExpression(binary),
    // ...
};
```

### Builder Pattern

Syntax trees use records and immutable data structures:

```csharp
public sealed record BinaryExpression(ExpressionNode Left, Token Operator, ExpressionNode Right);
```

## Extensibility

### Adding Custom Commands

1. Implement `ICommandHandler`
2. Register with DI container in `Program.cs`
3. The dispatcher automatically routes commands

### Adding Custom Types

1. Define a new `TypeSymbol` in `Binder.cs`
2. Update operator binding logic
3. Add tests in `Gex.Tests`

### Using Source Generators

1. Annotate POCOs with `[GenerateBinder]`
2. Build the solution
3. Generated code appears in `obj/` directory
4. Use `YourClassBinder.Bind()` and `YourClassBinder.ToDictionary()`

## Target Frameworks

- **Core/Cli/Tests/Benchmarks**: net8.0 and net10.0 (multi-targeted)
- **Generators**: netstandard2.0 (Roslyn requirement)

## Build Configuration

`Directory.Build.props` provides:
- Nullable reference types enabled
- AOT compilation support (net8.0+)
- Trimming enabled
- XML documentation generation
- Warning as errors

## Testing Strategy

### Unit Tests

Located in `Gex.Tests/`:
- `TokenizerTests`: Token generation
- `ParserTests`: Syntax tree construction
- `BinderTests`: Type binding and error checking

Test frameworks:
- xUnit for test execution
- FluentAssertions for readable assertions
- Moq for mocking (if needed)

### Benchmarks

Located in `Gex.Benchmarks/`:
- `TokenizerBenchmarks`: Tokenization performance
- `ParserBenchmarks`: Parsing performance
- `BinderBenchmarks`: Binding performance

Uses BenchmarkDotNet with memory diagnostics enabled.

## CI/CD

GitHub Actions workflows:

**build.yml**
- Builds on multiple platforms (Ubuntu, Windows, macOS)
- Tests on multiple frameworks

**test.yml**
- Runs tests with coverage collection
- Uploads coverage to Codecov

**benchmark.yml**
- Runs performance benchmarks
- Uploads results as artifacts

## Performance Considerations

1. **Tokenizer**: Single-pass lexing, minimal allocations
2. **Parser**: Recursive descent with operator precedence
3. **Binder**: Symbol table lookup, type checking
4. **AOT**: Enabled for CLI to reduce startup time
5. **Trimming**: Removes unused code from published apps

## Error Handling

All components provide detailed error information:
- Position, line, and column tracking
- Descriptive error messages
- Diagnostic display with color coding

## Future Enhancements

Potential areas for extension:
- Statement support (assignments, declarations)
- Control flow (if, while, for)
- More built-in types and functions
- REPL mode
- Language server protocol (LSP) support
- Debugger integration
