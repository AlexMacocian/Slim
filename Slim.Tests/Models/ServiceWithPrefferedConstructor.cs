using Slim.Attributes;

namespace Slim.Tests.Models
{
    public sealed class ServiceWithPrefferedConstructor
    {
        public bool CalledPrefferedConstructor { get; init; }
        public bool CalledPrefferedConstructor0 { get; init; }
        public bool CalledPrefferedConstructor1 { get; init; }

        public ServiceWithPrefferedConstructor()
        {
        }

        [PreferredConstructor]
        public ServiceWithPrefferedConstructor(IIndependentService independentService)
        {
            this.CalledPrefferedConstructor = true;
        }

        [PreferredConstructor(Priority = 0)]
        public ServiceWithPrefferedConstructor(IDependentService dependentService)
        {
            this.CalledPrefferedConstructor0 = true;
        }

        [PreferredConstructor(Priority = 1)]
        public ServiceWithPrefferedConstructor(IDependentService2 dependentService2)
        {
            this.CalledPrefferedConstructor1 = true;
        }
    }
}
