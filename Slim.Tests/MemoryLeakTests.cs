using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slim.Tests.Models;
using System;
using System.Threading.Tasks;

namespace Slim.Tests
{
    [TestClass]
    public sealed class MemoryLeakTests
    {
        private const int ScopesCount = 10000;

        [TestMethod]
        public async Task CreateAndReleaseScopesDisposeManagerGCCollects()
        {
            var di = new ServiceManager();
            di.RegisterSingleton<IIndependentService, IndependentService>();
            di.RegisterScoped<IDependentService, DependentService>();
            var independentService = di.GetService<IIndependentService>();
            var dependentService = di.GetService<IDependentService>();

            var weakRefs = new WeakReference[ScopesCount];
            for (int i = 0; i < ScopesCount; i++)
            {
                var scopedDi = di.CreateScope();
                weakRefs[i] = new WeakReference(scopedDi);
                var scopedDependentService = scopedDi.GetService<IDependentService>();
                scopedDependentService.Should().NotBeNull();
                scopedDependentService.Should().NotBe(dependentService);
                scopedDependentService.IndependentService.Should().NotBeNull();
                scopedDependentService.IndependentService.Should().Be(independentService);
            }

            await Task.Delay(1500);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            foreach(var weakRef in weakRefs)
            {
                weakRef.IsAlive.Should().BeFalse();
            }
        }
    }
}
