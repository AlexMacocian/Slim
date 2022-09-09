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
        IServiceProvider scopedServiceManager = this.serviceManager.CreateScope();
        return new ServiceScope { ServiceProvider = scopedServiceManager };
    }
}
