using System;
using Microsoft.Extensions.DependencyInjection;

namespace Slim.Integration.ServiceCollection;

public sealed class ServiceScope : IServiceScope
{
    public System.IServiceProvider ServiceProvider { get; set; }

    public void Dispose()
    {
        if (this.ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
