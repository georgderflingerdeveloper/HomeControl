using Moq;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.BASIC_CONSTANTS;
using BASIC_COMPONENTS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Timers;


namespace HomeControl.UNIT_TESTS.ADVANCED_COMPONENTS
{
    [TestClass]
    public class unittest_devicecommander
    {
        CommanderConfiguration TestConfig;
        bool TestResult = false;
        int NumberOfTestToggling = 10;
        Mock<ITimer> _MockTimerOn = new Mock<ITimer>();
        Mock<ITimer> _MockTimerOff = new Mock<ITimer>();

        [TestMethod]
        public void TestInactiveActivated( )
        {
            int Iterationes = 5;

            TestConfig = new CommanderConfiguration( )
            {
                Startindex = 0,
                Lastindex  = 0,
                Modes             = DeviceCommandos.SingleDeviceOnOffFallingEdge,
                ModesAutomaticoff = DeviceCommanderAutomaticOff.None,
                Modesdelayedon    = DeviceCommanderDelayedOn.None
            };

            DeviceCommander TestCommander = new DeviceCommander( TestConfig, new DeviceControlTimer( _MockTimerOn.Object, _MockTimerOff.Object ));

            Assert.AreEqual( DeviceState.Inactive, TestCommander.State );

            for( int i = 0; i < Iterationes; i++ )
            {
                // ON 
                TestCommander.MainTrigger( Edge.Rising );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.AreEqual( DeviceState.Activated, TestCommander.State );

                // OFF 
                TestCommander.MainTrigger( Edge.Rising );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.AreEqual( DeviceState.Inactive, TestCommander.State );
            }
        }

        [TestMethod]
        public void TestToggleDevice( )
        {
            TestConfig = new CommanderConfiguration( )
            {
                Startindex = 0,
                Lastindex  = 0,
                Modes = DeviceCommandos.SingleDeviceOnOffFallingEdge
            };

            DeviceCommander TestCommander = new DeviceCommander( TestConfig, new DeviceControlTimer() );
            TestCommander.EUpdate += TestCommander_EUpdate;

            for( int i = 0; i < NumberOfTestToggling; i++ )
            {
                // ON
                TestCommander.MainTrigger( Edge.Rising );
                Assert.IsFalse( TestResult );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.IsTrue( TestResult );

                // OFF
                TestCommander.MainTrigger( Edge.Rising );
                Assert.IsTrue( TestResult );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.IsFalse( TestResult );
            }
        }

        private void TestCommander_EUpdate( object sender, UpdateEventArgs e )
        {
            TestResult = e.Value;
        }

        [TestMethod]
        public void TestToggleWithAutomaticOff( )
        {
            TestConfig = new CommanderConfiguration( )
            {
                Startindex = 0,
                Lastindex = 0,
                Modes = DeviceCommandos.SingleDeviceOnOffFallingEdge,
                ModesAutomaticoff = DeviceCommanderAutomaticOff.WithMainAndPresenceTrigger
            };

            DeviceCommander TestCommander = new DeviceCommander( TestConfig, new DeviceControlTimer( _MockTimerOn.Object, _MockTimerOff.Object ));
            TestCommander.EUpdate += TestCommander_EUpdateAutoOff;

            // ON
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );
            Assert.IsTrue( TestResult );

            // OFF DELAY ELAPSED
            _MockTimerOff.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsFalse( TestResult );

            // ON
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );
            Assert.IsTrue( TestResult );

            TestCommander.PresenceTrigger( Edge.Rising );
            TestCommander.PresenceTrigger( Edge.Falling );

            // OFF DELAY ELAPSED
            _MockTimerOff.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsFalse( TestResult );
            Assert.IsTrue( TestCommander.InhibitPresenceDetection );

        }

        private void TestCommander_EUpdateAutoOff( object sender, UpdateEventArgs e )
        {
            TestResult = e.Value;
        }

        [TestMethod]
        public void TestDelayedOnOff()
        {
            TestResult = false;

            TestConfig = new CommanderConfiguration( )
            {
                Startindex = 0,
                Lastindex = 0,
                Modes = DeviceCommandos.SingleDeviceOnOffFallingEdge,
                ModesAutomaticoff = DeviceCommanderAutomaticOff.None,
                Modesdelayedon = DeviceCommanderDelayedOn.WithMainAndPresenceTrigger
            };

            DeviceCommander TestCommander = new DeviceCommander( TestConfig, new DeviceControlTimer( _MockTimerOn.Object, _MockTimerOff.Object ));
            TestCommander.EUpdate += TestCommander_EUpdateDelayedOn;

            // f.e
            // press button for a while => device is turned ON
            // press button again for a while => devices is turned OFF

            // ON is prevented as long as timer ran out
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );
            Assert.IsFalse( TestResult );

            // ON DELAY ELAPSED
            TestCommander.MainTrigger( Edge.Rising );
            _MockTimerOn.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            TestCommander.MainTrigger( Edge.Falling );
            Assert.IsTrue( TestResult );

            // no EFFECT !
            _MockTimerOff.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( TestResult );

            // no EFFECT !
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );
            Assert.IsTrue( TestResult );

            // TURN OFF
            // ON DELAY ELAPSED AGAIN
            TestCommander.MainTrigger( Edge.Rising );
            _MockTimerOn.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            TestCommander.MainTrigger( Edge.Falling );
            Assert.IsFalse( TestResult );

            // no EFFECT !
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );
            Assert.IsFalse( TestResult );
        }

        [TestMethod]
        public void TestDelayedOnOffWithAutomaticOff( )
        {
            TestResult = false;

            TestConfig = new CommanderConfiguration( )
            {
                Startindex = 0,
                Lastindex = 0,
                Modes = DeviceCommandos.SingleDeviceOnOffFallingEdge,
                ModesAutomaticoff = DeviceCommanderAutomaticOff.WithMainAndPresenceTrigger,
                Modesdelayedon = DeviceCommanderDelayedOn.WithMainAndPresenceTrigger
            };

            DeviceCommander TestCommander = new DeviceCommander( TestConfig, new DeviceControlTimer( _MockTimerOn.Object, _MockTimerOff.Object ));
            TestCommander.EUpdate += TestCommander_EUpdateDelayedOn;

            // f.e
            // press button for a while => device is turned ON
            // press button again for a while => devices is turned OFF
            // when device is on and configured time elapsed, the device will turn OFF again

            for( int i = 0; i < NumberOfTestToggling; i++ )
            {
                // ON is prevented as long as timer ran out
                TestCommander.MainTrigger( Edge.Rising );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.IsFalse( TestResult );

                // ON DELAY ELAPSED
                TestCommander.MainTrigger( Edge.Rising );
                _MockTimerOn.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.IsTrue( TestResult );

                // no EFFECT !
                TestCommander.MainTrigger( Edge.Rising );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.IsTrue( TestResult );

                // TURN OFF
                // ON DELAY ELAPSED AGAIN
                TestCommander.MainTrigger( Edge.Rising );
                _MockTimerOn.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.IsFalse( TestResult );

                // no EFFECT !
                TestCommander.MainTrigger( Edge.Rising );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.IsFalse( TestResult );

                // ON DELAY ELAPSED
                TestCommander.MainTrigger( Edge.Rising );
                _MockTimerOn.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.IsTrue( TestResult );

                _MockTimerOff.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                Assert.IsFalse( TestResult );
            }

        }

        [TestMethod]
        public void TestDelayedOnToggleRisingEdge( )
        {
            TestResult = false;

            TestConfig = new CommanderConfiguration( )
            {
                Startindex = 0,
                Lastindex = 0,
                Modes = DeviceCommandos.SingleDeviceToggleRisingEdge,
                ModesAutomaticoff = DeviceCommanderAutomaticOff.WithMainAndPresenceTrigger,
                Modesdelayedon = DeviceCommanderDelayedOn.WithMainAndPresenceTrigger
            };

            DeviceCommander TestCommander = new DeviceCommander( TestConfig, new DeviceControlTimer( _MockTimerOn.Object, _MockTimerOff.Object ));
            TestCommander.EUpdate += TestCommander_EUpdateDelayedOn;

            // f.e
            // press button for a while => device is turned ON
            // press button again for a while => devices is turned OFF
            // when device is on and configured time elapsed, the device will turn OFF again

            for( int i = 0; i < NumberOfTestToggling; i++ )
            {
                // ON is prevented as long as timer ran out
                TestCommander.MainTrigger( Edge.Rising );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.IsFalse( TestResult );

                // ON DELAY ELAPSED
                TestCommander.MainTrigger( Edge.Rising );
                _MockTimerOn.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                Assert.IsTrue( TestResult );
                TestCommander.MainTrigger( Edge.Falling );

                // no EFFECT !
                TestCommander.MainTrigger( Edge.Rising );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.IsTrue( TestResult );

                // TURN OFF
                // ON DELAY ELAPSED AGAIN
                TestCommander.MainTrigger( Edge.Rising );
                _MockTimerOn.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                /* *********************** */
                Assert.IsFalse( TestResult );
                /* *********************** */
                TestCommander.MainTrigger( Edge.Falling );

                // no EFFECT !
                TestCommander.MainTrigger( Edge.Rising );
                TestCommander.MainTrigger( Edge.Falling );
                Assert.IsFalse( TestResult );

                // ON DELAY ELAPSED
                TestCommander.MainTrigger( Edge.Rising );
                _MockTimerOn.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                Assert.IsTrue( TestResult );
                TestCommander.MainTrigger( Edge.Falling );

                _MockTimerOff.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                Assert.IsFalse( TestResult );
            }

        }

        private void TestCommander_EUpdateDelayedOn( object sender, UpdateEventArgs e )
        {
            TestResult = e.Value;
        }
    }
}
