using System;

namespace Slim.Resolvers
{
    public interface IDependencyResolver
    {
        bool CanResolve(Type type);
        object Resolve(Type type);
    }
}
