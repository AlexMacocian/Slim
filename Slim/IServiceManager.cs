﻿using System;

namespace Slim
{
    /// <summary>
    /// Interface for <see cref="ServiceManager"/>.
    /// </summary>
    public interface IServiceManager : IServiceProvider, IServiceProducer
    {
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
}
