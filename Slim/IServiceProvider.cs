using System;
using System.Collections.Generic;
using Slim.Exceptions;

namespace Slim;

/// <summary>
/// Interface allowing to request services from the <see cref="ServiceManager"/>.
/// </summary>
public interface IServiceProvider : System.IServiceProvider, IDisposable
{
    /// <summary>
    /// Returns all services that can be cast to provided type.
    /// </summary>
    /// <typeparam name="T">Type of the returned services.</typeparam>
    IEnumerable<T> GetServicesOfType<T>();
    /// <summary>
    /// Returns all services that can be cast to provided type.
    /// </summary>
    /// <param name="type">Type of the returned services.</param>
    IEnumerable<object> GetServicesOfType(Type type);
    /// <summary>
    /// Creates a scoped <see cref="IServiceProvider"/>.
    /// </summary>
    /// <returns>Scoped <see cref="IServiceProvider"/>.</returns>
    IServiceProvider CreateScope();
    /// <summary>
    /// Resolves and returns the required service.
    /// </summary>
    /// <typeparam name="TInterface">Type of required service.</typeparam>
    /// <returns>Required service.</returns>
    /// <exception cref="DependencyInjectionException">Thrown when unable to resolve required service.</exception>
    TInterface? GetService<TInterface>() where TInterface : class;
}
