# Gex CLI

A modern, high-performance CLI toolkit for argument parsing, binding, and diagnostics. Modular, extensible, and benchmarked for speed and allocations.

## Structure

- `src/Gex.Core`: Core domain logic, tokenizer, parser, binder abstractions
- `src/Gex.Cli`: CLI entry point, command dispatch, interactive UX
- `src/Gex.Generators`: Roslyn source generators for static binders
- `tests/Gex.Core.Tests`: Unit tests for core logic
- `tests/Gex.Cli.Tests`: CLI integration tests
- `benchmarks/Gex.Benchmarks`: BenchmarkDotNet suite
- `.github/workflows/ci.yml`: CI for build/test/benchmark

## Build & Test

- Build: `dotnet build`
- Test: `dotnet test`
- Benchmark: `dotnet run -c Release --project benchmarks/Gex.Benchmarks`
- Switch to net10.0: uncomment in `Directory.Build.props`

## Extending

- Add commands in `Gex.Core`, annotate for source generator, implement handlers.
- Diagnostics: implement helpful error messages for parser/binder.
- Shell support: cover escaping, quoting, Unix/Windows differences in tests.

## Contribution

See architecture notes in this README. PRs require passing CI (build, test, benchmark).
