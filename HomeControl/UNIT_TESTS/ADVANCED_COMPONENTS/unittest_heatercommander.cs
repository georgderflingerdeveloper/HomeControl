using Moq;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.BASIC_CONSTANTS;
using BASIC_COMPONENTS;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace HomeControl.UNIT_TESTS.ADVANCED_COMPONENTS
{
    [TestClass]
    public class unittest_heatercommander
    {
        HeaterCommanderConfiguration TestConfig;
        Mock<ITimer> _MockTimerOn  = new Mock<ITimer>();
        Mock<ITimer> _MockTimerOff = new Mock<ITimer>();
        bool TestResult;
        int ReportedDeviceIndex;

        void Config( )
        {
            TestConfig = new HeaterCommanderConfiguration( )
            {
                Startindex = 0,
                Lastindex  = 0,
                Modes             = DeviceCommandos.SingleDeviceOnOffFallingEdge,
                ModesAutomaticoff = DeviceCommanderAutomaticOff.None,
                Modesdelayedon    = DeviceCommanderDelayedOn.None,
                Options           = HOptions.EventSwitchActive,
                OptionProperties  = HOptionProperties.SwitchOffEvent
            };
        }

        HeaterCommander CreateHeaterCommander( )
        {
            HeaterCommander TestCommander = new HeaterCommander( TestConfig, new DeviceControlTimer( _MockTimerOn.Object, _MockTimerOff.Object ));

            TestCommander.EUpdate += TestCommander_EUpdate;

            return ( TestCommander );
        }

        void CleanupTestResult( )
        {
            TestResult = false;
            ReportedDeviceIndex = 0;
        }

        [TestMethod]
        public void EventOff_UnitTest( )
        {
            Config( );

            HeaterCommander TestCommander = CreateHeaterCommander( );

            CleanupTestResult( );

            // TURN ON
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            // EVENT OFF TRIGGERED ( f.e. Window OPENS )
            TestCommander.EventSwitch(  TurnDevice.OFF );

            Assert.IsFalse( TestResult );
            Assert.AreEqual( TestConfig.Startindex, ReportedDeviceIndex );
        }

        [TestMethod]
        public void TestOFFAfterEvent( )
        {
            Config( );

            HeaterCommander TestCommander = CreateHeaterCommander( );

            CleanupTestResult( );

            // TURN ON
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            // EVENT OFF TRIGGERED ( f.e. Window OPENS )
            TestCommander.EventSwitch( TurnDevice.OFF );

            // EVENT ON TRIGGERED ( f.e. Window is closed )
            TestCommander.EventSwitch( TurnDevice.ON );

            // TURN OFF
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            // EVENT OFF TRIGGERED ( f.e. Window OPENS )
            TestCommander.EventSwitch( TurnDevice.OFF );
            // EVENT OFF TRIGGERED ( f.e. Window CLOSES )
            TestCommander.EventSwitch( TurnDevice.ON );

            // NO EFFECT !
            Assert.IsFalse( TestResult );
            Assert.AreEqual( TestConfig.Startindex, ReportedDeviceIndex );
        }


        [TestMethod]
        public void Test_Off_On_TurnOff_Event( )
        {
            Config( );

            HeaterCommander TestCommander = CreateHeaterCommander( );

            CleanupTestResult( );

            // TURN ON
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            // EVENT OFF TRIGGERED ( f.e. Window OPENS )
            TestCommander.EventSwitch( TurnDevice.OFF );

            // TURN OFF
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            Assert.IsFalse( TestResult );
            Assert.AreEqual( TestConfig.Startindex, ReportedDeviceIndex );
        }

        private void TestCommander_EUpdate( object sender, UpdateEventArgs e )
        {
            TestResult          = e.Value;
            ReportedDeviceIndex = e.Index;
        }
    }
}
