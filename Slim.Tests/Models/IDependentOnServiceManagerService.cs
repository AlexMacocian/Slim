namespace Slim.Tests.Models;

public interface IDependentOnServiceManagerService
{
    IServiceManager ServiceManager { get; }
    IServiceProducer ServiceProducer { get; }
    IServiceProvider ServiceProvider { get; }
}
