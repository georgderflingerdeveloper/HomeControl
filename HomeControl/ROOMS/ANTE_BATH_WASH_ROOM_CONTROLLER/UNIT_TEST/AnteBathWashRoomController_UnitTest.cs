using Moq;

using BASIC_COMPONENTS;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.BASIC_COMPONENTS.Interfaces;
using LibUdp;
using LibUdp.BASIC.INTERFACE;
using LibUdp.BASIC.RECEIVE;
using HomeAutomationProtocoll;
using HomeControl.BASIC_CONSTANTS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeControl.ADVANCED_COMPONENTS.Interfaces;

namespace HomeControl.ROOMS.ANTE_BATH_WASH_ROOM_CONTROLLER.UNIT_TEST
{
    [TestClass]
    public class UnitTest_AnteBathWashRoomController
    {
        static int                     IndexDigitalOutputReserverdForHeartBeat = 15;
        static double                  TestIntervallTimeHeartBeat              = 500;
        static int                     AnyUnknownScenarioNumber = 999;
        static int                     AnyUnknownIndex = 999;
        static bool                    FakeTrueForTesting = true;
        static bool FakeFalseForTesting = false;

        AnteBathWashRoomController       _TestAnteBathWashRoomController;
        AnteBathWashRoomConfiguration    _TestAnteBathWashRoomConfiguration;
        DeviceBlinker                    _TestHeartBeat = new DeviceBlinker( new BlinkerConfiguration( IndexDigitalOutputReserverdForHeartBeat, StartBlinker.eWithOnPeriode ), new Timer_( TestIntervallTimeHeartBeat ) );
        DigitalInputEventargs            _TestArgs      = new DigitalInputEventargs();
        Mock<IIOHandler>                 _MockTestIOHandler;
        Mock<IUdpBasic>                  _MockUdpCommunicator;
        Mock<IExtendedLightCommander>    _MockLightCommander;
        Mock<AnteBathWashRoomController> _MockAnteBathWashRoomController;
        UpdateEventArgs _TestFeedbackArgs = new UpdateEventArgs();

        public UnitTest_AnteBathWashRoomController( )
        {
            _MockUdpCommunicator               = new Mock<IUdpBasic>( );
            _MockTestIOHandler                 = new Mock<IIOHandler>( );
            _MockLightCommander                = new Mock<IExtendedLightCommander>();
            _TestAnteBathWashRoomConfiguration = new AnteBathWashRoomConfiguration( );
            _TestAnteBathWashRoomController    = new AnteBathWashRoomController( _TestAnteBathWashRoomConfiguration, _TestHeartBeat, _MockTestIOHandler.Object, _MockUdpCommunicator.Object );
            _MockAnteBathWashRoomController    = new Mock<AnteBathWashRoomController>( );
        }


        [TestMethod]
        public void TestBathRoomLight_FirstScenario( )
        {
            // prepare test
            _TestAnteBathWashRoomController.ScenarioNumberBathRoom = 1;
            _TestArgs.Index = IOAssignmentControllerAnteBathWashRoom.indDigitalInputBathRoomMainButton;
            _TestArgs.Value = true;

            // action
            _MockTestIOHandler.Raise( DigitalInputChanged => DigitalInputChanged.EDigitalInputChanged += null, _TestArgs );

            // proove
            Assert.AreEqual( 0, _TestAnteBathWashRoomController.ScenarioNumberBathRoom );
        }

        [TestMethod]
        public void TestAnteRoomLight_FirstScenario( )
        {
            _TestAnteBathWashRoomController.ScenarioNumberAnteRoom = 1;
            _TestArgs.Index = IOAssignmentControllerAnteBathWashRoom.indDigitalInputAnteRoomMainButton;
            _TestArgs.Value = true;

            _MockTestIOHandler.Raise( DigitalInputChanged => DigitalInputChanged.EDigitalInputChanged += null, _TestArgs );

            Assert.AreEqual( 0, _TestAnteBathWashRoomController.ScenarioNumberAnteRoom );
        }

        //[TestMethod]
        //public void TestAnteRoomLight_TURN_LIGHT_ANTEROOM_MAIN_ON_()
        //{
        //    _TestAnteBathWashRoomController.RemoteControl(new DataReceivingEventArgs() { Message = ComandoString.TURN_LIGHT_ANTEROOM_MAIN_ON });
        //    _MockLightCommander.Verify(obj => obj.TurnSingleDevice(TurnDevice.ON, IOAssignmentControllerAnteBathWashRoom
        //                                                .indDigitalOutputAnteRoomMainLight), Times.Once);
        //}

        [TestMethod]
        public void TestAnteRoomLight_TURN_LIGHT_ANTEROOM_MAIN_ON_Received_( )
        {
           _TestFeedbackArgs.Value = FakeFalseForTesting;
           _TestFeedbackArgs.Index = AnyUnknownIndex;
           _TestFeedbackArgs = _TestAnteBathWashRoomController.RemoteControl( new DataReceivingEventArgs( ) { Message = ComandoString.TURN_LIGHT_ANTEROOM_MAIN_ON } );
           Assert.AreEqual(TurnDevice.ON, _TestFeedbackArgs.Value );
           Assert.AreEqual(IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomMainLight,
                           _TestFeedbackArgs.Index);

        }

        [TestMethod]
        public void TestAnteRoomLight_TURN_LIGHT_ANTEROOM_MAIN_OFF_Received_()
        {
            _TestFeedbackArgs.Value = FakeTrueForTesting;
            _TestFeedbackArgs.Index = AnyUnknownIndex;

            _TestFeedbackArgs = _TestAnteBathWashRoomController.RemoteControl(new DataReceivingEventArgs() { Message = ComandoString.TURN_LIGHT_ANTEROOM_MAIN_OFF });
            Assert.AreEqual(TurnDevice.OFF, _TestFeedbackArgs.Value);
            Assert.AreEqual(IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomMainLight,
                            _TestFeedbackArgs.Index);
        }

        [TestMethod]
        public void TestAnteRoomLight_TURN_LIGHT_ANTEROOM_ON_BACK_Received_()
        {
            _TestFeedbackArgs.Value = FakeFalseForTesting;
            _TestFeedbackArgs.Index = AnyUnknownIndex;

            _TestFeedbackArgs = _TestAnteBathWashRoomController.RemoteControl(new DataReceivingEventArgs() { Message = ComandoString.TURN_LIGHT_ANTEROOM_BACK_ON });
            Assert.AreEqual(TurnDevice.ON, _TestFeedbackArgs.Value);
            Assert.AreEqual(IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomBackSide,
                            _TestFeedbackArgs.Index);
        }

        [TestMethod]
        public void TestAnteRoomLight_TURN_LIGHT_ANTEROOM_OFF_BACK_Received_()
        {
            _TestFeedbackArgs.Value = FakeFalseForTesting;
            _TestFeedbackArgs.Index = AnyUnknownIndex;

            _TestFeedbackArgs = _TestAnteBathWashRoomController.RemoteControl(new DataReceivingEventArgs() { Message = ComandoString.TURN_LIGHT_ANTEROOM_BACK_OFF });
            Assert.AreEqual(TurnDevice.OFF, _TestFeedbackArgs.Value);
            Assert.AreEqual(IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomBackSide,
                            _TestFeedbackArgs.Index);
        }


    }
}
