namespace Gex.Core;

public ref struct TokenEnumerator
{
    private readonly ReadOnlySpan<string> _args;
    private int _argIndex;
    private ReadOnlySpan<char> _currentArgSpan;
    private int _charIndex;
    private bool _inCombinedShortFlags;
    private ReadOnlySpan<char> _pendingValueSpan;
    private int _pendingValueArgIndex;
    private Token _current;

    public Token Current => _current;

    public TokenEnumerator(ReadOnlySpan<string> args)
    {
        _args = args;
        _argIndex = 0;
        _currentArgSpan = default;
        _charIndex = 0;
        _inCombinedShortFlags = false;
        _pendingValueSpan = default;
        _pendingValueArgIndex = -1;
        _current = default;
    }

    public bool MoveNext()
    {
        if (!_pendingValueSpan.IsEmpty)
        {
            _current = new Token(TokenKind.Value, _pendingValueArgIndex, _pendingValueSpan);
            _pendingValueSpan = default;
            _pendingValueArgIndex = -1;
            return true;
        }
        if (_inCombinedShortFlags && _charIndex < _currentArgSpan.Length)
        {
            var span = _currentArgSpan.Slice(_charIndex, 1);
            _charIndex++;
            _current = new Token(TokenKind.ShortFlag, _argIndex - 1, span);
            if (_charIndex >= _currentArgSpan.Length) _inCombinedShortFlags = false;
            return true;
        }
        _inCombinedShortFlags = false;
        while (_argIndex < _args.Length)
        {
            var s = _args[_argIndex].AsSpan();
            var idx = _argIndex++;
            if (s.Length == 0) continue;
            if (s.Length == 2 && s[0] == '-' && s[1] == '-')
            {
                _current = new Token(TokenKind.EndOfOptions, idx, s);
                return true;
            }
            if (s[0] == '-' && s.Length > 1 && s[1] == '-')
            {
                var nameAndMaybeValue = s.Slice(2);
                var eq = nameAndMaybeValue.IndexOf('=');
                if (eq >= 0)
                {
                    var name = nameAndMaybeValue.Slice(0, eq);
                    var val = nameAndMaybeValue.Slice(eq + 1);
                    _pendingValueSpan = val;
                    _pendingValueArgIndex = idx;
                    _current = new Token(TokenKind.LongOption, idx, name);
                    return true;
                }
                _current = new Token(TokenKind.LongOption, idx, nameAndMaybeValue);
                return true;
            }
            if (s[0] == '-' && s.Length > 1)
            {
                var tail = s.Slice(1);
                if (tail.Length > 1)
                {
                    _currentArgSpan = tail;
                    _charIndex = 0;
                    _inCombinedShortFlags = true;
                    var first = _currentArgSpan.Slice(_charIndex, 1);
                    _charIndex++;
                    _current = new Token(TokenKind.ShortFlag, idx, first);
                    if (_charIndex >= _currentArgSpan.Length) _inCombinedShortFlags = false;
                    return true;
                }
                _current = new Token(TokenKind.ShortFlag, idx, tail);
                return true;
            }
            if ((s[0] == '"' || s[0] == '\'') && s.Length >= 2 && s[s.Length - 1] == s[0])
            {
                _current = new Token(TokenKind.Value, idx, s.Slice(1, s.Length - 2));
                return true;
            }
            _current = new Token(TokenKind.Positional, idx, s);
            return true;
        }
        return false;
    }
}
