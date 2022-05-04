using System;

namespace Slim.Attributes
{
    /// <summary>
    /// Attribute used to mark constructors to be used with priority by the <see cref="ServiceManager"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class PrefferedConstructorAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="PrefferedConstructorAttribute"/>.
        /// </summary>
        public PrefferedConstructorAttribute()
        {
        }
    }
}
