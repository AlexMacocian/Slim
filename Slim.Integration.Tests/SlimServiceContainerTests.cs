using System.ComponentModel.Design;
using FluentAssertions;
using Slim.Integration.ServiceContainer;
using Slim.Integration.Tests.Models;

namespace Slim.Integration.Tests;

[TestClass]
public class SlimServiceContainerTests
{
    private readonly SlimServiceContainer slimServiceContainer = new();

    [TestMethod]
    public void AddService_GetService_ReturnsExpectedService()
    {
        var testService = new TestService();

        this.slimServiceContainer.AddService(typeof(ITestService), testService);
        var retrievedService = this.slimServiceContainer.GetService(typeof(ITestService));

        retrievedService.Should().BeAssignableTo<ITestService>();
        retrievedService.Should().Be(testService);
    }

    [TestMethod]
    public void AddServiceWithPromotion_GetService_ReturnsServiceFromParent()
    {
        var testService = new TestService();
        var parent = new ServiceManager() { AllowScopedManagerModifications = true };
        var child = parent.CreateScope().As<ServiceManager>();
        var container = new SlimServiceContainer(child);

        container.AddService(typeof(ITestService), testService, true);
        var retrievedService = parent.GetService<ITestService>();

        retrievedService.Should().BeAssignableTo<ITestService>();
        retrievedService.Should().Be(testService);
    }

    [TestMethod]
    public void AddServiceWithCallback_GetService_ReturnsExpectedService()
    {
        var testService = new TestService();

        this.slimServiceContainer.AddService(typeof(ITestService), new ServiceCreatorCallback((sc, type) => testService));
        var retrievedService = this.slimServiceContainer.GetService(typeof(ITestService));

        retrievedService.Should().BeAssignableTo<ITestService>();
        retrievedService.Should().Be(testService);
    }

    [TestMethod]
    public void AddServiceWithCallbackWithPromotion_GetService_ReturnsServiceFromParent()
    {
        var testService = new TestService();
        var parent = new ServiceManager() { AllowScopedManagerModifications = true };
        var child = parent.CreateScope().As<ServiceManager>();
        var container = new SlimServiceContainer(child);

        container.AddService(typeof(ITestService), new ServiceCreatorCallback((sc, type) => testService), true);
        var retrievedService = parent.GetService<ITestService>();

        retrievedService.Should().BeAssignableTo<ITestService>();
        retrievedService.Should().Be(testService);
    }

    [TestMethod]
    public void RemoveService_NotSupported()
    {
        var action =  () => this.slimServiceContainer.RemoveService(typeof(ITestService));

        action.Should().Throw<NotSupportedException>();
    }

    [TestMethod]
    public void RemoveServiceWithPromotion_NotSupported()
    {
        var action = () => this.slimServiceContainer.RemoveService(typeof(ITestService), true);

        action.Should().Throw<NotSupportedException>();
    }
}
