using System;

namespace Slim
{
    public interface IServiceProvider
    {
        TInterface GetService<TInterface>() where TInterface : class;
        object GetService(Type type);
    }
}
