﻿using Slim.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Slim
{
    public class ServiceManager : IServiceManager, IServiceProvider
    {
        private Dictionary<Type, (Type, Lifetime)> InterfaceMapping { get; } = new Dictionary<Type, (Type, Lifetime)>();
        private Dictionary<Type, object> Singletons { get; } = new Dictionary<Type, object>();
        private Dictionary<Type, Delegate> Factories { get; } = new Dictionary<Type, Delegate>();

        public void RegisterTransient<TInterface, TClass>()
            where TInterface : class
            where TClass : TInterface
        {
            lock (this)
            {
                this.InterfaceMapping[typeof(TInterface)] = (typeof(TClass), Lifetime.Transient);
            }
        }
        public void RegisterTransient<TInterface, TClass>(Func<IServiceProvider, TClass> serviceFactory)
            where TInterface : class
            where TClass : TInterface
        {
            if (serviceFactory is null) throw new ArgumentNullException(nameof(serviceFactory));

            lock (this)
            {
                this.InterfaceMapping[typeof(TInterface)] = (typeof(TClass), Lifetime.Transient);
                this.Factories[typeof(TInterface)] = serviceFactory;
            }
        }
        public void RegisterSingleton<TInterface, TClass>()
            where TInterface : class
            where TClass : TInterface
        {
            this.InterfaceMapping[typeof(TInterface)] = (typeof(TClass), Lifetime.Singleton);
        }
        public void RegisterSingleton<TInterface, TClass>(Func<IServiceProvider, TClass> serviceFactory)
            where TInterface : class
            where TClass : TInterface
        {
            if (serviceFactory is null) throw new ArgumentNullException(nameof(serviceFactory));

            lock (this)
            {
                this.InterfaceMapping[typeof(TInterface)] = (typeof(TClass), Lifetime.Singleton);
                this.Factories[typeof(TInterface)] = serviceFactory;
            }
        }
        public void RegisterTransient(Type tInterface, Type tClass)
        {
            lock (this)
            {
                this.InterfaceMapping[tInterface] = (tClass, Lifetime.Transient);
            }
        }
        public void RegisterTransient(Type tInterface, Type tClass, Func<IServiceProvider, object> serviceFactory)
        {
            if (serviceFactory is null) throw new ArgumentNullException(nameof(serviceFactory));

            lock (this)
            {
                this.InterfaceMapping[tInterface] = (tClass, Lifetime.Transient);
                this.Factories[tInterface] = serviceFactory;
            }
        }
        public void RegisterSingleton(Type tInterface, Type tClass)
        {
            lock (this)
            {
                this.InterfaceMapping[tInterface] = (tClass, Lifetime.Singleton);
            }
        }
        public void RegisterSingleton<TInterface, TClass>(Type tInterface, Type tClass, Func<IServiceProvider, object> serviceFactory)
        {
            if (serviceFactory is null) throw new ArgumentNullException(nameof(serviceFactory));

            lock (this)
            {
                this.InterfaceMapping[tInterface] = (tClass, Lifetime.Singleton);
                this.Factories[tInterface] = serviceFactory;
            }
        }

        public TInterface GetService<TInterface>() where TInterface : class
        {
            lock (this)
            {
                return GetObject(typeof(TInterface)) as TInterface;
            }
        }
        public object GetService(Type type)
        {
            lock (this)
            {
                return GetObject(type);
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
                    obj = TryImplementService(tInterface);
                }
            }
            else
            {
                obj = TryImplementService(tInterface);
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
                    var parameters = GetParameters(constructor.GetParameters());
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

                obj = TryImplementService(par.ParameterType);
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
