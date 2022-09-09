using Microsoft.Extensions.DependencyInjection;

namespace Slim.Integration.ServiceCollection;

internal sealed class SlimServiceScopeFactory : IServiceScopeFactory
{
    private readonly IServiceManager serviceManager;

    public SlimServiceScopeFactory(
        IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    public IServiceScope CreateScope()
    {
        IServiceProvider scopedServiceManager = this.serviceManager.CreateScope();
        return new SlimServiceScope { ServiceProvider = scopedServiceManager };
    }
}
