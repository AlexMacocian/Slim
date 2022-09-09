using System;

namespace Slim.Attributes;

/// <summary>
/// Attribute used to mark constructors to not be used by the <see cref="ServiceManager"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor)]
public sealed class DoNotInjectAttribute : Attribute
{
}
