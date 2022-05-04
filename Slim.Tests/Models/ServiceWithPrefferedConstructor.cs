using Slim.Attributes;

namespace Slim.Tests.Models
{
    public sealed class ServiceWithPrefferedConstructor
    {
        public bool CalledPrefferedConstructor { get; init; }

        public ServiceWithPrefferedConstructor()
        {
        }

        [PrefferedConstructor]
        public ServiceWithPrefferedConstructor(IIndependentService independentService)
        {
            this.CalledPrefferedConstructor = true;
        }
    }
}
