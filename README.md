# Slim
## Service lifetime manager and dependency injection framework

Uses lazy approach and only implements objects when called.

Supports multiple Register calls without needing to rebuild.

To use, simply create and register types:
```c#
var serviceManager = new ServiceManager();
serviceManager.RegisterSingleton<IService1, Service1>();
serviceManager.RegisterSingleton<IService2, Service2>(sp => new Service2(sp.GetService<IService1>());
serviceManager.GetService<IService2>();
```

To manage the lifetime of the objects, use ``` serviceManager.RegisterSingleton ``` or ``` serviceManager.RegisterTransient ```.

Supports both generics or arguments as types:
```c#
serviceManager.RegisterTransient<IService1, Service1>();
serviceManager.RegisterTransient(typeof(IService2), typeof(Service2));
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
