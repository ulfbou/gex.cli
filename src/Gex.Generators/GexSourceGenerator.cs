
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Gex.Generators;

// The following class should be implemented in a project targeting netstandard2.0
// and the .csproj should include <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
[Generator]
public class GexSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        // Example: emit a static binder stub
        var source = "namespace Gex.Core { public static class GeneratedBinder { } }";
        context.AddSource("GeneratedBinder.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
