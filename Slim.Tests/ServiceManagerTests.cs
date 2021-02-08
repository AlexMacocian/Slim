using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slim.Exceptions;
using Slim.Tests.Models;
using System;

namespace Slim.Tests
{
    [TestClass]
    public class ServiceManagerTests
    {
        [TestMethod]
        public void RetrieveThrowsNotRegistered()
        {
            var di = new ServiceManager();
            Assert.ThrowsException<DependencyInjectionException>(() => { di.GetService<IDependentService>(); });
        }

        [TestMethod]
        public void RegisterAndRetrieveThrowsNoSuitableConstructorFound()
        {
            var di = new ServiceManager();
            di.RegisterSingleton<IDependentService, DependentService>();
            Assert.ThrowsException<DependencyInjectionException>(() => { di.GetService<IDependentService>(); });
        }

        [TestMethod]
        public void RegisterMultipleServices()
        {
            var di = new ServiceManager();
            di.RegisterSingleton<IIndependentService, IndependentService>();
            di.RegisterSingleton<IDependentService, DependentService>();
        }

        [TestMethod]
        public void RegisterAndRetrieveService()
        {
            var di = new ServiceManager();
            di.RegisterSingleton<IDependentService, DependentService>();
            di.RegisterSingleton<IIndependentService, IndependentService>();

            var dependentService = di.GetService<IDependentService>();
            dependentService.Should().NotBeNull();
        }

        [TestMethod]
        public void TransientAreDifferent()
        {
            var di = new ServiceManager();
            di.RegisterTransient<IIndependentService, IndependentService>();

            var independentService = di.GetService<IIndependentService>();
            var independentService2 = di.GetService<IIndependentService>();

            independentService.Should().NotBeSameAs(independentService2);
        }

        [TestMethod]
        public void SingletonAreTheSame()
        {
            var di = new ServiceManager();
            di.RegisterSingleton<IIndependentService, IndependentService>();

            var independentService = di.GetService<IIndependentService>();
            var independentService2 = di.GetService<IIndependentService>();

            independentService.Should().BeSameAs(independentService2);
        }

        [TestMethod]
        public void RegisterServiceWithFactoryAndRetrieve()
        {
            var calledFactory = false;
            var di = new ServiceManager();
            di.RegisterSingleton<IIndependentService, IndependentService>();
            di.RegisterSingleton<IDependentService, DependentService>(sp => 
            {
                calledFactory = true;
                return new DependentService(sp.GetService<IIndependentService>());
            });

            var dependentService = di.GetService<IDependentService>();

            dependentService.Should().NotBeNull();
            calledFactory.Should().BeTrue();
        }

        [TestMethod]
        public void TransientDependentAndSingletonIndependentRespectLifetime()
        {
            var di = new ServiceManager();
            di.RegisterSingleton<IIndependentService, IndependentService>();
            di.RegisterTransient<IDependentService, DependentService>();

            var independentService = di.GetService<IIndependentService>();
            var dependentService = di.GetService<IDependentService>();
            var dependentService2 = di.GetService<IDependentService>();

            dependentService.Should().NotBeSameAs(dependentService2);
            dependentService.IndependentService.Should().BeSameAs(dependentService2.IndependentService);
            independentService.Should().BeSameAs(dependentService.IndependentService);
        }

        [TestMethod]
        public void TransientDependentWithFactoryAndSingletonIndependentRespectLifetime()
        {
            var di = new ServiceManager();
            di.RegisterSingleton<IIndependentService, IndependentService>();
            di.RegisterTransient<IDependentService, DependentService>(sp => new DependentService(sp.GetService<IIndependentService>()));

            var independentService = di.GetService<IIndependentService>();
            var dependentService = di.GetService<IDependentService>();
            var dependentService2 = di.GetService<IDependentService>();

            dependentService.Should().NotBeSameAs(dependentService2);
            dependentService.IndependentService.Should().BeSameAs(dependentService2.IndependentService);
            independentService.Should().BeSameAs(dependentService.IndependentService);
        }

        [TestMethod]
        public void MultipleDeclarationsReturnSameSingleton()
        {
            var di = new ServiceManager();
            di.RegisterSingleton<IIndependentService, IndependentService>();
            di.RegisterSingleton<IndependentService, IndependentService>();

            var independentService = di.GetService<IIndependentService>();
            var sameIndependentService = di.GetService<IndependentService>();

            independentService.Should().BeSameAs(sameIndependentService);
        }

        [TestMethod]
        public void MultipleDeclarationsOfSameInterfaceThrows()
        {
            var di = new ServiceManager();
            di.RegisterSingleton<IIndependentService, IndependentService>();
            
            var action = new Action(() =>
            {
                di.RegisterSingleton<IIndependentService, IndependentService>();
            });

            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void CatchSpecificException()
        {
            var di = new ServiceManager();
            var thrown = false;
            di.HandleException<InvalidOperationException>((sp, e) =>
            {
                thrown = true;
                return false;
            });

            di.RegisterSingleton<IIndependentService, IndependentService>();
            di.RegisterSingleton<IIndependentService, IndependentService>();

            thrown.Should().BeTrue();
        }

        [TestMethod]
        public void CatchSpecificExceptionAndRethrow()
        {
            var di = new ServiceManager();
            var thrown = false;
            di.HandleException<InvalidOperationException>((sp, e) =>
            {
                thrown = true;
                return true;
            });

            di.RegisterSingleton<IIndependentService, IndependentService>();
            var action = new Action(() =>
            {
                di.RegisterSingleton<IIndependentService, IndependentService>();
            });

            action.Should().Throw<InvalidOperationException>();
            thrown.Should().BeTrue();
        }

        [TestMethod]
        public void ClearAndGetShouldThrow()
        {
            var di = new ServiceManager();
            di.RegisterSingleton<IIndependentService, IndependentService>();
            
            var action = new Action(() =>
            {
                di.GetService<IndependentService>();
            });
            di.Clear();

            action.Should().Throw<DependencyInjectionException>();
        }

        [TestMethod]
        public void ClearShouldCallDispose()
        {
            var di = new ServiceManager();
            di.RegisterSingleton<IIDisposableService, IDisposableService>();
            var disposableService = (IDisposableService)di.GetService<IIDisposableService>();
            
            di.Clear();

            disposableService.DisposeCalled.Should().BeTrue();
        }
        
        [TestMethod]
        public void BuildAllSingletons()
        {
            var shouldCallConstructor = false;
            var shouldNotCallConstructor = false;
            var di = new ServiceManager();
            di.RegisterSingleton<IIndependentService, IndependentService>(sp =>
            {
                shouldCallConstructor = true;
                return new IndependentService();
            });
            di.RegisterTransient<IDependentService, DependentService>(sp =>
            {
                shouldNotCallConstructor = true;
                return new DependentService(sp.GetService<IIndependentService>());
            });

            di.BuildSingletons();

            shouldCallConstructor.Should().BeTrue();
            shouldNotCallConstructor.Should().BeFalse();
        }
    }
}
