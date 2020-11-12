namespace Slim.Tests.Models
{
    public interface IDependentService
    {
        IIndependentService IndependentService { get; }
    }
}
