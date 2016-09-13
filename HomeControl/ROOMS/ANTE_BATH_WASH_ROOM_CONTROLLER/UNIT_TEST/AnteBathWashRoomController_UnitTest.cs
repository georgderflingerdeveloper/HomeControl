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

        Mock<IIOHandler> _MockTestIOHandler = new Mock<IIOHandler>();

        public UnitTest_AnteBathWashRoomController( )
        {
            _TestAnteBathWashRoomConfiguration = new AnteBathWashRoomConfiguration( );
            _TestAnteBathWashRoomController    = new AnteBathWashRoomController( _TestAnteBathWashRoomConfiguration, _TestHeartBeat, _MockTestIOHandler.Object); 
        }

        //_MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );

        [Test]
        public void TestBathRoomLight_FirstScenario( )
        {
           //_MockTestIOHandler.Raise( DigitalInputChanged => DigitalInputChanged.EDigitalInputChanged += null, new EventArgs() as DigitalInputEventargs );
           Assert.AreEqual( 0, _TestAnteBathWashRoomController.ScenarioNumberBathRoom );
        }
    }
}
