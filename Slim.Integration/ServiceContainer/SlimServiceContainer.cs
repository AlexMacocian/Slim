using System;
using System.ComponentModel.Design;

namespace Slim.Integration.ServiceContainer;

public class SlimServiceContainer : IServiceContainer
{
    private readonly IServiceManager serviceManager;

    public SlimServiceContainer()
    {
        this.serviceManager = new ServiceManager();
    }

    public SlimServiceContainer(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    public void AddService(Type serviceType, ServiceCreatorCallback callback)
    {
        this.serviceManager.RegisterSingleton(serviceType, sp => callback(this, serviceType));
    }

    public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
    {
        var manager = this.serviceManager;
        if (promote is false)
        {
            this.AddService(serviceType, callback);
            return;
        }

        while (manager is not null)
        {
            manager.RegisterSingleton(serviceType, sp => callback(this, serviceType));
            manager = manager.ParentServiceManager;
        }
    }

    public void AddService(Type serviceType, object serviceInstance)
    {
        this.serviceManager.RegisterSingleton(serviceType, sp => serviceInstance);
    }

    public void AddService(Type serviceType, object serviceInstance, bool promote)
    {
        var manager = this.serviceManager;
        if (promote is false)
        {
            this.AddService(serviceType, serviceInstance);
            return;
        }

        while (manager is not null)
        {
            manager.RegisterSingleton(serviceType, sp => serviceInstance);
            manager = manager.ParentServiceManager;
        }
    }

    public object GetService(Type serviceType)
    {
        return this.serviceManager.GetService(serviceType);
    }

    public void RemoveService(Type serviceType)
    {
        throw new NotSupportedException("Removing one service is not currently supported");
    }

    public void RemoveService(Type serviceType, bool promote)
    {
        throw new NotSupportedException("Removing one service is not currently supported");
    }
}
