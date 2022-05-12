using Slim.Attributes;

namespace Slim.Tests.Models
{
    public sealed class DoNotInjectConstructorService
    {
        [DoNotInject]
        public DoNotInjectConstructorService()
        {
        }
    }
}
