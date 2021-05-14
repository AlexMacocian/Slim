using Slim.Resolvers;
using Slim.Tests.Models;
using System;

namespace Slim.Tests.Resolvers
{
    public sealed class DependentServiceResolver : IDependencyResolver
    {
        public bool CanResolve(Type type)
        {
            if (type == typeof(IDependentService))
            {
                return true;
            }

            return false;
        }

        public object Resolve(IServiceProvider serviceProvider, Type type)
        {
            var independentService = serviceProvider.GetService<IIndependentService>();
            return new DependentService(independentService);
        }
    }
}
