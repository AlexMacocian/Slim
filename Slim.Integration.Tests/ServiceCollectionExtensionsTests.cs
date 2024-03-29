﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Slim.Integration.ServiceCollection;
using Slim.Integration.Tests.Models;

namespace Slim.Integration.Tests;

[TestClass]
public sealed class ServiceCollectionExtensionsTests
{
    [TestMethod]
    public void BuildSlimServiceProvider_ReturnsServiceProvider()
    {
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();

        serviceProvider.Should().BeAssignableTo<System.IServiceProvider>();
    }

    [TestMethod]
    public void BuildSlimServiceProvider_WithServices_ReturnsProviderWithServices()
    {
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        ServiceCollectionServiceExtensions.AddScoped<ITestService, TestService>(serviceCollection);
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();

        var testService = serviceProvider.GetRequiredService<ITestService>();

        testService.Should().BeOfType<TestService>();
    }

    [TestMethod]
    public void BuildSlimServiceProvider_WithSingletonInstance_ReturnsSingletonInstance()
    {
        var testService = new TestService();

        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        ServiceCollectionServiceExtensions.AddSingleton<ITestService>(serviceCollection, testService);
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();

        var returnedService = serviceProvider.GetRequiredService<ITestService>();

        returnedService.Should().Be(testService);
    }

    [TestMethod]
    public void BuildSlimServiceProvider_WithSingletonFactory_ReturnsSingletonInstance()
    {
        var testService = new TestService();

        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        ServiceCollectionServiceExtensions.AddSingleton<ITestService>(serviceCollection, sp => testService);
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();

        var returnedService = serviceProvider.GetRequiredService<ITestService>();

        returnedService.Should().Be(testService);
    }

    [TestMethod]
    public void BuildSlimServiceProvider_WithScopedFactory_ReturnsScopedInstance()
    {
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        ServiceCollectionServiceExtensions.AddScoped<ITestService>(serviceCollection, sp => new TestService());
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();
        var scopedServiceProvider = serviceProvider.CreateScope().ServiceProvider;

        var returnedService1 = serviceProvider.GetRequiredService<ITestService>();
        var returnedService11 = serviceProvider.GetRequiredService<ITestService>();
        var returnedService2 = scopedServiceProvider.GetRequiredService<ITestService>();
        var returnedService21 = scopedServiceProvider.GetRequiredService<ITestService>();

        returnedService1.Should().NotBe(returnedService2);
        returnedService11.Should().Be(returnedService1);
        returnedService21.Should().Be(returnedService2);
    }

    [TestMethod]
    public void BuildSlimServiceProvider_WithScopedImplementation_ReturnsScopedInstance()
    {
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        ServiceCollectionServiceExtensions.AddScoped<ITestService, TestService>(serviceCollection);
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();
        var scopedServiceProvider = serviceProvider.CreateScope().ServiceProvider;

        var returnedService1 = serviceProvider.GetRequiredService<ITestService>();
        var returnedService11 = serviceProvider.GetRequiredService<ITestService>();
        var returnedService2 = scopedServiceProvider.GetRequiredService<ITestService>();
        var returnedService21 = scopedServiceProvider.GetRequiredService<ITestService>();

        returnedService1.Should().NotBe(returnedService2);
        returnedService11.Should().Be(returnedService1);
        returnedService21.Should().Be(returnedService2);
    }

    [TestMethod]
    public void BuildSlimServiceProvider_WithTransientFactory_ReturnsTransientInstance()
    {
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        ServiceCollectionServiceExtensions.AddTransient<ITestService, TestService>(serviceCollection, sp => new TestService());
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();
        var scopedServiceProvider = serviceProvider.CreateScope().ServiceProvider;

        var returnedService1 = serviceProvider.GetRequiredService<ITestService>();
        var returnedService11 = serviceProvider.GetRequiredService<ITestService>();
        var returnedService2 = scopedServiceProvider.GetRequiredService<ITestService>();
        var returnedService21 = scopedServiceProvider.GetRequiredService<ITestService>();

        returnedService1.Should().NotBe(returnedService2);
        returnedService11.Should().NotBe(returnedService1);
        returnedService21.Should().NotBe(returnedService2);
    }

    [TestMethod]
    public void BuildSlimServiceProvider_WithTransientImplementation_ReturnsTransientInstance()
    {
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        ServiceCollectionServiceExtensions.AddTransient<ITestService, TestService>(serviceCollection);
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();
        var scopedServiceProvider = serviceProvider.CreateScope().ServiceProvider;

        var returnedService1 = serviceProvider.GetRequiredService<ITestService>();
        var returnedService11 = serviceProvider.GetRequiredService<ITestService>();
        var returnedService2 = scopedServiceProvider.GetRequiredService<ITestService>();
        var returnedService21 = scopedServiceProvider.GetRequiredService<ITestService>();

        returnedService1.Should().NotBe(returnedService2);
        returnedService11.Should().NotBe(returnedService1);
        returnedService21.Should().NotBe(returnedService2);
    }

    [TestMethod]
    public void BuildSlimServiceProvider_CreateScope_ReturnsScopedServiceProvider()
    {
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();
        var scopedServiceProvider = serviceProvider.CreateScope().ServiceProvider;

        serviceProvider.Should().NotBe(scopedServiceProvider);
        scopedServiceProvider.Should().NotBeNull();
        scopedServiceProvider.Should().BeOfType<ServiceManager>();
        serviceProvider.Should().BeOfType<ServiceManager>();
        scopedServiceProvider.As<ServiceManager>().ParentServiceManager.Should().Be(serviceProvider);
    }

    [TestMethod]
    public void BuildSlimServiceProvider_SupportsSlimServiceProviderIsService_ReturnsTrueForRegisteredService()
    {
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        ServiceCollectionServiceExtensions.AddTransient<ITestService, TestService>(serviceCollection);
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();

        var isServiceApi = serviceProvider.GetRequiredService<IServiceProviderIsService>();

        isServiceApi.IsService(typeof(ITestService)).Should().BeTrue();
    }

    [TestMethod]
    public void BuildSlimServiceProvider_SupportsSlimServiceProviderIsService_ReturnsFalseForRegisteredService()
    {
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        ServiceCollectionServiceExtensions.AddTransient<ITestService, TestService>(serviceCollection);
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();

        var isServiceApi = serviceProvider.GetRequiredService<IServiceProviderIsService>();

        isServiceApi.IsService(typeof(TestService)).Should().BeFalse();
        isServiceApi.IsService(typeof(object)).Should().BeFalse();
    }

    [TestMethod]
    public void BuildSlimServiceProvider_GetServices_ReturnsExpectedServices()
    {
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        ServiceCollectionServiceExtensions.AddTransient<TestService>(serviceCollection);
        ServiceCollectionServiceExtensions.AddTransient<TestService2>(serviceCollection);
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();

        var services = serviceProvider.GetServices<ITestService>();

        services.Should().HaveCount(2);
    }

    [TestMethod]
    public void BuildSlimServiceProvider_GetServicesNoServices_ReturnsNoServices()
    {
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();

        var services = serviceProvider.GetServices<ITestService>();

        services.Should().HaveCount(0);
    }

    [TestMethod]
    public void BuildSlimServiceProvider_WithExistingServiceManager_UsesExistingServiceManager()
    {
        var serviceManager = new ServiceManager();
        serviceManager.RegisterSingleton<ITestService, TestService>();
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        var serviceProvider = serviceCollection.BuildSlimServiceProvider(serviceManager);

        var testService = serviceProvider.GetRequiredService<ITestService>();

        testService.Should().NotBeNull();
    }
}
