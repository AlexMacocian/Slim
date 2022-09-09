using System;

namespace Slim.Tests.Models;

public interface IIDisposableService : IDisposable
{
    bool DisposeCalled { get; set; }
}
