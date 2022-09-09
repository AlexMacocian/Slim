using System;
using System.Runtime.Serialization;

namespace Slim.Exceptions;

/// <summary>
/// Exception thrown when <see cref="IServiceManager"/> fails to create a service.
/// </summary>
public class DependencyInjectionException : Exception
{
    /// <summary>
    /// Creates a new instance of <see cref="DependencyInjectionException"/>.
    /// </summary>
    public DependencyInjectionException()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="DependencyInjectionException"/>.
    /// </summary>
    /// <param name="message">Message of the exception.</param>
    public DependencyInjectionException(string message) : base(message)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="DependencyInjectionException"/>.
    /// </summary>
    /// <param name="message">Message of the exception.</param>
    /// <param name="innerException">Inner exception.</param>
    public DependencyInjectionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="DependencyInjectionException"/>.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="context">Streaming context.</param>
    protected DependencyInjectionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
