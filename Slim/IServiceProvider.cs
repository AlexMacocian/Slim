using Slim.Exceptions;
using System;

namespace Slim
{
    /// <summary>
    /// Interface allowing to request services from the <see cref="ServiceManager"/>.
    /// </summary>
    public interface IServiceProvider
    {
        /// <summary>
        /// Resolves and returns the required service.
        /// </summary>
        /// <typeparam name="TInterface">Type of required service.</typeparam>
        /// <returns>Required service.</returns>
        /// <exception cref="DependencyInjectionException">Thrown when unable to resolve required service.</exception>
        TInterface GetService<TInterface>() where TInterface : class;
        /// <summary>
        /// Resolves and returns the required service.
        /// </summary>
        /// <param name="type">Type of required service.</param>
        /// <returns>Required service.</returns>
        /// <exception cref="DependencyInjectionException">Thrown when unable to resolve required service.</exception>
        object GetService(Type type);
    }
}
