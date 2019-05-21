using BASIC_COMPONENTS;
using HomeAutomationProtocoll;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.ADVANCED_COMPONENTS.Interfaces;
using HomeControl.BASIC_COMPONENTS.Interfaces;
using HomeControl.BASIC_CONSTANTS;
using HomeControl.ROOMS.CONFIGURATION;
using LibUdp.BASIC.INTERFACE;
using LibUdp.BASIC.RECEIVE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace HomeControl.ROOMS.SLEEPING_ROOM.UNIT_TESTS
{
    [TestClass]
    public class SleepingRoomController_UnitTests
    {
        static int AnyUnknownScenarioNumber = 999;
        SleepingRoomController _TestSleepingRoomController;
        SleepingRoomConfiguration _TestSleepingRoomConfiguration;
        DigitalInputEventargs _TestArgs = new DigitalInputEventargs();
        DataReceivingEventArgs _TestReceivedArgs = new DataReceivingEventArgs();
        Mock<IIOHandler> _MockTestIOHandler;
        Mock<IUdpBasic>        _MockUdpCommunicator;
        Mock<IExtendedLightCommander> _MockLightCommander;
        Mock<IDeviceScenarioControl> _MockDeviceScenarioControl;
        Mock<IHeaterCommander> _MockHeaterCommander;
        UpdateEventArgs _TestFeedbackArgs = new UpdateEventArgs();

        IDeviceControlTimer    ControlTimer;
        IDeviceScenarioControl ScenarioControl;
        IExtendedLightCommander ExtendedLightCommander;

        public SleepingRoomController_UnitTests()
        {
            _MockUdpCommunicator = new Mock<IUdpBasic>();
            _MockTestIOHandler = new Mock<IIOHandler>();
            _MockLightCommander = new Mock<IExtendedLightCommander>();
            _MockDeviceScenarioControl = new Mock<IDeviceScenarioControl>();
            _MockHeaterCommander = new Mock<IHeaterCommander>();
            _TestSleepingRoomConfiguration = new SleepingRoomConfiguration();

            _TestSleepingRoomController = new SleepingRoomController( _TestSleepingRoomConfiguration,
                                                                      _MockTestIOHandler.Object,
                                                                      _MockUdpCommunicator.Object,
                                                                      _MockLightCommander.Object,
                                                                      _MockDeviceScenarioControl.Object,
                                                                      _MockHeaterCommander.Object);

            double TimeAllOn = _TestSleepingRoomConfiguration.
                                                           RoomConfig.
                                                           LightCommanderConfiguration.
                                                           DelayTimeAllOn;

            double TimeNextScenario = _TestSleepingRoomConfiguration.
                                                          RoomConfig.
                                               ScenarioConfiguration.
                                               DelayTimeNextScenario;

            CommanderConfiguration CommanderConfig = _TestSleepingRoomConfiguration.
                                                         RoomConfig.LightCommanderConfiguration;


            ControlTimer           = new DeviceControlTimer( new Timer_(TimeAllOn) );

            ScenarioControl        = new DeviceScenarioControl(0, 
                                                               1, 
                                                               new Timer_(TimeNextScenario),
                                                               new Timer_(0), 
                                                               new Timer_(0) );

            ExtendedLightCommander = new ExtendedLightCommander(CommanderConfig,
                                                                ControlTimer,
                                                                ScenarioControl);
        }

        [TestMethod]
        public void Test_MainButtonLightControl()
        {
            // prepare test
            _TestSleepingRoomController.ScenarioNumber = AnyUnknownScenarioNumber;
            _TestArgs.Index = IOAssignmentControllerSleepingRoom.indDigitalInputMainButton;
            _TestArgs.Value = true;

            // action
            _MockTestIOHandler.Raise(obj => obj.EDigitalInputChanged += null, _TestArgs);

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
            _MockTestIOHandler.Raise(obj => obj.EDigitalInputChanged += null, _TestArgs);

            // proove
            Assert.AreEqual(0, _TestSleepingRoomController.ScenarioNumber);
        }

        [TestMethod]
        public void Test_HeaterOn()
        {
            _TestArgs.Index = IOAssignmentControllerSleepingRoom.indDigitalInputMainButton;
            _TestArgs.Value = true;

            // action
            _MockTestIOHandler.Raise(obj => obj.EDigitalInputChanged += null, _TestArgs);

            _MockHeaterCommander.Verify(obj => obj.MainTrigger(true), Times.Exactly(1));

        }

        [TestMethod]
        public void Test_HeaterOff()
        {
            _TestArgs.Index = IOAssignmentControllerSleepingRoom.indDigitalInputMainButton;

            // action
            _MockTestIOHandler.Raise(obj => obj.EDigitalInputChanged += null, _TestArgs);

            _MockHeaterCommander.Verify(obj => obj.MainTrigger(false), Times.Exactly(1));

        }

        [TestMethod]
        public void Test_TURN_ALL_LIGHTS_ON_Received_()
        {
            _TestSleepingRoomController.RemoteControl(new DataReceivingEventArgs()
            { Message = ComandoString.TURN_ALL_LIGHTS_KIDROOM_ON });

            _MockLightCommander.Verify(obj =>
                                       obj.ScenarioTriggerPersitent(TurnDevice.ON,
                                                                    ScenarioConstantsSleepingRoom.ScenarionAllLights),
                                                                    Times.Exactly(1));
        }

        [TestMethod]
        public void Test_TURN_ALL_LIGHTS_ON_Received_Reset()
        {
            _TestSleepingRoomController.RemoteControl(new DataReceivingEventArgs()
            { Message = ComandoString.TURN_ALL_LIGHTS_KIDROOM_ON });

            _MockLightCommander.Verify(obj =>
                                       obj.Reset(),
                                       Times.Exactly(1));
        }

        [TestMethod]
        public void Test_TURN_ALL_LIGHTS_OFF_Received_()
        {
            _TestSleepingRoomController.RemoteControl(new DataReceivingEventArgs()
            { Message = ComandoString.TURN_ALL_LIGHTS_KIDROOM_OFF });

            _MockLightCommander.Verify(obj =>
                                       obj.ScenarioTriggerPersitent(TurnDevice.OFF,
                                                                    ScenarioConstantsSleepingRoom.ScenarionAllLights),
                                                                    Times.Exactly(1));
        }

        [TestMethod]
        public void Test_TURN_LIGHT1_ON_Received_()
        {
            _TestSleepingRoomController.RemoteControl(new DataReceivingEventArgs()
            { Message = ComandoString.TURN_LIGHT_KIDROOM1_ON });

            _MockLightCommander.Verify(obj =>
                                       obj.TurnSingleDevice(TurnDevice.ON,
                                                            IOAssignmentControllerSleepingRoom.indDigitalOutputLightCeiling),
                                                            Times.Exactly(1));
        }

        [TestMethod]
        public void Test_TURN_LIGHT1_OFF_Received_()
        {
            _TestSleepingRoomController.RemoteControl(new DataReceivingEventArgs()
            { Message = ComandoString.TURN_LIGHT_KIDROOM1_OFF });

            _MockLightCommander.Verify(obj =>
                                       obj.TurnSingleDevice(TurnDevice.OFF,
                                                            IOAssignmentControllerSleepingRoom.indDigitalOutputLightCeiling),
                                                            Times.Exactly(1));
        }

        [TestMethod]
        public void Test_ReceivedData()
        {
            _TestReceivedArgs.Message = "Hello";
            _MockUdpCommunicator.Raise(obj => obj.EDataReceived += null, _TestReceivedArgs);
            Assert.AreEqual("Hello", _TestSleepingRoomController.FeedbackReceivedArgs.Message);
        }

    }
}
