namespace Slim.Tests.Models;

public class MultiConstructorService : IIndependentService
{
    public int X { get; }
    public int Y { get; }
    public object Z { get; }

    public MultiConstructorService(int x, int y, object z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public MultiConstructorService()
    {
    }
}
