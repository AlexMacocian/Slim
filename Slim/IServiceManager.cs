using System;
using System.Collections.Generic;

namespace Slim;

/// <summary>
/// Interface for <see cref="ServiceManager"/>.
/// </summary>
public interface IServiceManager : IServiceProvider, IServiceProducer
{
    /// <summary>
    /// Gets the <see cref="IServiceManager"/> that created the current <see cref="IServiceManager"/>. Returns null in case there is no parent.
    /// </summary>
    IServiceManager ParentServiceManager { get; }
    /// <summary>
    /// Allow modifications to scoped <see cref="IServiceManager"/> created from this <see cref="IServiceManager"/>.
    /// </summary>
    bool AllowScopedManagerModifications { get; set; }
    /// <summary>
    /// Returns true if <see cref="IServiceManager"/> is readonly.
    /// </summary>
    bool IsReadOnly { get; }
    /// <summary>
    /// Builds all registered singletons.
    /// </summary>
    void BuildSingletons();
    /// <summary>
    /// Marks a type of exception to be caught and handled.
    /// </summary>
    /// <typeparam name="T">Type of exception to catch.</typeparam>
    /// <param name="handle">Handler of the exception. Handler returns true if the exception should be thrown again.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    void HandleException<T>(Func<IServiceProvider, T, bool> handle)
        where T : Exception;
    /// <summary>
    /// Clears all registered types, singletons, factories and exception handlers.
    /// </summary>
    /// <remarks>
    /// Calls <see cref="IDisposable.Dispose"/> on all <see cref="IDisposable"/> singletons.
    /// </remarks>
    void Clear();
}
