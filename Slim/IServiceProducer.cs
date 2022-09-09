using Slim.Resolvers;
using System;

namespace Slim;

/// <summary>
/// Interface allowing to produce services for the <see cref="ServiceManager"/>.
/// </summary>
public interface IServiceProducer
{
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
    /// Register a resolver that manually resolves dependencies.
    /// </summary>
    /// <param name="dependencyResolver">Resolver that manually creates a dependency.</param>
    void RegisterResolver(IDependencyResolver dependencyResolver);
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterSingleton<TClass>(bool registerAllInterfaces = false)
        where TClass : class;
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="serviceFactory">Factory for the implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterSingleton<TClass>(Func<IServiceProvider, TClass> serviceFactory, bool registerAllInterfaces = false)
        where TClass : class;
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterTransient<TClass>(bool registerAllInterfaces = false)
        where TClass : class;
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <param name="serviceFactory">Factory for the implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterTransient<TClass>(Func<IServiceProvider, TClass> serviceFactory, bool registerAllInterfaces = false)
        where TClass : class;
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterScoped<TClass>(bool registerAllInterfaces = false)
        where TClass : class;
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <param name="serviceFactory">Factory for the implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterScoped<TClass>(Func<IServiceProvider, TClass> serviceFactory, bool registerAllInterfaces = false)
        where TClass : class;
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterTransient<TInterface, TClass>()
        where TInterface : class
        where TClass : TInterface;
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <param name="serviceFactory">Factory for the implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterTransient<TInterface, TClass>(Func<IServiceProvider, TClass> serviceFactory)
        where TInterface : class
        where TClass : TInterface;
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterSingleton<TInterface, TClass>()
        where TInterface : class
        where TClass : TInterface;
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterSingleton<TInterface, TClass>(Func<IServiceProvider, TClass> serviceFactory)
        where TInterface : class
        where TClass : TInterface;
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterScoped<TInterface, TClass>()
        where TInterface : class
        where TClass : TInterface;
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <param name="serviceFactory">Factory for the implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterScoped<TInterface, TClass>(Func<IServiceProvider, TClass> serviceFactory)
        where TInterface : class
        where TClass : TInterface;
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// </summary>
    /// <param name="tInterface">Type of interface.</param>
    /// <param name="tClass">Type of implementation.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterTransient(Type tInterface, Type tClass);
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// </summary>
    /// <param name="tInterface">Type of interface.</param>
    /// <param name="tClass">Type of implementation.</param>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterTransient(Type tInterface, Type tClass, Func<IServiceProvider, object> serviceFactory);
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// </summary>
    /// <param name="tInterface">Type of interface.</param>
    /// <param name="tClass">Type of implementation.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterSingleton(Type tInterface, Type tClass);
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// </summary>
    /// <param name="tInterface">Type of interface.</param>
    /// <param name="tClass">Type of implementation.</param>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterSingleton(Type tInterface, Type tClass, Func<IServiceProvider, object> serviceFactory);
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// </summary>
    /// <param name="tInterface">Type of interface.</param>
    /// <param name="tClass">Type of implementation.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterScoped(Type tInterface, Type tClass);
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// </summary>
    /// <param name="tInterface">Type of interface.</param>
    /// <param name="tClass">Type of implementation.</param>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterScoped(Type tInterface, Type tClass, Func<IServiceProvider, object> serviceFactory);
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="tClass">Type of implementation.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterTransient(Type tClass, bool registerAllInterfaces = false);
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="tClass">Type of implementation.</param>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterTransient(Type tClass, Func<IServiceProvider, object> serviceFactory, bool registerAllInterfaces = false);
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="tClass">Type of implementation.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterSingleton(Type tClass, bool registerAllInterfaces = false);
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="tClass">Type of implementation.</param>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterSingleton(Type tClass, Func<IServiceProvider, object> serviceFactory, bool registerAllInterfaces = false);
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="tClass">Type of implementation.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterScoped(Type tClass, bool registerAllInterfaces = false);
    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="tClass">Type of implementation.</param>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    void RegisterScoped(Type tClass, Func<IServiceProvider, object> serviceFactory, bool registerAllInterfaces = false);
}
