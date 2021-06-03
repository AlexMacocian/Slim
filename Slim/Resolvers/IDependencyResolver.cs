using System;

namespace Slim.Resolvers
{
    /// <summary>
    /// Resolver that can be added to a <see cref="IServiceManager"/> to manually resolve a dependency.
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Returns true if the <see cref="IDependencyResolver"/> can resolve the dependency of provided <see cref="Type"/>.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of dependency to be resolved.</param>
        /// <returns>True if <see cref="IDependencyResolver"/> can resolve the dependency. Otherwise false.</returns>
        bool CanResolve(Type type);
        /// <summary>
        /// Returns a resolved dependency of the provided type.
        /// </summary>
        /// <param name="serviceProvider">Reference to the calling <see cref="IServiceProvider"/>.</param>
        /// <param name="type"><see cref="Type"/> of the dependency to be resolved.</param>
        /// <returns>Resolved dependency of provided <see cref="Type"/>.</returns>
        object Resolve(IServiceProvider serviceProvider, Type type);
    }
}
