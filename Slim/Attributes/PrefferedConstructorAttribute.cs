using System;

namespace Slim.Attributes;

/// <summary>
/// Attribute used to mark constructors to be used with priority by the <see cref="ServiceManager"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor)]
public sealed class PreferredConstructorAttribute : Attribute
{
    /// <summary>
    /// Priority of the constructor. Constructors are ordered using this value when present.
    /// </summary>
    public int Priority { get; set; } = int.MaxValue - 1;

    /// <summary>
    /// Creates a new instance of <see cref="PreferredConstructorAttribute"/>.
    /// </summary>
    public PreferredConstructorAttribute()
    {
    }
}
