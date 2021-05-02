namespace Slim.Tests.Models
{
    public class MultiConstructorService : IIndependentService
    {
        public MultiConstructorService(int x, int y, object z)
        {
        }

        public MultiConstructorService()
        {
        }
    }
}
