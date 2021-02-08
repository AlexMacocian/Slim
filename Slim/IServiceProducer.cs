using System;

namespace Slim
{
    /// <summary>
    /// Interface allowing to produce services for the <see cref="ServiceManager"/>.
    /// </summary>
    public interface IServiceProducer
    {
        /// <summary>
        /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
        /// </summary>
        /// <typeparam name="TInterface">Type of interface.</typeparam>
        /// <typeparam name="TClass">Type of implementation.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
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
        /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
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
        /// Register a service into <see cref="ServiceManager"/> with <see cref="Lifetime.Transient"/>.
        /// </summary>
        /// <param name="tInterface">Type of interface.</param>
        /// <param name="tClass">Type of implementation.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
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
        /// <exception cref="ArgumentNullException">Thrown when the provided serviceFactory is null.</exception>
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
    }
}
