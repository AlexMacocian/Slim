using System;
using Microsoft.Extensions.DependencyInjection;

namespace Slim.Integration.ServiceCollection;
public static class ServiceCollectionExtensions
{
    public static System.IServiceProvider BuildSlimServiceProvider(this Microsoft.Extensions.DependencyInjection.ServiceCollection serviceCollection)
    {
        var serviceManager = new ServiceManager();
        foreach(var serviceDescriptor in serviceCollection)
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

    private static void RegisterServiceProviderDependencies(IServiceManager serviceManager)
    {
        serviceManager.RegisterScoped<IServiceScopeFactory, SlimServiceScopeFactory>();
        serviceManager.RegisterScoped<IServiceProviderIsService, SlimServiceProviderIsService>();
    }

    private static void RegisterSingleton(ServiceManager serviceManager, ServiceDescriptor serviceDescriptor)
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

    private static void RegisterScoped(ServiceManager serviceManager, ServiceDescriptor serviceDescriptor)
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

    private static void RegisterTransient(ServiceManager serviceManager, ServiceDescriptor serviceDescriptor)
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
