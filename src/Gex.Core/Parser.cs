using System;
using System.Collections.Generic;
using System.Linq;

namespace Gex.Core;

public sealed class ParseResult
{
    private readonly Dictionary<string, List<ReadOnlyMemory<char>>> _options = new(StringComparer.Ordinal);
    private readonly List<ReadOnlyMemory<char>> _positionals = new();
    private readonly List<string> _errors = new();

    public IReadOnlyDictionary<string, List<ReadOnlyMemory<char>>> Options => _options;
    public IReadOnlyList<ReadOnlyMemory<char>> Positionals => _positionals;
    public IReadOnlyList<string> Errors => _errors;

    internal void AddOption(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
    {
        var key = name.ToString();
        if (!_options.TryGetValue(key, out var list))
        {
            list = new List<ReadOnlyMemory<char>>();
            _options[key] = list;
        }
        list.Add(value.ToArray());
    }

    internal void AddFlag(ReadOnlySpan<char> name)
    {
        AddOption(name, ReadOnlySpan<char>.Empty);
    }

    internal void AddPositional(ReadOnlySpan<char> value)
    {
        _positionals.Add(value.ToArray());
    }

    internal void AddError(string message, int position)
    {
        _errors.Add(Diagnostics.FormatError(message, position));
    }

    public string? GetValue(string name)
    {
        if (_options.TryGetValue(name, out var list) && list.Count > 0)
            return new string(list[0].Span);
        return null;
    }
}

public static class Parser
{
    public static ParseResult Parse(string[] args)
    {
        var result = new ParseResult();
        var enumerator = Tokenizer.GetTokens(args);
        while (enumerator.MoveNext())
        {
            var t = enumerator.Current;
            switch (t.Kind)
            {
                case TokenKind.LongOption:
                    if (enumerator.MoveNext())
                    {
                        var next = enumerator.Current;
                        if (next.Kind == TokenKind.Value && next.ArgIndex == t.ArgIndex)
                            result.AddOption(t.Span, next.Span);
                        else
                        {
                            result.AddFlag(t.Span);
                            if (next.Kind == TokenKind.Positional || next.Kind == TokenKind.Value)
                                result.AddPositional(next.Span);
                            else if (next.Kind == TokenKind.ShortFlag || next.Kind == TokenKind.LongOption)
                                result.AddFlag(next.Span);
                        }
                    }
                    else
                    {
                        result.AddFlag(t.Span);
                    }
                    break;
                case TokenKind.ShortFlag:
                    result.AddFlag(t.Span);
                    break;
                case TokenKind.Value:
                    result.AddPositional(t.Span);
                    break;
                case TokenKind.Positional:
                    result.AddPositional(t.Span);
                    break;
                case TokenKind.EndOfOptions:
                    while (enumerator.MoveNext())
                    {
                        var rem = enumerator.Current;
                        if (rem.Kind == TokenKind.Positional || rem.Kind == TokenKind.Value)
                            result.AddPositional(rem.Span);
                        else if (rem.Kind == TokenKind.ShortFlag || rem.Kind == TokenKind.LongOption)
                            result.AddFlag(rem.Span);
                    }
                    break;
                default:
                    result.AddError($"Unknown token kind: {t.Kind}", t.ArgIndex);
                    break;
            }
        }
        return result;
    }
}
