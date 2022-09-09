using Slim.Attributes;

namespace Slim.Tests.Models;

public sealed class ServiceWithPrefferedConstructor
{
    public bool CalledPrefferedConstructor { get; init; }
    public bool CalledPrefferedConstructor0 { get; init; }
    public bool CalledPrefferedConstructor1 { get; init; }
    public IIndependentService IndependentService { get; }
    public IDependentService DependentService { get; }
    public IDependentService2 DependentService2 { get; }

    public ServiceWithPrefferedConstructor()
    {
    }

    [PreferredConstructor]
    public ServiceWithPrefferedConstructor(IIndependentService independentService)
    {
        this.CalledPrefferedConstructor = true;
        this.IndependentService = independentService;
    }

    [PreferredConstructor(Priority = 0)]
    public ServiceWithPrefferedConstructor(IDependentService dependentService)
    {
        this.CalledPrefferedConstructor0 = true;
        this.DependentService = dependentService;
    }

    [PreferredConstructor(Priority = 1)]
    public ServiceWithPrefferedConstructor(IDependentService2 dependentService2)
    {
        this.CalledPrefferedConstructor1 = true;
        this.DependentService2 = dependentService2;
    }
}
