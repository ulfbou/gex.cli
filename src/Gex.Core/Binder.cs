namespace Gex.Core;

public interface ICommandBinder
{
    object? Bind(ParseResult result);
}

public class ReflectionBinder : ICommandBinder
{
    public object? Bind(ParseResult result)
    {
        // TODO: Implement reflection-based binding
        return null;
    }
}
