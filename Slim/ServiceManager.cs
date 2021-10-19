using Slim.Exceptions;
using Slim.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Slim
{
    /// <summary>
    /// <see cref="ServiceManager"/> responsible with storing and resolving services.
    /// </summary>
    public sealed class ServiceManager : IServiceManager
    {
        private bool disposedValue;
        private readonly bool scoped = false;

        private Dictionary<Type, (Type, Lifetime)> InterfaceMapping { get; } = new Dictionary<Type, (Type, Lifetime)>();
        private Dictionary<Type, object> Instances { get; } = new Dictionary<Type, object>();
        private Dictionary<Type, Delegate> Factories { get; } = new Dictionary<Type, Delegate>();
        private Dictionary<Type, Delegate> ExceptionHandlers { get; } = new Dictionary<Type, Delegate>();
        private List<IDependencyResolver> Resolvers { get; } = new List<IDependencyResolver>();

        /// <summary>
        /// Creates an instance of <see cref="IServiceManager"/>.
        /// </summary>
        public ServiceManager()
        {
        }

        private ServiceManager(
            Dictionary<Type, (Type, Lifetime)> interfaceMapping,
            Dictionary<Type, object> singletons,
            Dictionary<Type, Delegate> factories,
            Dictionary<Type, Delegate> exceptionHandlers,
            List<IDependencyResolver> resolvers)
        {
            this.InterfaceMapping = interfaceMapping;
            this.Instances = singletons;
            this.Factories = factories;
            this.ExceptionHandlers = exceptionHandlers;
            this.Resolvers = resolvers;
            this.scoped = true;
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
        /// Register the current service manager as a valid dependency.
        /// </summary>
        /// <remarks>Registers current service manager as <see cref="ServiceManager"/>, <see cref="IServiceManager"/>, <see cref="IServiceProducer"/>, <see cref="IServiceProvider"/>.</remarks>
        public void RegisterServiceManager()
        {
            this.RegisterSingleton<IServiceManager, ServiceManager>(sp => this);
            this.RegisterSingleton<ServiceManager, ServiceManager>(sp => this);
            this.RegisterSingleton<IServiceProducer, ServiceManager>(sp => this);
            this.RegisterSingleton<IServiceProvider, ServiceManager>(sp => this);
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
        public TInterface GetService<TInterface>() where TInterface : class
        {
            return this.PrepareAndGetService(typeof(TInterface)) as TInterface;
        }
        /// <summary>
        /// Resolves and returns the required service.
        /// </summary>
        /// <param name="type">Type of required service.</param>
        /// <returns>Required service.</returns>
        /// <exception cref="DependencyInjectionException">Thrown when unable to resolve required service.</exception>
        public object GetService(Type type)
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
        /// Clears all registered types, singletons, factories and exception handlers.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="IDisposable.Dispose"/> on all <see cref="IDisposable"/> singletons.
        /// </remarks>
        public void Clear()
        {
            this.InterfaceMapping.Clear();
            foreach(var singleton in this.Instances.Values.Where(s => s is IDisposable).Select(s => (IDisposable)s))
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
            foreach(var mapping in this.InterfaceMapping.Where(map => map.Value.Item2 == Lifetime.Singleton))
            {
                this.GetService(mapping.Key);
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
            foreach(var kvp in this.InterfaceMapping)
            {
                var objectType = kvp.Value.Item1;
                if (type.IsAssignableFrom(objectType))
                {
                    yield return this.PrepareAndGetService(kvp.Key);
                }
            }
        }
        private void RegisterService(Type tClass, Lifetime lifetime, Type tInterface = null, Delegate serviceFactory = null, bool registerAllInterfaces = false)
        {
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
        private object PrepareAndGetService(Type tInterface)
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
                    this.Factories[tinterface] = factory;
                }
            });
        }
        private void Map(Type tinterface, Type tclass, Lifetime lifetime)
        {
            this.TryAction(() =>
            {
                lock (this)
                {
                    if (this.InterfaceMapping.ContainsKey(tinterface))
                    {
                        throw new InvalidOperationException($"{nameof(ServiceManager)} already contains an entry for type {tinterface}");
                    }

                    this.InterfaceMapping[tinterface] = (tclass, lifetime);
                }
            });
        }
        private bool IsMapped(Type tinterface)
        {
            return this.TryFunc(() =>
            {
                lock (this)
                {
                    return this.InterfaceMapping.ContainsKey(tinterface);
                }
            });
        }
        private object GetObject(Type tInterface)
        {
            if (!this.InterfaceMapping.TryGetValue(tInterface, out var mappingTuple) &&
                this.Resolvers.Any(resolver => resolver.CanResolve(tInterface)) is false)
            {
                throw new DependencyInjectionException($"No registered service and no resolver can resolve for type {tInterface.Name}.");
            }

            (var type, var lifetime) = mappingTuple;
            object obj;
            if (lifetime == Lifetime.Singleton || lifetime == Lifetime.Scoped)
            {
                if (!this.Instances.TryGetValue(type, out obj))
                {
                    obj = this.TryImplementService(tInterface);
                }
            }
            else
            {
                obj = this.TryImplementService(tInterface);
            }

            if (obj is object)
            {
                if (lifetime == Lifetime.Singleton || lifetime == Lifetime.Scoped)
                {
                    this.Instances[type] = obj;
                }

                return obj;
            }

            /*
             * If no constructors were able to be called succesfully, throw exception.
             */

            throw new DependencyInjectionException($"No suitable constructor was found for type {tInterface.Name}!");
        }
        private object TryImplementService(Type type)
        {
            foreach (var resolver in this.Resolvers)
            {
                if (resolver.CanResolve(type))
                {
                    return resolver.Resolve(this, type);
                }
            }

            (var implementType, _) = this.InterfaceMapping[type];
            if (this.Factories.TryGetValue(type, out var factory))
            {
                return factory.DynamicInvoke(this);
            }

            var constructors = implementType.GetConstructors();
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
        private object[] GetParameters(ParameterInfo[] parameterInfos)
        {
            var parameterImplementationList = new List<object>();
            foreach (var par in parameterInfos)
            {
                if (!this.InterfaceMapping.TryGetValue(par.ParameterType, out var mappingTuple))
                {
                    object resolvedParam = null;
                    foreach (var resolver in this.Resolvers)
                    {
                        if (resolver.CanResolve(par.ParameterType))
                        {
                            resolvedParam = resolver.Resolve(this, par.ParameterType);
                            continue;
                        }
                    }

                    if (resolvedParam is not null)
                    {
                        parameterImplementationList.Add(resolvedParam);
                        continue;
                    }

                    return null;
                }

                (var actualType, var lifetime) = mappingTuple;
                if (this.Instances.TryGetValue(par.ParameterType, out var obj))
                {
                    parameterImplementationList.Add(obj);
                    continue;
                }

                if (this.Instances.TryGetValue(actualType, out obj))
                {
                    parameterImplementationList.Add(obj);
                    continue;
                }

                obj = this.TryImplementService(par.ParameterType);
                if (obj is null)
                {
                    return null;
                }

                if (lifetime == Lifetime.Singleton)
                {
                    this.Instances[actualType] = obj;
                }

                parameterImplementationList.Add(obj);
            }

            return parameterImplementationList.ToArray();
        }
        private ServiceManager CreateScopeInner()
        {
            var interfaceMapping = new Dictionary<Type, (Type, Lifetime)>(this.InterfaceMapping);
            var factories = new Dictionary<Type, Delegate>(this.Factories);
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
                if (kvp.Value.Item2 is Lifetime.Singleton)
                {
                    factories[kvp.Key] = new Func<IServiceProvider, object>((_) => this.PrepareAndGetService(kvp.Key));
                }
            }

            return new ServiceManager(interfaceMapping, new Dictionary<Type, object>(), factories, exceptionHandlers, resolvers);
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
        private T TryFunc<T>(Func<T> action)
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
                    foreach (var interfacePair in this.InterfaceMapping
                        .Where(kvp => kvp.Value.Item2 != Lifetime.Transient)
                        .Where(kvp => this.scoped is false || kvp.Value.Item2 != Lifetime.Singleton))
                    {
                        if (this.Instances.TryGetValue(interfacePair.Key, out var instance) && instance is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                        else if (this.Instances.TryGetValue(interfacePair.Value.Item1, out var instance2) && instance2 is IDisposable disposable2)
                        {
                            disposable2.Dispose();
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
    }
}