using System;
using System.Collections.Generic;
using System.Linq;
using Slim.Resolvers;

namespace Slim.Integration.ServiceCollection;

internal sealed class IEnumerableResolver : IDependencyResolver
{
    private static readonly Type EnumerableType = typeof(IEnumerable<>);

    public bool CanResolve(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == EnumerableType)
        {
            return true;
        }

        return false;
    }

    public object Resolve(IServiceProvider serviceProvider, Type type)
    {
        var genericArgumentType = type.GetGenericArguments().FirstOrDefault();
        if (genericArgumentType is null)
        {
            throw new InvalidOperationException($"Unable to resolve {type.Name}");
        }

        var list = serviceProvider.GetServicesOfType(genericArgumentType).ToArray();

        // Dynamically invoke IEnumerable.Cast to create an IEnumerable<type>
        var enumerable = typeof(Enumerable)
            .GetMethod("Cast", new[] { typeof(System.Collections.IEnumerable) })
            .MakeGenericMethod(genericArgumentType)
            .Invoke(null, new object[] { list });
        return enumerable;
    }
}
