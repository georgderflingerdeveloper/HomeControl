using Moq;
using NUnit.Framework;
using BASIC_COMPONENTS;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.BASIC_COMPONENTS.Interfaces;
using LibUdp;
using LibUdp.BASIC.INTERFACE;
using LibUdp.BASIC.RECEIVE;
using HomeAutomationProtocoll;
using HomeControl.ROOMS.CONFIGURATION;
using System.Threading.Tasks;

namespace HomeControl.ROOMS.SLEEPING_ROOM.UNIT_TESTS
{
    [TestFixture]
    class SleepingRoomController_UnitTests
    {
        static int AnyUnknownScenarioNumber = 999;
        SleepingRoomController       _TestSleepingRoomController;
        SleepingRoomConfiguration    _TestSleepingRoomConfiguration;
        DigitalInputEventargs        _TestArgs = new DigitalInputEventargs();
        Mock<IIOHandler>             _MockTestIOHandler;
        Mock<IUdpBasic>              _MockUdpCommunicator;
        Mock<SleepingRoomController> _MockSleepingRoomController;

        public SleepingRoomController_UnitTests()
        {
            _MockUdpCommunicator = new Mock<IUdpBasic>();
            _MockTestIOHandler   = new Mock<IIOHandler>();
            _TestSleepingRoomConfiguration = new SleepingRoomConfiguration();
            _TestSleepingRoomController = new SleepingRoomController(_TestSleepingRoomConfiguration, _MockTestIOHandler.Object, _MockUdpCommunicator.Object);
        }

        [Test]
        public void Test_FirstScenario()
        {
            // prepare test
            _TestSleepingRoomController.ScenarioNumber = AnyUnknownScenarioNumber;
            _TestArgs.Index = IOAssignmentControllerAnteBathWashRoom.indDigitalInputBathRoomMainButton;
            _TestArgs.Value = true;

            // action
            _MockTestIOHandler.Raise(DigitalInputChanged => DigitalInputChanged.EDigitalInputChanged += null, _TestArgs);

            // proove
            Assert.AreEqual(0, _TestSleepingRoomController.ScenarioNumber );
        }

    }
}
