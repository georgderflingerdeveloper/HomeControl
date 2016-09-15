using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using Moq;
using NUnit.Framework;
using BASIC_COMPONENTS;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.BASIC_COMPONENTS.Interfaces;
using Phidgets.Events;
using HomeControl.ROOMS;


namespace HomeControl.ROOMS.ANTE_BATH_WASH_ROOM_CONTROLLER.UNIT_TEST
{
    [TestFixture]
    class UnitTest_AnteBathWashRoomController
    {
        static int                     IndexDigitalOutputReserverdForHeartBeat = 15;
        static double                  TestIntervallTimeHeartBeat              = 500;

        AnteBathWashRoomController    _TestAnteBathWashRoomController;
        AnteBathWashRoomConfiguration _TestAnteBathWashRoomConfiguration;
        DeviceBlinker                 _TestHeartBeat = new DeviceBlinker( new BlinkerConfiguration( IndexDigitalOutputReserverdForHeartBeat, StartBlinker.eWithOnPeriode ), new Timer_( TestIntervallTimeHeartBeat ) );
        DigitalInputEventargs         _TestArgs      = new DigitalInputEventargs();
        Mock<IIOHandler>              _MockTestIOHandler;

        public UnitTest_AnteBathWashRoomController( )
        {
            _MockTestIOHandler                 = new Mock<IIOHandler>( );
            _TestAnteBathWashRoomConfiguration = new AnteBathWashRoomConfiguration( );
            _TestAnteBathWashRoomController    = new AnteBathWashRoomController( _TestAnteBathWashRoomConfiguration, _TestHeartBeat, _MockTestIOHandler.Object); 
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


    }
}
