namespace Gex.Core;

public interface ICommandModule
{
    string Name { get; }
    void Execute(ParseResult result);
}
