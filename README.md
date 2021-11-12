# Slim
## Service lifetime manager and dependency injection framework

Uses lazy approach and only implements objects when called.

Supports multiple Register calls without needing to rebuild.

Supports scoped lifetime of services.

To use, simply create and register types:
```c#
var serviceManager = new ServiceManager();
serviceManager.RegisterSingleton<IService1, Service1>();
serviceManager.RegisterSingleton<IService2, Service2>(sp => new Service2(sp.GetService<IService1>());
serviceManager.GetService<IService2>();
```

To manage the lifetime of the objects, use ``` serviceManager.RegisterSingleton ```, ``` serviceManager.RegisterTransient ``` or ``` serviceManager.RegisterScoped ```.

Can register a services for all interaces it implements:
```c#
serviceManager.RegisterSingleton<Service1>(registerAllInterfaces: true);
serviceManager.GetService<I1Service1>();
serviceManager.GetService<I2Service1>();
```
Otherwise, calling
```c#
serviceManager.RegisterSingleton<Service1>();
```
Will only register Service1 as Service1.

Supports both generics or arguments as types:
```c#
serviceManager.RegisterTransient<IService1, Service1>();
serviceManager.RegisterTransient(typeof(IService2), typeof(Service2));
```

Can register current service manager as a valid dependency:
```c#
serviceManager.RegisterServiceManager();
```

Can handle multiple types of exceptions:
```c#
serviceManager.HandleException<DependencyInjectionException>((sp, ex) => DoSomething1(ex));
serviceManager.HandleException<InvalidOperationException>((sp, ex) => DoSomething2(ex));
serviceManager.HandleException<Exception>((sp, ex) => DoSomething3(ex));
```

In case of reinitialization, call:
```c#
serviceManager.Clear();
```
``` serviceManager.Clear ``` calls ``` IDisposable.Dispose ``` on singletons that implement ``` IDisposable ```.

To build all singletons currently registered, call:
```c#
serviceManager.BuildSingletons();
```

`IDependencyResolver` interface to implement manual resolvers for special cases. Register on the `IServiceManager` using:
```c#
serviceManager.RegisterResolver(new SomeDependencyResolver());
```

To create a scope, call the following method and use the new instance of scoped IServiceProvider.
```c#
serviceManager.CreateScope();
```

By default, the returned scope manager will be readonly. This means, calling
```c#
scopedServiceManager.RegisterService<INewService, NewService>();
```
will throw an InvalidOperationException.

To override the functionality from above, set the `AllowScopedManagerModifications` property to true in the original service manager before creating the scoped service manager
```c#
serviceManager.AllowScopedManagerModifications = true;
var scopedServiceManager = serviceManager.CreateScope();
scopedServiceManager.RegisterService<INewService, NewService>();
```
This way, no exception is thrown.

To retrieve all services of a certain type from the service manager, use
```c#
serviceManager.GetServicesOfType<ISharedInterface>();
```
This will return an `IEnumerable<ISharedInterface>` with all the services which implement the `ISharedInterface`.