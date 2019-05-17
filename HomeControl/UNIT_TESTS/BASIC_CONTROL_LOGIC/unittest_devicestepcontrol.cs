//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Timers;
using Moq;
using BASIC_COMPONENTS;

using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace BASIC_CONTROL_LOGIC
{
    [TestClass]
    public class unittest_lightstepcontrol
    {
        const uint NumberOfDevices           = 16;
        const double TimeNext                = 40;
        const double LittleDelay             = 20;
        const uint NumberOfOnOffTests        = 2;
        uint  StartIndex                     = 0;
        const int TestStep1 = 1;
        const int TestStep2 = 2;
        int EventRaisedCounter = 1;
        long ExpectedEvents = 0;

        devicestepcontrol StepControl;
        ITimer timernext =  new Timer_( TimeNext );

        void SignalChangeOnOff( )
        {
            StepControl.WatchForInputValueChange( true );
            StepControl.WatchForInputValueChange( false );
        }

        [TestMethod]
        public void Test_DeviceValueSwitch( )
        {
            StepControl = new devicestepcontrol( StartIndex, NumberOfDevices, timernext );

            for( uint i = 0; i < NumberOfOnOffTests; i++ )
            {
                SignalChangeOnOff( ); // f.e. emulation of pushing / pulling a button
                Assert.AreEqual( true, StepControl.Value );
                SignalChangeOnOff( );
                Assert.AreEqual( false, StepControl.Value );
                StepControl.Reset( );
                Assert.AreEqual( (System.UInt32) 0, StepControl.Number );
            }
        }

 
        [TestMethod]
        public void Test_StartDeviceValueWhenSwitchingToNextElement( )
        {
            EventRaisedCounter = 1;
            Mock<ITimer> MockTimer = new Mock<ITimer>();
             
            StepControl = new devicestepcontrol(StartIndex, NumberOfDevices, MockTimer.Object );
            StepControl.EStep += StepControl_EStep;

            StepControl.WatchForInputValueChange( true );
            MockTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            StepControl.WatchForInputValueChange( false );
            StepControl.Reset( );
            Assert.AreEqual( 4 , EventRaisedCounter );
        }
        private void StepControl_EStep( uint number, bool value )
        {
            switch( EventRaisedCounter )
            {
                case TestStep1:
                     Assert.AreEqual( (System.UInt32) 0, StepControl.Number );
                     Assert.AreEqual( false, StepControl.Value );
                     break;
                case TestStep2:
                     Assert.AreEqual( (System.UInt32) 1, StepControl.Number );
                     Assert.AreEqual( false, StepControl.Value );
                     break;
            }
            EventRaisedCounter++;
        }

        [TestMethod]
        public void Test_DeviceValueWhenSwitchingToNextElement( )
        {
            StartIndex = 0;
            Mock<ITimer> MockTimer = new Mock<ITimer>();

            StepControl = new devicestepcontrol( StartIndex, NumberOfDevices, MockTimer.Object );
            StepControl.EStep += StepControl_EStepMulitple;

            do
            {
                for( uint i = StartIndex; i < NumberOfDevices; i++ )
                {
                    StepControl.WatchForInputValueChange( true );
                    MockTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                    StepControl.WatchForInputValueChange( false );
                    ExpectedEvents += 3; 
                }
                StartIndex++;
            } while( StartIndex <= NumberOfDevices - 1 );
            StepControl.Reset( );
            StartIndex = 0;
            Assert.AreEqual( ExpectedEvents, RaisedEvents_ );
        }

        int TestCounter_multi = 1;
        long RaisedEvents_ = 0;
        private void StepControl_EStepMulitple( uint number, bool value )
        {
            RaisedEvents_++;
            if( ( TestCounter_multi % 3 ) == 0 )
            {
                return;
            }

            if( (TestCounter_multi % 2) != 0)
            {
                 Assert.AreEqual( (System.UInt32) TestCounter_multi - 1, StepControl.Number );
                 Assert.AreEqual( false, StepControl.Value );
            }
            else if( ( TestCounter_multi % 2 ) == 0 )
            {
                 Assert.AreEqual( (System.UInt32) TestCounter_multi - 1, StepControl.Number );
                 Assert.AreEqual( false, StepControl.Value );
            }
            TestCounter_multi++;
      }
    }
}

