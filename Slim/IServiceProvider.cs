using Slim.Exceptions;

namespace Slim
{
    /// <summary>
    /// Interface allowing to request services from the <see cref="ServiceManager"/>.
    /// </summary>
    public interface IServiceProvider : System.IServiceProvider
    {
        /// <summary>
        /// Resolves and returns the required service.
        /// </summary>
        /// <typeparam name="TInterface">Type of required service.</typeparam>
        /// <returns>Required service.</returns>
        /// <exception cref="DependencyInjectionException">Thrown when unable to resolve required service.</exception>
        TInterface GetService<TInterface>() where TInterface : class;
    }
}
