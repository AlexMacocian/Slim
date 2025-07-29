using System;

namespace Slim.Tests.Models;
public sealed class ThrowingIndependentService : IIndependentService
{
    public ThrowingIndependentService()
    {
        throw new InvalidOperationException();
    }
}
