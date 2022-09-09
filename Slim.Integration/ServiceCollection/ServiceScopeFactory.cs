using System;
using Microsoft.Extensions.DependencyInjection;

namespace Slim.Integration.ServiceCollection;

public sealed class ServiceScopeFactory : IServiceScopeFactory
{
    private readonly IServiceManager serviceManager;

    public ServiceScopeFactory(
        IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    public IServiceScope CreateScope()
    {
        throw new NotImplementedException();
    }
}
