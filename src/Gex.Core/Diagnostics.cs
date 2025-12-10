namespace Gex.Core;

public static class Diagnostics
{
    public static string FormatError(string message, int position)
        => $"Error at {position}: {message}";
}
