using Moq;
using BASIC_COMPONENTS;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.BASIC_COMPONENTS.Interfaces;
using LibUdp;
using LibUdp.BASIC.INTERFACE;
using LibUdp.BASIC.RECEIVE;
using HomeAutomationProtocoll;
using HomeControl.ROOMS.CONFIGURATION;
using System.Threading.Tasks;
using HomeControl.ADVANCED_COMPONENTS.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace HomeControl.ROOMS.SLEEPING_ROOM.UNIT_TESTS
{
    [TestClass]
    public class SleepingRoomController_UnitTests
    {
        static int AnyUnknownScenarioNumber = 999;
        SleepingRoomController       _TestSleepingRoomController;
        SleepingRoomConfiguration    _TestSleepingRoomConfiguration;
        DigitalInputEventargs        _TestArgs = new DigitalInputEventargs();
        Mock<IIOHandler>             _MockTestIOHandler;
        Mock<IUdpBasic>              _MockUdpCommunicator;
        Mock<IExtendedLightCommander>        _MockLightCommander;
        Mock<IDeviceScenarioControl> _MockDeviceScenarioControl;

        Mock<SleepingRoomController> _MockSleepingRoomController;

        public SleepingRoomController_UnitTests()
        {
            _MockUdpCommunicator       = new Mock<IUdpBasic>();
            _MockTestIOHandler         = new Mock<IIOHandler>();
            _MockLightCommander        = new Mock<IExtendedLightCommander>();
            _MockDeviceScenarioControl = new Mock<IDeviceScenarioControl>();
            _TestSleepingRoomConfiguration = new SleepingRoomConfiguration();

            _TestSleepingRoomController = new SleepingRoomController( _TestSleepingRoomConfiguration,
                                                                      _MockTestIOHandler.Object, 
                                                                      _MockUdpCommunicator.Object,
                                                                      _MockLightCommander.Object,
                                                                      _MockDeviceScenarioControl.Object );
        }

        [TestMethod]
        public void Test_MainButtonLightControl()
        {
            // prepare test
            _TestSleepingRoomController.ScenarioNumber = AnyUnknownScenarioNumber;
            _TestArgs.Index = IOAssignmentControllerSleepingRoom.indDigitalInputMainButton;
            _TestArgs.Value = true;

            // action
            _MockTestIOHandler.Raise(DigitalInputChanged => DigitalInputChanged.EDigitalInputChanged += null, _TestArgs);

            // proove
            _MockLightCommander.Verify(obj => obj.ScenarioTrigger(true), Times.Exactly(1));
        }

        [TestMethod]
        public void Test_FirstScenario()
        {
            // prepare test
            _TestSleepingRoomController.ScenarioNumber = AnyUnknownScenarioNumber;
            _TestArgs.Index = IOAssignmentControllerSleepingRoom.indDigitalInputMainButton;
            _TestArgs.Value = true;

            // action
            _MockTestIOHandler.Raise(DigitalInputChanged => DigitalInputChanged.EDigitalInputChanged += null, _TestArgs);

            // proove
            Assert.AreEqual(0, _TestSleepingRoomController.ScenarioNumber );
        }

    }
}
