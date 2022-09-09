namespace Slim.Tests.Models;

public sealed class DependentOnPrivateCtrService : IDependentOnPrivateCtrService
{
    private readonly IPrivateConstructorService privateConstructorService;

    public DependentOnPrivateCtrService(
        IPrivateConstructorService privateConstructorService)
    {
        this.privateConstructorService = privateConstructorService;
    }
}
