using Moq;
using HomeControl.ADVANCED_COMPONENTS;
using BASIC_COMPONENTS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Timers;
using System.Threading;
using HomeControl.BASIC_CONSTANTS;
using HomeControl.ADVANCED_COMPONENTS.Interfaces;
using System.Collections.Generic;
using SystemServices;

namespace HomeControl.UNIT_TESTS.ADVANCED_COMPONENTS
{
    [TestClass]
    public class unittest_lightcommander_singlelight
    {
        Mock<ITimer> MockTimer_Off         = new Mock<ITimer>();

        CommanderConfiguration TestConfig; 

        int teststep = 0;
        bool fired = false;

        [TestMethod]
        public void Test_ToggleSingleLight( )
        {
            fired = false;

            TestConfig = new CommanderConfiguration( )
            {
                Startindex = 0,
                Modes = DeviceCommandos.SingleLight
            };

            LightCommander TestCommander = new LightCommander( TestConfig, new DeviceControlTimer() );
            TestCommander.EUpdate += TestCommander_EUpdate;

            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );
            teststep = 1;
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );
            teststep = 2;
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            Assert.IsTrue( fired );
        }

        private void TestCommander_EUpdate( object sender, UpdateEventArgs e )
        {
            fired = true;
            switch( teststep )
            {
                case 0:
                     Assert.AreEqual( TestConfig.Startindex, e.Index );
                     Assert.AreEqual( true, e.Value );
                     break;
                case 1:
                     Assert.AreEqual( TestConfig.Startindex, e.Index );
                     Assert.AreEqual( false, e.Value );
                     break;
                case 2:
                     Assert.AreEqual( TestConfig.Startindex, e.Index );
                     Assert.AreEqual( true, e.Value );
                     break;
            }
        }

        [TestMethod]
        public void Test_ToggleSingleLight_IndexOutOfRange_Negative( )
        {
            fired = false;

            TestConfig = new CommanderConfiguration( )
            {
                Startindex = -1, // WRONG INDEX !!!
                Modes = DeviceCommandos.SingleLight

            };

            LightCommander TestCommander = new LightCommander( TestConfig, new DeviceControlTimer() );
            TestCommander.EUpdate += TestCommander_EUpdate_OutOfRange;
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            // when using a wrong index, there is no firing
            Assert.IsFalse( fired );
        }

        [TestMethod]
        public void Test_ToggleSingleLight_IndexOutOfRange_Exceeded( )
        {
            fired = false;

            TestConfig = new CommanderConfiguration( )
            {
                Startindex = 333, // WRONG INDEX !!!
                Modes = DeviceCommandos.SingleLight

            };

            LightCommander TestCommander = new LightCommander( TestConfig, new DeviceControlTimer() );
            TestCommander.EUpdate += TestCommander_EUpdate_OutOfRange;
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            // when using a wrong index, there is no firing
            Assert.IsFalse( fired );
        }

        private void TestCommander_EUpdate_OutOfRange( object sender, UpdateEventArgs e )
        {
            fired = true;
        }

        [TestMethod]
        public void Test_ToggleSingleLight_AutomaticOff_ByMainTrigger( )
        {
            fired = false;

            TestConfig = new CommanderConfiguration( )
            {
                Startindex        = 0,
                Modes             = DeviceCommandos.SingleLight,
                ModesAutomaticoff = DeviceCommanderAutomaticOff.WithMainTrigger
            };

            Mock<IDeviceControlTimer> MockTimerDeviceControlTimer         = new Mock<IDeviceControlTimer>();

            LightCommander TestCommander = new LightCommander( TestConfig, MockTimerDeviceControlTimer.Object );
            TestCommander.EUpdate += TestCommander_EUpdateAutomaticOffByTrigger;

            teststep = 0;
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            teststep = 1;
            // MOCK AUTOMATIC OFF TIME ELAPSED
            MockTimerDeviceControlTimer.Raise( timer => timer.EControlOff += null , new System.EventArgs( ) as ElapsedEventArgs );

            Thread.Sleep( 1 );

            Assert.IsTrue( fired );
        }


        [TestMethod]
        public void Test_ToggleSingleLight_AutomaticOff_ByPresenceTrigger( )
        {
            fired = false;

            TestConfig = new CommanderConfiguration( )
            {
                Startindex = 0,
                Modes = DeviceCommandos.SingleLight,
                ModesAutomaticoff = DeviceCommanderAutomaticOff.WithPresenceTrigger
            };

            Mock<IDeviceControlTimer> MockTimerDeviceControlTimer         = new Mock<IDeviceControlTimer>();

            LightCommander TestCommander = new LightCommander( TestConfig, MockTimerDeviceControlTimer.Object );
            TestCommander.EUpdate += TestCommander_EUpdateAutomaticOffByTrigger;

            // TURN ON
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            teststep = 0;

            // PRESECENCE DETECTOR TRIGGERS
            TestCommander.PresenceTrigger( Edge.Rising );
            TestCommander.PresenceTrigger( Edge.Falling );

            teststep = 1;

            // MOCK AUTOMATIC OFF TIME ELAPSED
            MockTimerDeviceControlTimer.Raise( timer => timer.EControlOff += null, new System.EventArgs( ) as ElapsedEventArgs );

            Thread.Sleep( 1 );

            Assert.IsTrue( fired );

            teststep = 2;

            // PRESECENCE DETECTOR TRIGGERS AGAIN
            TestCommander.PresenceTrigger( Edge.Rising );
            TestCommander.PresenceTrigger( Edge.Falling );

            Assert.IsTrue( TestCommander.InhibitPresenceDetection );

            // TURN ON AGAIN
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            Assert.IsFalse( TestCommander.InhibitPresenceDetection );
        }

        private void TestCommander_EUpdateAutomaticOffByTrigger( object sender, UpdateEventArgs e )
        {
            switch( teststep )
            {
                case 0:
                     Assert.AreEqual( TestConfig.Startindex, e.Index );
                     Assert.AreEqual( true, e.Value );
                     break;

                case 1:
                     Assert.AreEqual( TestConfig.Startindex, e.Index );
                     Assert.AreEqual( false, e.Value );
                     fired = true;
                     break;

                default:
                     break;
            }
        }

        [TestMethod]
        public void Test_ToggleSingleLight_OnOff_ByPresenceTrigger_RisingEdge( )
        {

            fired = false;

            TestConfig = new CommanderConfiguration( )
            {
                Startindex = 0,
                Modes = DeviceCommandos.OnWithDelayedOffByRisingEdge
            };
            Mock<IDeviceControlTimer> MockTimerDeviceControlTimer         = new Mock<IDeviceControlTimer>();

            LightCommander TestCommander = new LightCommander( TestConfig, MockTimerDeviceControlTimer.Object );
            TestCommander.EUpdate += TestCommander_EUpdateOnOffByPresenceDetector; ;


            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            teststep = 0;

            TestCommander.PresenceTrigger( Edge.Falling );
            TestCommander.PresenceTrigger( Edge.Rising );

            teststep = 1;

            // MOCK AUTOMATIC OFF TIME ELAPSED
            MockTimerDeviceControlTimer.Raise( timer => timer.EControlOff += null, new System.EventArgs( ) as ElapsedEventArgs );

            Thread.Sleep( 1 );
            Assert.IsTrue( fired );

            teststep = 2;

            TestCommander.PresenceTrigger( Edge.Falling );
            TestCommander.PresenceTrigger( Edge.Rising );

            Assert.IsFalse( fired );
        }

        [TestMethod]
        public void Test_ToggleSingleLight_OnOff_ByPresenceTrigger_FallingEdge( )
        {
            fired = false;

            TestConfig = new CommanderConfiguration( )
            {
                Startindex = 0,
                Modes = DeviceCommandos.OnWithDelayedOffByFallingEdge
            };
            Mock<IDeviceControlTimer> MockTimerDeviceControlTimer         = new Mock<IDeviceControlTimer>();

            LightCommander TestCommander = new LightCommander( TestConfig, MockTimerDeviceControlTimer.Object );
            TestCommander.EUpdate += TestCommander_EUpdateOnOffByPresenceDetector; ;

            teststep = 0;
            TestCommander.MainTrigger( Edge.Rising );
            TestCommander.MainTrigger( Edge.Falling );

            teststep = 1;
            // MOCK AUTOMATIC OFF TIME ELAPSED
            MockTimerDeviceControlTimer.Raise( timer => timer.EControlOff += null, new System.EventArgs( ) as ElapsedEventArgs );

            Thread.Sleep( 1 );
            Assert.IsTrue( fired );

            teststep = 2;
            TestCommander.PresenceTrigger( Edge.Rising );
            TestCommander.PresenceTrigger( Edge.Falling );

            Assert.IsFalse( fired );
        }

        private void TestCommander_EUpdateOnOffByPresenceDetector( object sender, UpdateEventArgs e )
        {
            switch( teststep )
            {
                case 0:
                     Assert.AreEqual( TestConfig.Startindex, e.Index );
                     Assert.AreEqual( true, e.Value );
                     break;
                case 1:
                     Assert.AreEqual( TestConfig.Startindex, e.Index );
                     Assert.AreEqual( false, e.Value );
                     fired = true;
                     break;
                case 2:
                     Assert.AreEqual( TestConfig.Startindex, e.Index );
                     Assert.AreEqual( true, e.Value );
                     fired = false;
                     break;
            }

        }
    }

    [TestClass]
    public class unittest_lightcommander_multilights
    {
        Mock<ITimer> MockTimer_Off         = new Mock<ITimer>();
        LightCommander TestCommander;
        CommanderConfiguration TestConfig = new CommanderConfiguration();
        bool[] TestLightGroup            = new bool[] {true, true, true, true, true, true};
        bool[] TestLightGroupExpectedOff = new bool[] {false, true, false, true, false, true};

        public unittest_lightcommander_multilights()
        {
            TestConfig.DeviceRemainOnAfterAutomaticOff = new List<int>(){ 1, 3, 5 };
            TestConfig.Startindex = 0;
            TestConfig.Lastindex  = 5;
            TestConfig.ModesAutomaticoff = DeviceCommanderAutomaticOff.WithMainAndPresenceTrigger;

            TestCommander = new LightCommander( TestConfig, new DeviceControlTimer( new Timer_(1), MockTimer_Off.Object ) );
            TestCommander.Commando = DeviceCommandos.ScenarioLight;
            TestCommander.EUpdate += TestCommander_EUpdate;
        }

        [TestMethod]
        public void TestAutomaticOff_SomeLightsRemainON( )
        {
            TestCommander.PresenceTrigger( Edge.Falling );
            MockTimer_Off.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup , TestLightGroupExpectedOff ) );
        }

        private void TestCommander_EUpdate( object sender, UpdateEventArgs e )
        {
            TestLightGroup[e.Index] = e.Value;
        }
    }

    [TestClass]
    public class unittest_extendedlightcommander
    {
        Mock<ITimer> MockTimer_Off          = new Mock<ITimer>();
        Mock<ITimer> MockTimer_On           = new Mock<ITimer>();
        Mock<ITimer> MockTimer_FinalOff     = new Mock<ITimer>();
        Mock<ITimer> MockTimer_Idle         = new Mock<ITimer>();
        Mock<ITimer> MockTimer_Next         = new Mock<ITimer>();
        Mock<ITimer> MockTimer_Auto         = new Mock<ITimer>();

        ExtendedLightCommander TestCommander;
        CommanderConfiguration  TestConfig = new CommanderConfiguration();
        DeviceScenarioControl   TestScenarioControl;

        bool[] TestLightExpectedOn          = new bool[] {true,  true,  true,  true,  true,  true,  true};
        bool[] TestLightGroup               = new bool[] {true,  true,  true,  true,  true,  true,  true};
        bool[] TestLightGroupExpectedOff    = new bool[] {false, true,  false, true,  false, true,  false};
        bool[] TestLightGroupExpectedAllOff = new bool[] {false, false, false, false, false, false, false};

        public unittest_extendedlightcommander( )
        {
            // COMMANDER CONFIGURATION
            TestConfig.DeviceRemainOnAfterAutomaticOff = new List<int>( ) { 1, 3, 5 };
            TestConfig.Startindex = 0;
            TestConfig.Lastindex  = 6;
            TestConfig.ModesAutomaticoff = DeviceCommanderAutomaticOff.WithMainAndPresenceTrigger;

            // SCENARIO CONFIGURATION
            TestScenarioControl = new DeviceScenarioControl(TestConfig.Startindex, TestConfig.Lastindex,
                                                        MockTimer_Next.Object, MockTimer_Auto.Object, MockTimer_Idle.Object );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
              new List<int> { 0, 1, 2, 3, 4, 5, 6 },
              new List<int> { 7, 8, 9 }
            };

            TestCommander = new ExtendedLightCommander( TestConfig,
                                                        new DeviceControlTimer( MockTimer_On.Object, MockTimer_Off.Object ),
                                                        TestScenarioControl );
            TestCommander.Commando = DeviceCommandos.ScenarioLight;
            TestCommander.ExtUpdate += TestCommander_EUpdate;
            TestCommander.AvailableScenarios = TestScenarioControl.Scenarios;
        }

        void ToggleOnOff()
        {
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );
        }

        void TurnOn()
        {
            ToggleOnOff( );
        }

        void TurnOff( )
        {
            ToggleOnOff( );
        }

        void PresenceTrigger()
        {
            TestCommander.PresenceTrigger( Edge.Falling );
            MockTimer_Off.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
        }

        void TurnPartialOffWithPresenceDetector()
        {
            PresenceTrigger( );
        }

        void ActivatePresenceTrigger()
        {
            PresenceTrigger( );
        }

        [TestMethod]
        public void TestAutomaticOff_SomeLightsRemainON( )
        {
            TestCommander.ScenarioTrigger( Edge.Falling );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightExpectedOn ) );

            TestCommander.PresenceTrigger( Edge.Falling );
            MockTimer_Off.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightGroupExpectedOff ) );
            Assert.IsTrue( TestCommander.InhibitPresenceDetection );
        }

        [TestMethod]
        public void TestPartialOff( )
        {
            TurnOn( );

            TurnPartialOffWithPresenceDetector( );

            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightGroupExpectedOff ) );
            Assert.IsTrue( TestCommander.InhibitPresenceDetection );
        }

        [TestMethod]
        public void TestAllOffAfterPartialOff( )
        {
            TurnOn( );

            TurnPartialOffWithPresenceDetector( );

            TurnOff( );

            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightGroupExpectedAllOff ) );
            Assert.IsTrue( TestCommander.InhibitPresenceDetection );
        }

        [TestMethod]
        public void TestAllOffAfterPartialOffIgnorePresenceTrigger( )
        {
            TurnOn( );

            TurnPartialOffWithPresenceDetector( );

            TurnOff( );

            ActivatePresenceTrigger( );

            //NO EFFECT!
            Assert.IsTrue( TestCommander.InhibitPresenceDetection );
        }


        [TestMethod]
        public void TestAutomaticOff_SomeLightsRemainON_RESTART_PRESCENCEDETECTOR( )
        {
            TurnOn( );

            TurnPartialOffWithPresenceDetector( );

            TurnOff( ); // All

            //NO EFFECT!
            ActivatePresenceTrigger( );

            // ALL ON
            TurnOn( );

            // Presence detector is active again
            Assert.IsFalse( TestCommander.InhibitPresenceDetection );
        }
 

        [TestMethod]
        public void TestAutomaticOff_FinalOff( )
        {
            TestCommander = new ExtendedLightCommander( TestConfig,
                                            new DeviceControlTimer( MockTimer_On.Object, MockTimer_Off.Object, MockTimer_FinalOff.Object ),
                                            TestScenarioControl );

            TestCommander.Commando = DeviceCommandos.ScenarioLight;
            TestCommander.ExtUpdate += TestCommander_EUpdate;

            // ON
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightExpectedOn ) );

            // FINAL OFF
            MockTimer_FinalOff.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightGroupExpectedAllOff ) );

            // ALL ON
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightExpectedOn ) );
        }

        [TestMethod]
        public void TestAutomaticOff_PartialOff_FinalOff( )
        {
            TestCommander = new ExtendedLightCommander( TestConfig,
                                            new DeviceControlTimer( MockTimer_On.Object, MockTimer_Off.Object, MockTimer_FinalOff.Object ),
                                            TestScenarioControl );
            TestCommander.Commando = DeviceCommandos.ScenarioLight;
            TestCommander.ExtUpdate += TestCommander_EUpdate;

            // ON
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightExpectedOn ) );

            //PARTIAL OFF
            TestCommander.PresenceTrigger( Edge.Falling );
            MockTimer_Off.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( TestCommander.InhibitPresenceDetection );

            //FINAL OFF
            MockTimer_FinalOff.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightGroupExpectedAllOff ) );
            Assert.IsTrue( TestCommander.InhibitPresenceDetection );

            ////NO EFFECT!
            TestCommander.PresenceTrigger( Edge.Falling );
            MockTimer_Off.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( TestCommander.InhibitPresenceDetection );

            // ALL ON
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );
            // Presence detector is active again
            Assert.IsFalse( TestCommander.InhibitPresenceDetection );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightExpectedOn ) );
        }

        [TestMethod]
        public void TestAutomaticOff_FinalOff_After_Idle( )
        {
            TestCommander = new ExtendedLightCommander( TestConfig,
                                            new DeviceControlTimer( MockTimer_On.Object, MockTimer_Off.Object, MockTimer_FinalOff.Object ),
                                            TestScenarioControl );

            TestCommander.Commando = DeviceCommandos.ScenarioLight;
            TestCommander.ExtUpdate += TestCommander_EUpdate;

            // ON
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightExpectedOn ) );

            // IDLE
            TestCommander.ScenarioTrigger( Edge.Rising );
            MockTimer_Idle.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            TestCommander.ScenarioTrigger( Edge.Falling );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightExpectedOn ) );

            // FINAL OFF
            MockTimer_FinalOff.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightGroupExpectedAllOff ) );

            // ALL ON
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightExpectedOn ) );
        }

        [TestMethod]
        public void TestAutomaticOff( )
        {
            TestConfig.ModesAutomaticoff = DeviceCommanderAutomaticOff.WithMainTrigger;
            TestConfig.DeviceRemainOnAfterAutomaticOff = new List<int>( ) {};
            TestCommander = new ExtendedLightCommander( TestConfig,
                                            new DeviceControlTimer( MockTimer_On.Object, MockTimer_Off.Object, MockTimer_FinalOff.Object ),
                                            TestScenarioControl );

            TestCommander.Commando = DeviceCommandos.ScenarioLight;
            TestCommander.ExtUpdate += TestCommander_EUpdate;

            // ON
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightExpectedOn ) );

            // AUTOMATIC OFF
            MockTimer_Off.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightGroupExpectedAllOff ) );

            // ON
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup, TestLightExpectedOn ) );
        }

        private void TestCommander_EUpdate( object sender, UpdateEventArgs e )
        {
            TestLightGroup[e.Index] = e.Value;
        }

        bool[] TestLightExpectedOff_AfterMissingPresenceSignal       = new bool[] {false, false, false, false };
        bool[] TestLightExpectedOn_                                  = new bool[] {true, true, false, false };
        bool[] TestLightExpectedOff_                                 = new bool[] {false, false, false, false };
        bool[] TestLightGroup_Actual                                 = new bool[] {false, false, false, false };

        void TestScenarioAnteRoom()
        {
            TestConfig.DeviceRemainOnAfterAutomaticOff = new List<int>( ) { 2, 3 };
            TestConfig.Startindex = 0;
            TestConfig.Lastindex = 3;
            TestConfig.ModesAutomaticoff = DeviceCommanderAutomaticOff.WithMainAndPresenceTrigger;


            // SCENARIO CONFIGURATION
            TestScenarioControl = new DeviceScenarioControl( TestConfig.Startindex, TestConfig.Lastindex,
                                                        MockTimer_Next.Object, MockTimer_Auto.Object, MockTimer_Idle.Object );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
              new List<int> { 0, 1 },
              new List<int> { 2, 3 },
              new List<int> { 0, 1 ,2, 3 }
            };

            TestCommander = new ExtendedLightCommander( TestConfig,
                                            new DeviceControlTimer( MockTimer_On.Object, MockTimer_Off.Object, MockTimer_FinalOff.Object ),
                                            TestScenarioControl );

            TestCommander.Commando           = DeviceCommandos.ScenarioLight;
            TestCommander.ExtUpdate         += TestCommander_EUpdate_AnteRoom;
            TestCommander.AvailableScenarios = TestScenarioControl.Scenarios;
        }


        [TestMethod]
        public void Test_AnteRoom_TurnON( )
        {
            TestScenarioAnteRoom( );

            // ON
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );
            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup_Actual, TestLightExpectedOn_ ) );
        }

        [TestMethod]
        public void Test_AnteRoom_TurnOFF( )
        {
            TestScenarioAnteRoom( );

            // ON
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );

            // OFF
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );

            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup_Actual, TestLightExpectedOff_ ) );
        }

        void On_To_Automatic_Off_Via_Presence_Detector()
        {
            // ON
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );

            // PRESENCE DETECTOR
            TestCommander.PresenceTrigger( Edge.Falling );
            TestCommander.PresenceTrigger( Edge.Rising );

            // AUTOMATIC OFF
            MockTimer_Off.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
        }

        [TestMethod]
        public void Test_AnteRoom_TurnOFF_BY_PresenceDetector( )
        {
            TestScenarioAnteRoom( );

            On_To_Automatic_Off_Via_Presence_Detector( );

            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup_Actual, TestLightExpectedOff_ ) );
        }


        [TestMethod]
        public void TestOff_ByMissingPresenceSignal_TurnOnAgain( )
        {
            TestScenarioAnteRoom( );

            On_To_Automatic_Off_Via_Presence_Detector( );

            // ON
            TestCommander.ScenarioTrigger( Edge.Rising );
            TestCommander.ScenarioTrigger( Edge.Falling );

            Assert.IsTrue( Memory.CompareBoolArryas( TestLightGroup_Actual, TestLightExpectedOn_ ) );
        }


        private void TestCommander_EUpdate_AnteRoom( object sender, UpdateEventArgs e )
        {
            TestLightGroup_Actual[e.Index] = e.Value;
        }

    }
}
