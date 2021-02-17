namespace Slim.Tests.Models
{
    public class DependentOnServiceManagerService : IDependentOnServiceManagerService
    {
        public IServiceManager ServiceManager { get; }
        public IServiceProducer ServiceProducer { get; }
        public IServiceProvider ServiceProvider { get; }

        public DependentOnServiceManagerService(
            IServiceManager serviceManager,
            IServiceProvider serviceProvider,
            IServiceProducer serviceProducer)
        {
            this.ServiceManager = serviceManager;
            this.ServiceProvider = serviceProvider;
            this.ServiceProducer = serviceProducer;
        }
    }
}
