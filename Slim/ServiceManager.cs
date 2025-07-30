using Slim.Attributes;
using Slim.Exceptions;
using Slim.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Slim;

/// <summary>
/// <see cref="ServiceManager"/> responsible with storing and resolving services.
/// </summary>
public sealed class ServiceManager : IServiceManager
{
    private bool disposedValue;
    private readonly bool scoped = false;

    private Dictionary<Type, List<(Type, Lifetime)>> InterfaceMapping { get; } = [];
    private Dictionary<Type, List<object>> Instances { get; } = [];
    private Dictionary<Type, List<Delegate>> Factories { get; } = [];
    private Dictionary<Type, Delegate> ExceptionHandlers { get; } = [];
    private List<IDependencyResolver> Resolvers { get; } = [];

    /// <summary>
    /// Allow modifications to scoped <see cref="IServiceManager"/> created from this <see cref="IServiceManager"/>.
    /// </summary>
    public bool AllowScopedManagerModifications { get; set; }
    /// <summary>
    /// Returns true if <see cref="IServiceManager"/> is readonly.
    /// </summary>
    public bool IsReadOnly { get; }
    /// <summary>
    /// Gets the <see cref="IServiceManager"/> that created the current <see cref="IServiceManager"/>. Returns null in case there is no parent.
    /// </summary>
    public IServiceManager? ParentServiceManager { get; }

    /// <summary>
    /// Creates an instance of <see cref="IServiceManager"/>.
    /// </summary>
    public ServiceManager()
    {
    }

    private ServiceManager(
        Dictionary<Type, List<(Type, Lifetime)>> interfaceMapping,
        Dictionary<Type, List<object>> singletons,
        Dictionary<Type, List<Delegate>> factories,
        Dictionary<Type, Delegate> exceptionHandlers,
        List<IDependencyResolver> resolvers,
        IServiceManager parent,
        bool isReadOnly)
    {
        this.InterfaceMapping = interfaceMapping;
        this.Instances = singletons;
        this.Factories = factories;
        this.ExceptionHandlers = exceptionHandlers;
        this.Resolvers = resolvers;
        this.scoped = true;
        this.ParentServiceManager = parent;
        this.IsReadOnly = isReadOnly;
    }

    /// <summary>
    /// Returns true if there exists a registration for <paramref name="tInterface"/>.
    /// </summary>
    /// <param name="tInterface">Type of registered service.</param>
    /// <returns>True if service <paramref name="tInterface"/> is registered. Otherwise returns false.</returns>
    public bool IsRegistered(Type tInterface)
    {
        return this.IsMapped(tInterface);
    }

    /// <summary>
    /// Returns true if there exists a registration for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of registered service.</typeparam>
    /// <returns>True if service <typeparamref name="T"/> is registered. Otherwise returns false.</returns>
    public bool IsRegistered<T>()
    {
        return this.IsMapped(typeof(T));
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterTransient<TInterface, TClass>()
        where TInterface : class
        where TClass : TInterface
    {
        this.RegisterService(typeof(TClass), Lifetime.Transient, typeof(TInterface));
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <param name="serviceFactory">Factory for the implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterTransient<TInterface, TClass>(Func<IServiceProvider, TClass> serviceFactory)
        where TInterface : class
        where TClass : TInterface
    {
        if (serviceFactory is null)
        {
            throw new ArgumentNullException(nameof(serviceFactory));
        }

        this.RegisterService(typeof(TClass), Lifetime.Transient, typeof(TInterface), serviceFactory);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterSingleton<TInterface, TClass>()
        where TInterface : class
        where TClass : TInterface
    {
        this.RegisterService(typeof(TClass), Lifetime.Singleton, typeof(TInterface));
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterSingleton<TInterface, TClass>(Func<IServiceProvider, TClass> serviceFactory)
        where TInterface : class
        where TClass : TInterface
    {
        if (serviceFactory is null)
        {
            throw new ArgumentNullException(nameof(serviceFactory));
        }

        this.RegisterService(typeof(TClass), Lifetime.Singleton, typeof(TInterface), serviceFactory);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterScoped<TInterface, TClass>()
        where TInterface : class
        where TClass : TInterface
    {
        this.RegisterService(typeof(TClass), Lifetime.Scoped, typeof(TInterface));
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterScoped<TInterface, TClass>(Func<IServiceProvider, TClass> serviceFactory)
        where TInterface : class
        where TClass : TInterface
    {
        if (serviceFactory is null)
        {
            throw new ArgumentNullException(nameof(serviceFactory));
        }

        this.RegisterService(typeof(TClass), Lifetime.Scoped, typeof(TInterface), serviceFactory);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// </summary>
    /// <param name="tInterface">Type of interface.</param>
    /// <param name="tClass">Type of implementation.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterTransient(Type tInterface, Type tClass)
    {
        this.RegisterService(tClass, Lifetime.Transient, tInterface);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// </summary>
    /// <param name="tInterface">Type of interface.</param>
    /// <param name="tClass">Type of implementation.</param>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterTransient(Type tInterface, Type tClass, Func<IServiceProvider, object> serviceFactory)
    {
        if (serviceFactory is null)
        {
            throw new ArgumentNullException(nameof(serviceFactory));
        }

        this.RegisterService(tClass, Lifetime.Transient, tInterface, serviceFactory);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// </summary>
    /// <param name="tInterface">Type of interface.</param>
    /// <param name="tClass">Type of implementation.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterSingleton(Type tInterface, Type tClass)
    {
        this.RegisterService(tClass, Lifetime.Singleton, tInterface);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// </summary>
    /// <param name="tInterface">Type of interface.</param>
    /// <param name="tClass">Type of implementation.</param>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterSingleton(Type tInterface, Type tClass, Func<IServiceProvider, object> serviceFactory)
    {
        if (serviceFactory is null)
        {
            throw new ArgumentNullException(nameof(serviceFactory));
        }

        this.RegisterService(tClass, Lifetime.Singleton, tInterface, serviceFactory);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// </summary>
    /// <param name="tInterface">Type of interface.</param>
    /// <param name="tClass">Type of implementation.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterScoped(Type tInterface, Type tClass)
    {
        this.RegisterService(tClass, Lifetime.Scoped, tInterface);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// </summary>
    /// <param name="tInterface">Type of interface.</param>
    /// <param name="tClass">Type of implementation.</param>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterScoped(Type tInterface, Type tClass, Func<IServiceProvider, object> serviceFactory)
    {
        if (serviceFactory is null)
        {
            throw new ArgumentNullException(nameof(serviceFactory));
        }

        this.RegisterService(tClass, Lifetime.Scoped, tInterface, serviceFactory);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterSingleton<TClass>(bool registerAllInterfaces = false) where TClass : class
    {
        this.RegisterService(typeof(TClass), Lifetime.Singleton, registerAllInterfaces: registerAllInterfaces);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <param name="serviceFactory">Factory for the implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterSingleton<TClass>(Func<IServiceProvider, TClass> serviceFactory, bool registerAllInterfaces = false) where TClass : class
    {
        if (serviceFactory is null)
        {
            throw new ArgumentNullException(nameof(serviceFactory));
        }

        this.RegisterService(typeof(TClass), Lifetime.Singleton, null, serviceFactory, registerAllInterfaces);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterTransient<TClass>(bool registerAllInterfaces = false) where TClass : class
    {
        this.RegisterService(typeof(TClass), Lifetime.Transient, registerAllInterfaces: registerAllInterfaces);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <param name="serviceFactory">Factory for the implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterTransient<TClass>(Func<IServiceProvider, TClass> serviceFactory, bool registerAllInterfaces = false) where TClass : class
    {
        if (serviceFactory is null)
        {
            throw new ArgumentNullException(nameof(serviceFactory));
        }

        this.RegisterService(typeof(TClass), Lifetime.Transient, null, serviceFactory, registerAllInterfaces);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterScoped<TClass>(bool registerAllInterfaces = false) where TClass : class
    {
        this.RegisterService(typeof(TClass), Lifetime.Scoped, registerAllInterfaces: registerAllInterfaces);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <typeparam name="TClass">Type of implementation.</typeparam>
    /// <param name="serviceFactory">Factory for the implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterScoped<TClass>(Func<IServiceProvider, TClass> serviceFactory, bool registerAllInterfaces = false) where TClass : class
    {
        if (serviceFactory is null)
        {
            throw new ArgumentNullException(nameof(serviceFactory));
        }

        this.RegisterService(typeof(TClass), Lifetime.Scoped, null, serviceFactory, registerAllInterfaces);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="tClass">Type of implementation.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterTransient(Type tClass, bool registerAllInterfaces = false)
    {
        this.RegisterService(tClass, Lifetime.Transient, registerAllInterfaces: registerAllInterfaces);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="tClass">Type of implementation.</param>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterTransient(Type tClass, Func<IServiceProvider, object> serviceFactory, bool registerAllInterfaces = false)
    {
        this.RegisterService(tClass, Lifetime.Transient, null, serviceFactory, registerAllInterfaces: registerAllInterfaces);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="tClass">Type of implementation.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterSingleton(Type tClass, bool registerAllInterfaces = false)
    {
        this.RegisterService(tClass, Lifetime.Singleton, registerAllInterfaces: registerAllInterfaces);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="tClass">Type of implementation.</param>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterSingleton(Type tClass, Func<IServiceProvider, object> serviceFactory, bool registerAllInterfaces = false)
    {
        this.RegisterService(tClass, Lifetime.Singleton, null, serviceFactory, registerAllInterfaces: registerAllInterfaces);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="tClass">Type of implementation.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterScoped(Type tClass, bool registerAllInterfaces = false)
    {
        this.RegisterService(tClass, Lifetime.Scoped, registerAllInterfaces: registerAllInterfaces);
    }

    /// <summary>
    /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Scoped"/>.
    /// Registers the service for all interfaces it implements.
    /// </summary>
    /// <param name="registerAllInterfaces">If true, <see cref="ServiceManager"/> will register all interfaces implemented by the provided class./></param>
    /// <param name="tClass">Type of implementation.</param>
    /// <param name="serviceFactory">Factory for implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
    public void RegisterScoped(Type tClass, Func<IServiceProvider, object> serviceFactory, bool registerAllInterfaces = false)
    {
        this.RegisterService(tClass, Lifetime.Scoped, null, serviceFactory, registerAllInterfaces: registerAllInterfaces);
    }

    /// <summary>
    /// Creates a scoped <see cref="IServiceProvider"/>.
    /// </summary>
    /// <returns>Scoped <see cref="IServiceProvider"/>.</returns>
    public IServiceProvider CreateScope()
    {
        return this.CreateScopeInner();
    }

    /// <summary>
    /// Resolves and returns the required service.
    /// </summary>
    /// <typeparam name="TInterface">Type of required service.</typeparam>
    /// <returns>Required service.</returns>
    /// <exception cref="DependencyInjectionException">Thrown when unable to resolve required service.</exception>
    public TInterface? GetService<TInterface>() where TInterface : class
    {
        return this.PrepareAndGetService(typeof(TInterface)) as TInterface;
    }

    /// <summary>
    /// Resolves and returns the required service.
    /// </summary>
    /// <param name="type">Type of required service.</param>
    /// <returns>Required service.</returns>
    /// <exception cref="DependencyInjectionException">Thrown when unable to resolve required service.</exception>
    public object? GetService(Type type)
    {
        return this.PrepareAndGetService(type);
    }

    /// <summary>
    /// Returns all services that can be cast to provided type.
    /// </summary>
    /// <typeparam name="T">Type of the returned services.</typeparam>
    public IEnumerable<T> GetServicesOfType<T>()
    {
        return this.EnumerateAndReturnServicesOfType(typeof(T)).OfType<T>();
    }

    /// <summary>
    /// Returns all services that can be cast to provided type.
    /// </summary>
    /// <param name="type">Type of the returned services.</param>
    public IEnumerable<object> GetServicesOfType(Type type)
    {
        return this.EnumerateAndReturnServicesOfType(type);
    }

    /// <summary>
    /// Marks a type of exception to be caught and handled.
    /// </summary>
    /// <typeparam name="T">Type of exception to catch.</typeparam>
    /// <param name="handle">Handler of the exception. Handler returns true if the exception should be thrown again.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public void HandleException<T>(Func<IServiceProvider, T, bool> handle)
        where T : Exception
    {
        this.ExceptionHandlers.Add(typeof(T), handle);
    }

    /// <summary>
    /// Remove a registered service.
    /// </summary>
    /// <param name="tInterface">Registered type.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void Remove(Type tInterface)
    {
        _ = tInterface ?? throw new ArgumentNullException(nameof(tInterface));

        this.RemoveService(tInterface);
    }

    /// <summary>
    /// Remove a registered service.
    /// </summary>
    /// <typeparam name="T">Registered interface.</typeparam>
    public void Remove<T>()
    {
        this.RemoveService(typeof(T));
    }

    /// <summary>
    /// Clears all registered types, singletons, factories and exception handlers.
    /// </summary>
    /// <remarks>
    /// Calls <see cref="IDisposable.Dispose"/> on all <see cref="IDisposable"/> singletons.
    /// </remarks>
    public void Clear()
    {
        if (this.IsReadOnly)
        {
            throw new InvalidOperationException($"Cannot clear {nameof(ServiceManager)}. {nameof(ServiceManager)} is readonly!");
        }

        this.InterfaceMapping.Clear();
        foreach (var singleton in this.Instances.Values.SelectMany(instances => instances).Where(s => s is IDisposable).Select(s => (IDisposable)s))
        {
            singleton.Dispose();
        }

        this.Factories.Clear();
        this.ExceptionHandlers.Clear();
    }

    /// <summary>
    /// Build all registered singletons.
    /// </summary>
    public void BuildSingletons()
    {
        foreach(var mapping in this.InterfaceMapping)
        {
            foreach(var kvp in mapping.Value)
            {
                if (kvp.Item2 is Lifetime.Singleton)
                {
                    this.GetService(mapping.Key);
                }
            }
        }
    }

    /// <summary>
    /// Register a <see cref="IDependencyResolver"/> that manually resolves dependencies.
    /// </summary>
    /// <remarks>
    /// This resolver will be called before auto-resolving a dependency. If resolver can handle the dependency, <see cref="IServiceManager"/> will return the resolved value.
    /// </remarks>
    /// <param name="dependencyResolver"><see cref="IDependencyResolver"/> that manually creates a dependency.</param>
    public void RegisterResolver(IDependencyResolver dependencyResolver)
    {
        if (this.IsReadOnly)
        {
            throw new InvalidOperationException($"Cannot register resolver. {nameof(ServiceManager)} is readonly!");
        }

        this.Resolvers.Add(dependencyResolver);
    }

    /// <summary>
    /// Cleans up resources and disposes of all disposable singletons.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(disposing: true);
    }

    private IEnumerable<object> EnumerateAndReturnServicesOfType(Type type)
    {
        foreach (var kvp in this.InterfaceMapping)
        {
            foreach(var registration in kvp.Value)
            {
                var objectType = registration.Item1;
                if (type.IsAssignableFrom(objectType) &&
                    this.PrepareAndGetService(kvp.Key) is object obj)
                {
                    yield return obj;
                }
            }
        }
    }

    private void RegisterService(Type tClass, Lifetime lifetime, Type? tInterface = null, Delegate? serviceFactory = null, bool registerAllInterfaces = false)
    {
        if (this.IsReadOnly)
        {
            throw new InvalidOperationException($"Cannot register service. {nameof(ServiceManager)} is readonly!");
        }

        if (tInterface is not null)
        {
            this.Map(tInterface, tClass, lifetime);
            if (serviceFactory is not null)
            {
                this.RegisterFactory(tInterface, serviceFactory);
            }

            return;
        }

        if (registerAllInterfaces)
        {
            foreach (var i in tClass.GetInterfaces())
            {
                this.Map(i, tClass, lifetime);
                if (serviceFactory is not null)
                {
                    this.RegisterFactory(i, serviceFactory);
                }
            }
        }

        this.Map(tClass, tClass, lifetime);
        if (serviceFactory is not null)
        {
            this.RegisterFactory(tClass, serviceFactory);
        }
    }

    private object? PrepareAndGetService(Type tInterface)
    {
        return this.TryFunc(() =>
        {
            return this.GetObject(tInterface);
        });
    }

    private void RegisterFactory(Type tinterface, Delegate factory)
    {
        this.TryAction(() =>
        {
            lock (this)
            {
                if (!this.Factories.TryGetValue(tinterface, out var factories))
                {
                    factories = [];
                    this.Factories[tinterface] = factories;
                }

                factories.Add(factory);
            }
        });
    }

    private void RemoveService(Type tInterface)
    {
        this.TryAction(() =>
        {
            lock (this)
            {
                this.Factories.Remove(tInterface);
                this.InterfaceMapping.Remove(tInterface);
                if (this.Instances.TryGetValue(tInterface, out var service) &&
                    service is IDisposable disposableService)
                {
                    disposableService.Dispose();
                }
            }
        });
    }

    private void Map(Type tinterface, Type tclass, Lifetime lifetime)
    {
        this.TryAction(() =>
        {
            lock (this)
            {
                if (!this.InterfaceMapping.TryGetValue(tinterface, out var value))
                {
                    value = [];
                    this.InterfaceMapping[tinterface] = value;
                }

                value.Add((tclass, lifetime));
            }
        });
    }

    private bool IsMapped(Type tinterface)
    {
        return this.TryFunc(() =>
        {
            if (IsServiceManagerDependency(tinterface))
            {
                return true;
            }

            lock (this)
            {
                if (this.Resolvers.Any(resolver => resolver.CanResolve(tinterface)))
                {
                    return true;
                }

                return this.InterfaceMapping.ContainsKey(tinterface);
            }
        });
    }

    private object? GetObject(Type tInterface)
    {
        if (IsServiceManagerDependency(tInterface))
        {
            return this;
        }

        if (!this.InterfaceMapping.TryGetValue(tInterface, out var mappingTuples) &&
            this.Resolvers.Any(resolver => resolver.CanResolve(tInterface)) is false)
        {
            return default;
        }

        var tuple = mappingTuples?.FirstOrDefault();
        if (tuple is not null &&
            tuple.Value.Item2 is Lifetime.Singleton or Lifetime.Scoped)
        {
            if (this.Instances.TryGetValue(tuple.Value.Item1, out var objs) &&
                objs.FirstOrDefault() is object obj)
            {
                return obj;
            }
            else
            {
                if (this.TryImplementService(tInterface) is not object newObj)
                {
                    throw new DependencyInjectionException($"No suitable constructor was found for type {tInterface.Name}");
                }

                if (objs is null)
                {
                    objs = [];
                    this.Instances[tuple.Value.Item1] = objs;
                }

                objs.Add(newObj);
                return newObj;
            }
        }
        else
        {
            if (this.TryImplementService(tInterface) is not object obj)
            {
                throw new DependencyInjectionException($"No suitable constructor was found for type {tInterface.Name}");
            }

            return obj;
        }
    }

    private object? TryImplementService(Type type)
    {
        foreach (var resolver in this.Resolvers)
        {
            if (resolver.CanResolve(type))
            {
                return resolver.Resolve(this, type);
            }
        }

        var registrations = this.InterfaceMapping[type];
        if (registrations is null ||
            registrations.Count is 0)
        {
            throw new DependencyInjectionException($"No registered service for type {type.Name}.");
        }

        (var implementType, _) = registrations.FirstOrDefault();
        if (this.Factories.TryGetValue(type, out var factories) &&
            factories is not null &&
            factories.Count > 0)
        {
            return factories.FirstOrDefault().DynamicInvoke(this);
        }

        return this.FindAndCallConstructors(implementType);
    }

    private object? FindAndCallConstructors(Type implementType)
    {
        /*
         * Order constructors to give priority to constructors that have PrefferedConstructorAttribute decorator
         */
        var constructors = implementType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(c => c.GetCustomAttribute<DoNotInjectAttribute>() is null)
            .OrderBy(c =>
            c.GetCustomAttribute<PreferredConstructorAttribute>() is PreferredConstructorAttribute preferredConstructorAttribute ?
            preferredConstructorAttribute.Priority :
            int.MaxValue);
        foreach (var constructor in constructors)
        {
            /*
             * Filter by public constructors and try to find if the DI Service has
             * implementations for the required parameters. If there are, add them to a list and call
             * the constructor with the paramters.
             * If the parameters are missing, try with other constructors.
             */

            var parameters = this.GetParameters(constructor.GetParameters());
            if (parameters is null)
            {
                continue;
            }

            return constructor.Invoke(parameters);
        }

        return null;
    }

    private object[]? GetParameters(ParameterInfo[] parameterInfos)
    {
        var parameterImplementationList = new object[parameterInfos.Length];
        var index = 0;
        foreach (var par in parameterInfos)
        {
            if (IsServiceManagerDependency(par.ParameterType))
            {
                parameterImplementationList[index] = this;
                index++;
                continue;
            }

            if (!this.InterfaceMapping.TryGetValue(par.ParameterType, out var registrations) ||
                registrations is null ||
                registrations.Count is 0)
            {
                object? resolvedParam = null;
                foreach (var resolver in this.Resolvers)
                {
                    if (resolver.CanResolve(par.ParameterType))
                    {
                        resolvedParam = resolver.Resolve(this, par.ParameterType);
                        break;
                    }
                }

                if (resolvedParam is not null)
                {
                    parameterImplementationList[index] = resolvedParam;
                    index++;
                    continue;
                }

                return default;
            }

            (var actualType, var lifetime) = registrations.FirstOrDefault();
            if (this.Instances.TryGetValue(par.ParameterType, out var instances) &&
                instances is not null &&
                instances.Count > 0)
            {
                parameterImplementationList[index] = instances.FirstOrDefault();
                index++;
                continue;
            }

            if (this.Instances.TryGetValue(actualType, out var instances2) &&
                instances2 is not null &&
                instances2.Count > 0)
            {
                parameterImplementationList[index] = instances2.FirstOrDefault();
                index++;
                continue;
            }

            var obj = this.TryImplementService(par.ParameterType);
            if (obj is null)
            {
                return null;
            }

            if (lifetime is Lifetime.Singleton or Lifetime.Scoped)
            {
                if (!this.Instances.TryGetValue(actualType, out var actualRegistrations) ||
                    actualRegistrations is null)
                {
                    actualRegistrations = [];
                    this.Instances[actualType] = actualRegistrations;
                }

                actualRegistrations.Add(obj);
            }

            parameterImplementationList[index] = obj;
            index++;
        }

        return parameterImplementationList;
    }

    private ServiceManager CreateScopeInner()
    {
        var interfaceMapping = new Dictionary<Type, List<(Type, Lifetime)>>(this.InterfaceMapping);
        var factories = new Dictionary<Type, List<Delegate>>();
        var exceptionHandlers = new Dictionary<Type, Delegate>(this.ExceptionHandlers);
        var resolvers = new List<IDependencyResolver>(this.Resolvers);
        /*
         * For singletons, create a factory in scoped service manager that should call this manager
         * to retrieve the singleton.
         * This means that if the singleton has not yet been created in this service manager,
         * the scoped service manager will not create a new one but instead ask the original service manager to
         * create the singleton instead.
         */
        foreach (var kvp in this.InterfaceMapping)
        {
            if (kvp.Value.Count is 0)
            {
                continue;
            }

            foreach(var registration in kvp.Value)
            {
                /*
                 * Singletons reference the original service manager while
                 * scoped or transient will reference the original factories.
                 */
                if (registration.Item2 is Lifetime.Singleton)
                {
                    if (!factories.TryGetValue(kvp.Key, out var factoryRegistrations) ||
                        factoryRegistrations is null)
                    {
                        factoryRegistrations = [];
                        factories[kvp.Key] = factoryRegistrations;
                    }

                    factoryRegistrations.Add(new Func<IServiceProvider, object>((scopedManager) => this.ResolveSingletonWithScopedManager((ServiceManager)scopedManager, kvp.Key, registration.Item1)));
                }
                else if (this.Factories.TryGetValue(kvp.Key, out var originalFactories))
                {
                    if (!factories.TryGetValue(kvp.Key, out var factoryRegistrations) ||
                        factoryRegistrations is null)
                    {
                        factoryRegistrations = [];
                        factories[kvp.Key] = factoryRegistrations;
                    }

                    foreach(var factory in originalFactories)
                    {
                        factoryRegistrations.Add(factory);
                    }
                }
            }
        }

        return new ServiceManager(
            interfaceMapping: interfaceMapping,
            singletons: [],
            factories: factories,
            exceptionHandlers: exceptionHandlers,
            resolvers: resolvers,
            parent: this,
            isReadOnly: this.AllowScopedManagerModifications is false);
    }

    private object ResolveSingletonWithScopedManager(ServiceManager scopedManager, Type registerType, Type implementedType)
    {
        try
        {
            if (this.PrepareAndGetService(registerType) is not object obj)
            {
                throw new DependencyInjectionException($"Unable to resolve singleton of type {registerType.Name}.");
            }

            return obj;
        }
        catch (DependencyInjectionException)
        {
            if (scopedManager.IsReadOnly)
            {
                throw;
            }

            /*
             * Most probably scoped manager has been modified and contains different definitions
             * than the parent manager.
             * As a last ditch effort, let the scoped provider try to implement the singleton.
             * If successful, update the parent provider as well.
             */
            var obj = scopedManager.FindAndCallConstructors(implementedType);
            if (obj is not null)
            {
                if (!scopedManager.Instances.TryGetValue(implementedType, out var scopedInstances) ||
                    scopedInstances is null)
                {
                    scopedInstances = [];
                    scopedManager.Instances[implementedType] = scopedInstances;
                }

                scopedInstances.Add(obj);

                if (!this.Instances.TryGetValue(implementedType, out var instances) ||
                    instances is null)
                {
                    instances = [];
                    this.Instances[implementedType] = instances;
                }

                instances.Add(obj);
                return obj;
            }

            throw;
        }
    }

    private void TryAction(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private T? TryFunc<T>(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }

        return default;
    }

    private void HandleException(Exception exception)
    {
        if (exception is TargetInvocationException && exception.InnerException is not null)
        {
            exception = exception.InnerException;
        }

        if (this.ExceptionHandlers.TryGetValue(exception.GetType(), out var handler))
        {
            var shouldThrow = (bool)handler.DynamicInvoke(this, exception);
            if (shouldThrow is false)
            {
                throw exception;
            }
        }
        else
        {
            throw exception;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                /*
                 * Dispose of all instances. If this instance of ServiceManager is scoped, ignore the Singleton instances as they are shared
                 * across multiple service managers.
                 */
                foreach(var kvp in this.InterfaceMapping)
                {
                    foreach((var type, var lifetime) in kvp.Value)
                    {
                        if (lifetime is Lifetime.Scoped ||
                            this.scoped is false && lifetime is Lifetime.Singleton)
                        {
                            if (this.Instances.TryGetValue(kvp.Key, out var instances1))
                            {
                                foreach(var instance in instances1.OfType<IDisposable>())
                                {
                                    instance.Dispose();
                                }
                            }
                            else if (this.Instances.TryGetValue(type, out var instances2))
                            {
                                foreach(var instance in instances2.OfType<IDisposable>())
                                {
                                    instance.Dispose();
                                }
                            }
                        }
                    }
                }
            }

            this.InterfaceMapping.Clear();
            this.Instances.Clear();
            this.Resolvers.Clear();
            this.ExceptionHandlers.Clear();
            this.Factories.Clear();
            this.disposedValue = true;
        }
    }

    private static bool IsServiceManagerDependency(Type type)
    {
        return type == typeof(IServiceManager) ||
                type == typeof(IServiceProducer) ||
                type == typeof(IServiceProvider) ||
                type == typeof(ServiceManager) ||
                type == typeof(System.IServiceProvider);
    }
}
