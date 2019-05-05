using Moq;
using NUnit.Framework;
using BASIC_COMPONENTS;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.BASIC_COMPONENTS.Interfaces;
using LibUdp;
using LibUdp.BASIC.INTERFACE;
using LibUdp.BASIC.RECEIVE;
using HomeAutomationProtocoll;
using HomeControl.BASIC_CONSTANTS;


namespace HomeControl.ROOMS.ANTE_BATH_WASH_ROOM_CONTROLLER.UNIT_TEST
{
    [TestFixture]
    class UnitTest_AnteBathWashRoomController
    {
        static int                     IndexDigitalOutputReserverdForHeartBeat = 15;
        static double                  TestIntervallTimeHeartBeat              = 500;
        static int                     AnyUnknownScenarioNumber = 999;
        static int                     AnyUnknownIndex = 999;
        static bool FakeTrueForTesting = true;

        AnteBathWashRoomController _TestAnteBathWashRoomController;
        AnteBathWashRoomConfiguration    _TestAnteBathWashRoomConfiguration;
        DeviceBlinker                    _TestHeartBeat = new DeviceBlinker( new BlinkerConfiguration( IndexDigitalOutputReserverdForHeartBeat, StartBlinker.eWithOnPeriode ), new Timer_( TestIntervallTimeHeartBeat ) );
        DigitalInputEventargs            _TestArgs      = new DigitalInputEventargs();
        Mock<IIOHandler>                 _MockTestIOHandler;
        Mock<IUdpBasic>                  _MockUdpCommunicator;
        Mock<AnteBathWashRoomController> _MockAnteBathWashRoomController;
        UpdateEventArgs _TestFeedbackArgs = new UpdateEventArgs();

        public UnitTest_AnteBathWashRoomController( )
        {
            _MockUdpCommunicator               = new Mock<IUdpBasic>( );
            _MockTestIOHandler                 = new Mock<IIOHandler>( );
            _TestAnteBathWashRoomConfiguration = new AnteBathWashRoomConfiguration( );
            _TestAnteBathWashRoomController    = new AnteBathWashRoomController( _TestAnteBathWashRoomConfiguration, _TestHeartBeat, _MockTestIOHandler.Object, _MockUdpCommunicator.Object );
            _MockAnteBathWashRoomController    = new Mock<AnteBathWashRoomController>( );
        }


        [Test]
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

        [Test]
        public void TestAnteRoomLight_FirstScenario( )
        {
            _TestAnteBathWashRoomController.ScenarioNumberAnteRoom = 1;
            _TestArgs.Index = IOAssignmentControllerAnteBathWashRoom.indDigitalInputAnteRoomMainButton;
            _TestArgs.Value = true;

            _MockTestIOHandler.Raise( DigitalInputChanged => DigitalInputChanged.EDigitalInputChanged += null, _TestArgs );

            Assert.AreEqual( 0, _TestAnteBathWashRoomController.ScenarioNumberAnteRoom );
        }

        [Test]
        public void TestAnteRoomLight_TURN_LIGHT_ANTEROOM_MAIN_ON_Received_( )
        {
           _TestFeedbackArgs = _TestAnteBathWashRoomController.RemoteControl( new DataReceivingEventArgs( ) { Message = ComandoString.TURN_LIGHT_ANTEROOM_MAIN_ON } );
           Assert.AreEqual(TurnDevice.ON, _TestFeedbackArgs.Value );
           Assert.AreEqual(IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomMainLight,
                           _TestFeedbackArgs.Index);

        }

        [Test]
        public void TestAnteRoomLight_TURN_LIGHT_ANTEROOM_MAIN_OFF_Received_()
        {
            _TestFeedbackArgs.Value = FakeTrueForTesting;
            _TestFeedbackArgs.Index = AnyUnknownIndex;

            _TestFeedbackArgs = _TestAnteBathWashRoomController.RemoteControl( new DataReceivingEventArgs( ) { Message = ComandoString.TURN_LIGHT_ANTEROOM_MAIN_OFF } );
            Assert.AreEqual(TurnDevice.OFF, _TestFeedbackArgs.Value);
            Assert.AreEqual(IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomMainLight,
                            _TestFeedbackArgs.Index);
        }

    }
}
