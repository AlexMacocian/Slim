namespace Slim.Tests.Models;

public sealed class IDisposableSingletonService : IIDisposableService
{
    public bool DisposeCalled { get; set; }

    public void Dispose()
    {
        this.DisposeCalled = true;
    }
}
