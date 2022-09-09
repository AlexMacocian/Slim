using System;

namespace Slim.Tests.Models;

public sealed class ThrowingService : IThrowingService
{
    public ThrowingService()
    {
        throw new InvalidOperationException();
    }
}
