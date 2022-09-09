using System;
using Microsoft.Extensions.DependencyInjection;

namespace Slim.Integration.ServiceCollection;

internal sealed class SlimServiceScope : IServiceScope
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
