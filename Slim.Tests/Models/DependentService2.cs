namespace Slim.Tests.Models
{
    public sealed class DependentService2 : IDependentService2
    {
        public IndependentService IndependentService { get; }

        public DependentService2(IndependentService independentService)
        {
            this.IndependentService = independentService;
        }
    }
}
