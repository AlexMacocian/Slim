using Slim.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Slim
{
    /// <summary>
    /// <see cref="ServiceManager"/> responsible with storing and resolving services.
    /// </summary>
    public class ServiceManager : IServiceManager
    {
        private Dictionary<Type, (Type, Lifetime)> InterfaceMapping { get; } = new Dictionary<Type, (Type, Lifetime)>();
        private Dictionary<Type, object> Singletons { get; } = new Dictionary<Type, object>();
        private Dictionary<Type, Delegate> Factories { get; } = new Dictionary<Type, Delegate>();
        private Dictionary<Type, Delegate> ExceptionHandlers { get; } = new Dictionary<Type, Delegate>();

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
            this.Map(typeof(TInterface), typeof(TClass), Lifetime.Transient);
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
            if (serviceFactory is null) throw new ArgumentNullException(nameof(serviceFactory));

            this.Map(typeof(TInterface), typeof(TClass), Lifetime.Transient);
            this.RegisterFactory(typeof(TInterface), serviceFactory);
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
            this.Map(typeof(TInterface), typeof(TClass), Lifetime.Singleton);
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
            if (serviceFactory is null) throw new ArgumentNullException(nameof(serviceFactory));

            this.Map(typeof(TInterface), typeof(TClass), Lifetime.Singleton);
            this.RegisterFactory(typeof(TInterface), serviceFactory);
        }
        /// <summary>
        /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
        /// </summary>
        /// <param name="tInterface">Type of interface.</param>
        /// <param name="tClass">Type of implementation.</param>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
        public void RegisterTransient(Type tInterface, Type tClass)
        {
            this.Map(tInterface, tClass, Lifetime.Transient);
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
            if (serviceFactory is null) throw new ArgumentNullException(nameof(serviceFactory));

            this.Map(tInterface, tClass, Lifetime.Transient);
            this.RegisterFactory(tInterface, serviceFactory);
        }
        /// <summary>
        /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
        /// </summary>
        /// <param name="tInterface">Type of interface.</param>
        /// <param name="tClass">Type of implementation.</param>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
        public void RegisterSingleton(Type tInterface, Type tClass)
        {
            this.Map(tInterface, tClass, Lifetime.Singleton);
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
            if (serviceFactory is null) throw new ArgumentNullException(nameof(serviceFactory));

            this.Map(tInterface, tClass, Lifetime.Singleton);
            this.RegisterFactory(tInterface, serviceFactory);
        }
        /// <summary>
        /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
        /// Registers the service for all interfaces it implements.
        /// </summary>
        /// <typeparam name="TClass">Type of implementation.</typeparam>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
        public void RegisterSingleton<TClass>() where TClass : class
        {
            foreach(var i in typeof(TClass).GetInterfaces())
            {
                this.Map(i, typeof(TClass), Lifetime.Singleton);
            }
        }
        /// <summary>
        /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
        /// Registers the service for all interfaces it implements.
        /// </summary>
        /// <typeparam name="TClass">Type of implementation.</typeparam>
        /// <param name="serviceFactory">Factory for the implementation.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
        public void RegisterSingleton<TClass>(Func<IServiceProvider, TClass> serviceFactory) where TClass : class
        {
            if (serviceFactory is null) throw new ArgumentNullException(nameof(serviceFactory));

            foreach (var i in typeof(TClass).GetInterfaces())
            {
                this.Map(i, typeof(TClass), Lifetime.Singleton);
                this.RegisterFactory(i, serviceFactory);
            }
        }
        /// <summary>
        /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
        /// Registers the service for all interfaces it implements.
        /// </summary>
        /// <typeparam name="TClass">Type of implementation.</typeparam>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
        public void RegisterTransient<TClass>() where TClass : class
        {
            foreach (var i in typeof(TClass).GetInterfaces())
            {
                this.Map(i, typeof(TClass), Lifetime.Transient);
            }
        }
        /// <summary>
        /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
        /// Registers the service for all interfaces it implements.
        /// </summary>
        /// <typeparam name="TClass">Type of implementation.</typeparam>
        /// <param name="serviceFactory">Factory for the implementation.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
        public void RegisterTransient<TClass>(Func<IServiceProvider, TClass> serviceFactory) where TClass : class
        {
            if (serviceFactory is null) throw new ArgumentNullException(nameof(serviceFactory));

            foreach (var i in typeof(TClass).GetInterfaces())
            {
                this.Map(i, typeof(TClass), Lifetime.Transient);
                this.RegisterFactory(i, serviceFactory);
            }
        }
        /// <summary>
        /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
        /// Registers the service for all interfaces it implements.
        /// </summary>
        /// <param name="tClass">Type of implementation.</param>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
        public void RegisterTransient(Type tClass)
        {
            foreach (var i in tClass.GetInterfaces())
            {
                this.Map(i, tClass, Lifetime.Transient);
            }
        }
        /// <summary>
        /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
        /// Registers the service for all interfaces it implements.
        /// </summary>
        /// <param name="tClass">Type of implementation.</param>
        /// <param name="serviceFactory">Factory for implementation.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
        public void RegisterTransient(Type tClass, Func<IServiceProvider, object> serviceFactory)
        {
            foreach (var i in tClass.GetInterfaces())
            {
                this.Map(i, tClass, Lifetime.Transient);
                this.RegisterFactory(i, serviceFactory);
            }
        }
        /// <summary>
        /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
        /// Registers the service for all interfaces it implements.
        /// </summary>
        /// <param name="tClass">Type of implementation.</param>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
        public void RegisterSingleton(Type tClass)
        {
            foreach (var i in tClass.GetInterfaces())
            {
                this.Map(i, tClass, Lifetime.Singleton);
            }
        }
        /// <summary>
        /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Singleton"/>.
        /// Registers the service for all interfaces it implements.
        /// </summary>
        /// <param name="tClass">Type of implementation.</param>
        /// <param name="serviceFactory">Factory for implementation.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="ServiceManager"/> contains an entry for the provided interface type.</exception>
        public void RegisterSingleton(Type tClass, Func<IServiceProvider, object> serviceFactory)
        {
            foreach (var i in tClass.GetInterfaces())
            {
                this.Map(i, tClass, Lifetime.Singleton);
                this.RegisterFactory(i, serviceFactory);
            }
        }

        /// <summary>
        /// Register the current service manager as a valid dependency.
        /// </summary>
        /// <remarks>Same functionality as calling <see cref="IServiceProducer.RegisterSingleton(Type, Func{IServiceProvider, object})" /> with current <see cref="IServiceManager"/>.</remarks>
        public void RegisterServiceManager()
        {
            foreach(var i in this.GetType().GetInterfaces())
            {
                this.Map(i, this.GetType(), Lifetime.Singleton);
                this.RegisterFactory(i, new Func<IServiceProvider, object>((sp) => { return this; }));
            }
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
            foreach(var singleton in this.Singletons.Values.Where(s => s is IDisposable).Select(s => (IDisposable)s))
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

        private object PrepareAndGetService(Type tInterface)
        {
            try
            {
                lock (this)
                {
                    return this.GetObject(tInterface);
                }
            }
            catch(Exception e)
            {
                if (this.ExceptionHandlers.TryGetValue(e.GetType(), out var handler))
                {
                    var shouldThrow = (bool)handler.DynamicInvoke(this, e);
                    if (shouldThrow)
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
                return null;
            }
        }
        private void RegisterFactory(Type tinterface, Delegate factory)
        {
            try
            {
                lock (this)
                {
                    this.Factories[tinterface] = factory;
                }
            }
            catch(Exception e)
            {
                if (this.ExceptionHandlers.TryGetValue(e.GetType(), out var handler))
                {
                    var shouldThrow = (bool)handler.DynamicInvoke(this, e);
                    if (shouldThrow)
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }
        private void Map(Type tinterface, Type tclass, Lifetime lifetime)
        {
            try
            {
                lock (this)
                {
                    if (this.InterfaceMapping.ContainsKey(tinterface))
                    {
                        throw new InvalidOperationException($"{nameof(ServiceManager)} already contains an entry for type {tinterface}");
                    }
                    this.InterfaceMapping[tinterface] = (tclass, lifetime);
                }
            }
            catch(Exception e)
            {
                if (this.ExceptionHandlers.TryGetValue(e.GetType(), out var handler))
                {
                    var shouldThrow = (bool)handler.DynamicInvoke(this, e);
                    if (shouldThrow)
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }
        private object GetObject(Type tInterface)
        {
            if (!this.InterfaceMapping.TryGetValue(tInterface, out var mappingTuple))
            {
                throw new DependencyInjectionException($"No registered service for type {tInterface.Name}!");
            }

            (var type, var lifetime) = mappingTuple;
            object obj = null;
            if (lifetime == Lifetime.Singleton)
            {
                if (!this.Singletons.TryGetValue(type, out obj))
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
                if (lifetime == Lifetime.Singleton)
                {
                    this.Singletons[type] = obj;
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
            (var implementType, _) = InterfaceMapping[type];
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

                if (!constructor.IsPublic)
                {
                    continue;
                }

                try
                {
                    var parameters = this.GetParameters(constructor.GetParameters());
                    if (parameters is null)
                    {
                        continue;
                    }

                    return constructor.Invoke(parameters);
                }
                catch(DependencyInjectionException exception)
                {
                    throw new DependencyInjectionException($"Could not instantiate service of type {implementType}!", exception);
                }
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
                    throw new DependencyInjectionException($"No service registered with type {par.ParameterType}!");
                }

                (var actualType, var lifetime) = mappingTuple;
                if (this.Singletons.TryGetValue(par.ParameterType, out var obj))
                {
                    parameterImplementationList.Add(obj);
                    continue;
                }

                if (this.Singletons.TryGetValue(actualType, out obj))
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
                    this.Singletons[actualType] = obj;
                }

                parameterImplementationList.Add(obj);
            }

            return parameterImplementationList.ToArray();
        }
    }
}
