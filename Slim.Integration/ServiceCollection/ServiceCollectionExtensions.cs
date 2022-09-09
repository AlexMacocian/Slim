using System;
using Microsoft.Extensions.DependencyInjection;

namespace Slim.Integration.ServiceCollection;
public static class ServiceCollectionExtensions
{
    public static System.IServiceProvider BuildSlimServiceProvider(this IServiceCollection serviceCollection, IServiceManager serviceManager)
    {
        _ = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        _ = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));

        foreach (var serviceDescriptor in serviceCollection)
        {
            switch (serviceDescriptor.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    RegisterSingleton(serviceManager, serviceDescriptor);
                    break;
                case ServiceLifetime.Scoped:
                    RegisterScoped(serviceManager, serviceDescriptor);
                    break;
                case ServiceLifetime.Transient:
                    RegisterTransient(serviceManager, serviceDescriptor);
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected service lifetime {serviceDescriptor.Lifetime}.");
            }
        }

        RegisterServiceProviderDependencies(serviceManager);
        return serviceManager;
    }

    public static System.IServiceProvider BuildSlimServiceProvider(this IServiceCollection serviceCollection)
    {
        _ = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));

        var serviceManager = new ServiceManager();
        return serviceCollection.BuildSlimServiceProvider(serviceManager);
    }

    private static void RegisterServiceProviderDependencies(IServiceManager serviceManager)
    {
        serviceManager.RegisterResolver(new IEnumerableResolver());
        serviceManager.RegisterScoped<IServiceScopeFactory, SlimServiceScopeFactory>();
        serviceManager.RegisterScoped<IServiceProviderIsService, SlimServiceProviderIsService>();
    }

    private static void RegisterSingleton(IServiceManager serviceManager, ServiceDescriptor serviceDescriptor)
    {
        if (serviceDescriptor.ImplementationInstance is null &&
            serviceDescriptor.ImplementationFactory is not null)
        {
            serviceManager.RegisterSingleton(serviceDescriptor.ServiceType, sp => serviceDescriptor.ImplementationFactory(sp));
        }
        else if (serviceDescriptor.ImplementationFactory is null &&
            serviceDescriptor.ImplementationInstance is not null)
        {
            serviceManager.RegisterSingleton(serviceDescriptor.ServiceType, sp => serviceDescriptor.ImplementationInstance);
        }
        else
        {
            serviceManager.RegisterSingleton(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType);
        }
    }

    private static void RegisterScoped(IServiceManager serviceManager, ServiceDescriptor serviceDescriptor)
    {
        if (serviceDescriptor.ImplementationInstance is null &&
            serviceDescriptor.ImplementationFactory is not null)
        {
            serviceManager.RegisterScoped(serviceDescriptor.ServiceType, sp => serviceDescriptor.ImplementationFactory(sp));
        }
        else if (serviceDescriptor.ImplementationFactory is null &&
            serviceDescriptor.ImplementationInstance is not null)
        {
            serviceManager.RegisterScoped(serviceDescriptor.ServiceType, sp => serviceDescriptor.ImplementationInstance);
        }
        else
        {
            serviceManager.RegisterScoped(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType);
        }
    }

    private static void RegisterTransient(IServiceManager serviceManager, ServiceDescriptor serviceDescriptor)
    {
        if (serviceDescriptor.ImplementationInstance is null &&
            serviceDescriptor.ImplementationFactory is not null)
        {
            serviceManager.RegisterTransient(serviceDescriptor.ServiceType, sp => serviceDescriptor.ImplementationFactory(sp));
        }
        else if (serviceDescriptor.ImplementationFactory is null &&
            serviceDescriptor.ImplementationInstance is not null)
        {
            serviceManager.RegisterTransient(serviceDescriptor.ServiceType, sp => serviceDescriptor.ImplementationInstance);
        }
        else
        {
            serviceManager.RegisterTransient(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType);
        }
    }
}
