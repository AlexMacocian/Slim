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
    public void BuildSlimServiceProvider_WithScopedInstance_ReturnsScopedInstance()
    {
        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        ServiceCollectionServiceExtensions.AddScoped<ITestService>(serviceCollection, sp => new TestService());
        var serviceProvider = serviceCollection.BuildSlimServiceProvider();
        var scopedServiceProvider = serviceProvider.CreateScope().ServiceProvider;

        var returnedService1 = serviceProvider.GetRequiredService<ITestService>();
        var returnedService2 = scopedServiceProvider.GetRequiredService<ITestService>();

        returnedService.Should().Be(testService);
    }
}
