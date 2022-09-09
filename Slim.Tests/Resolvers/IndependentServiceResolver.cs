using Slim.Resolvers;
using Slim.Tests.Models;
using System;

namespace Slim.Tests.Resolvers;

public class IndependentServiceResolver : IDependencyResolver
{
    public int Called { get; private set; }

    public bool CanResolve(Type type)
    {
        if (type == typeof(IIndependentService))
        {
            return true;
        }

        return false;
    }

    public object Resolve(IServiceProvider serviceProvider, Type type)
    {
        this.Called++;
        return new IndependentService();
    }
}
