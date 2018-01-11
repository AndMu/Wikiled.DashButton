using System;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Wikiled.DashButton.Config;
using Wikiled.DashButton.Lights;
using Wikiled.DashButton.Monitor;
using Wikiled.DashButton.Service;

namespace Wikiled.DashButton.Tests.Service
{
    [TestFixture]
    public class LightsServiceTests : ReactiveTest
    {
        private Mock<ServiceConfig> mockServiceConfig;

        private Mock<IMonitoringManager> mockMonitoringManager;

        private Mock<ILightsManagerFactory> mockLightsManagerFactory;

        private TestScheduler scheduler;

        private LightsService instance;

        [SetUp]
        public void SetUp()
        {
            mockServiceConfig = new Mock<ServiceConfig>();
            mockMonitoringManager = new Mock<IMonitoringManager>();
            mockLightsManagerFactory = new Mock<ILightsManagerFactory>();
            scheduler = new TestScheduler();
            instance = CreateService();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new LightsService(
                mockServiceConfig.Object,
                mockMonitoringManager.Object,
                mockLightsManagerFactory.Object,
                scheduler));
        }

        private LightsService CreateService()
        {
            return new LightsService(
                mockServiceConfig.Object,
                mockMonitoringManager.Object,
                mockLightsManagerFactory.Object,
                scheduler);
        }
    }
}