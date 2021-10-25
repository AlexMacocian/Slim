using Slim.Resolvers;
using System;
using System.Collections.Generic;

namespace Slim
{
    /// <summary>
    /// Interface for <see cref="ServiceManager"/>.
    /// </summary>
    public interface IServiceManager : IServiceProvider, IServiceProducer
    {
        /// <summary>
        /// Allow modifications to scoped <see cref="IServiceManager"/> created from this <see cref="IServiceManager"/>.
        /// </summary>
        bool AllowScopedManagerModifications { get; set; }
        /// <summary>
        /// Returns true if <see cref="IServiceManager"/> is readonly.
        /// </summary>
        bool IsReadOnly { get; }
        /// <summary>
        /// Returns true if there exists a registration for <paramref name="tInterface"/>.
        /// </summary>
        /// <param name="tInterface">Type of registered service.</param>
        /// <returns>True if service <paramref name="tInterface"/> is registered. Otherwise returns false.</returns>
        bool IsRegistered(Type tInterface);
        /// <summary>
        /// Returns true if there exists a registration for <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of registered service.</typeparam>
        /// <returns>True if service <typeparamref name="T"/> is registered. Otherwise returns false.</returns>
        bool IsRegistered<T>();
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
        /// Register the current service manager as a valid dependency.
        /// </summary>
        /// <remarks>Registers current service manager as <see cref="ServiceManager"/>, <see cref="IServiceManager"/>, <see cref="IServiceProducer"/>, <see cref="IServiceProvider"/>.</remarks>
        void RegisterServiceManager();
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
        /// <summary>
        /// Register a resolver that manually resolves dependencies.
        /// </summary>
        /// <param name="dependencyResolver">Resolver that manually creates a dependency.</param>
        void RegisterResolver(IDependencyResolver dependencyResolver);
    }
}
