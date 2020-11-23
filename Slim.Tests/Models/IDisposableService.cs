namespace Slim.Tests.Models
{
    public class IDisposableService : IIDisposableService
    {
        public bool DisposeCalled { get; set; }

        public void Dispose()
        {
            this.DisposeCalled = true;
        }
    }
}
