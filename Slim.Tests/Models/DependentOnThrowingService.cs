namespace Slim.Tests.Models;

public sealed class DependentOnThrowingService : IDependentOnThrowingService
{
    private readonly IThrowingService throwingService;

    public DependentOnThrowingService(IThrowingService throwingService)
    {
        this.throwingService = throwingService;
    }
}
