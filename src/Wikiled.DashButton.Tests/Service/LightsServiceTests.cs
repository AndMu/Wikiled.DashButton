using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Threading.Tasks;
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
        private ServiceConfig serviceConfig;

        private Mock<IMonitoringManager> mockMonitoringManager;

        private Mock<ILightsManagerFactory> mockLightsManagerFactory;

        private TestScheduler scheduler;

        private LightsService instance;

        private PacketInformation packet;

        [SetUp]
        public void SetUp()
        {
            packet = new PacketInformation(new VendorInfo("00-11-22-33-44-55", "Amazon"), PhysicalAddress.Parse("00-11-22-33-44-55"));
            serviceConfig = new ServiceConfig();
            serviceConfig.Bridges = new Dictionary<string, BridgeConfig>();
            serviceConfig.Bridges["One"] = new BridgeConfig();
            serviceConfig.Buttons = new Dictionary<string, ButtonConfig>();
            serviceConfig.Buttons["Main"] = new ButtonConfig();
            serviceConfig.Buttons["Main"].Mac = "00-11-22-33-44-55";
            serviceConfig.Buttons["Main"].Actions = new[] { new ButtonAction { Groups = new[] { "TestMain" }, Type = ButtonActionType.Simple } };
            mockMonitoringManager = new Mock<IMonitoringManager>();
            mockLightsManagerFactory = new Mock<ILightsManagerFactory>();
            scheduler = new TestScheduler();
            instance = CreateService();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new LightsService(
                null,
                mockMonitoringManager.Object,
                mockLightsManagerFactory.Object,
                scheduler));

            Assert.Throws<ArgumentNullException>(() => new LightsService(
                serviceConfig,
                null,
                mockLightsManagerFactory.Object,
                scheduler));

            Assert.Throws<ArgumentNullException>(() => new LightsService(
                serviceConfig,
                mockMonitoringManager.Object,
                null,
                scheduler));

            Assert.Throws<ArgumentNullException>(() => new LightsService(
                serviceConfig,
                mockMonitoringManager.Object,
                mockLightsManagerFactory.Object,
                null));
        }

        [Test]
        public void Start()
        {
            Mock<ILightsManager> manager = new Mock<ILightsManager>();
            mockLightsManagerFactory.Setup(item => item.Construct(It.IsAny<BridgeConfig>()))
                                    .Returns(manager.Object);
            var observable = scheduler.CreateHotObservable(
                new Recorded<Notification<PacketInformation>>(0, Notification.CreateOnNext(packet)),
                new Recorded<Notification<PacketInformation>>(1, Notification.CreateOnNext(packet)),
                new Recorded<Notification<PacketInformation>>(TimeSpan.FromSeconds(5).Ticks, Notification.CreateOnNext(packet)));

            mockMonitoringManager.Setup(item => item.StartListening())
                                 .Returns(observable);
            instance.Start();
            var groups = new[] { "TestMain" };
            manager.Setup(item => item.IsAnyOn(groups)).Returns(Task.FromResult(false));
            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(50).Ticks);
            manager.Verify(item => item.TurnGroup(groups, true), Times.Exactly(1));
            scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);
            manager.Verify(item => item.TurnGroup(groups, true), Times.Exactly(2));
        }

        private LightsService CreateService()
        {
            return new LightsService(
                serviceConfig,
                mockMonitoringManager.Object,
                mockLightsManagerFactory.Object,
                scheduler);
        }
    }
}