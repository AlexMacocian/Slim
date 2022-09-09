namespace Slim.Tests.Models;

public class DependentService : IDependentService
{
    public DependentService(IIndependentService independentService)
    {
        this.IndependentService = independentService;
    }

    public IIndependentService IndependentService { get; }
}
