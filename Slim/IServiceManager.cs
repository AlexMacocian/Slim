using System;

namespace Slim
{
    public interface IServiceManager : IServiceProvider
    {
        void RegisterTransient<TInterface, TClass>()
            where TInterface : class
            where TClass : TInterface;

        void RegisterTransient<TInterface, TClass>(Func<IServiceProvider, TClass> serviceFactory)
            where TInterface : class
            where TClass : TInterface;

        void RegisterSingleton<TInterface, TClass>()
            where TInterface : class
            where TClass : TInterface;

        void RegisterSingleton<TInterface, TClass>(Func<IServiceProvider, TClass> serviceFactory)
            where TInterface : class
            where TClass : TInterface;

        void RegisterTransient(Type tInterface, Type tClass);

        void RegisterTransient(Type tInterface, Type tClass, Func<IServiceProvider, object> serviceFactory);

        void RegisterSingleton(Type tInterface, Type tClass);

        void RegisterSingleton<TInterface, TClass>(Type tInterface, Type tClass, Func<IServiceProvider, object> serviceFactory);
    }
}
