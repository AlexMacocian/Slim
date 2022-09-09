using System;
using Microsoft.Extensions.DependencyInjection;

namespace Slim.Integration.ServiceCollection;

internal sealed class SlimServiceProviderIsService : IServiceProviderIsService
{
    private readonly IServiceManager serviceManager;

    public SlimServiceProviderIsService(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    public bool IsService(Type serviceType)
    {
        return this.serviceManager.IsRegistered(serviceType);
    }
}
