using System;

namespace Gex.Core;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CliBindAttribute : Attribute
{
    public CliBindAttribute() { }
}
