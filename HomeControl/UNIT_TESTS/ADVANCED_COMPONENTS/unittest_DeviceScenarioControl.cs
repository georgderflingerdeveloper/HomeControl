﻿using BASIC_COMPONENTS;
using HomeControl.ADVANCED_COMPONENTS;
using System;
using System.Timers;
using System.Collections.Generic;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeControl.BASIC_CONSTANTS;
using SystemServices;


namespace HomeControl.UNIT_TESTS.ADVANCED_COMPONENTS
{
    [TestClass]
    public class unittest_DeviceScenarioControl
    {
        DeviceScenarioControl TestScenarioControl;
        int TestIndex = 0;
        int TestFirstScenario_ON_OFF = 0;
        int TestSecondScenario_ON_OFF = 1;
        const int TestDevice1=1, TestDevice2=2, TestDevice3=3, TestDevice4=4, TestDevice5=5, TestDevice6=6;

        Mock<ITimer> _MockNextScenario = new Mock<ITimer>();
        Mock<ITimer> _MockAutoScenario = new Mock<ITimer>();
        Mock<ITimer> _MockIdleScenario = new Mock<ITimer>();

        [TestMethod]
        public void ScenarioInitialisation()
        {
            TestIndex = 0;

            int FirstIndex = TestDevice1;
            int LastIndex = TestDevice6;
            List<List<int>>  Scenarios = new List<List<int>>()
            {
              // scenario 1
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              // scenario 2
              new List<int> { TestDevice4, TestDevice5, TestDevice6 }
            };

            TestScenarioControl = new DeviceScenarioControl(Scenarios, FirstIndex, LastIndex, new Timer_(1), new Timer_(1), new Timer_(1));

            Assert.IsTrue(Memory.CompareObjects(Scenarios, TestScenarioControl.Scenarios));

        }

        [TestMethod]
        public void SelectOneScenario_UnitTest()
        {
            TestIndex = 0;

            int FirstIndex = TestDevice1;
            int LastIndex = TestDevice6;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, new Timer_( 1 ), new Timer_( 1 ), new Timer_( 1 ) );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
              // scenario 1
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              // scenario 2
              new List<int> { TestDevice4, TestDevice5, TestDevice6 }
            };

            TestScenarioControl.EScenario += TestScenarioControl_EScenario;

            // TURN ON
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // TURN OFF
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            Assert.AreEqual( 6, TestIndex );
        }

        [TestMethod]
        public void SingleScenarioOnOff_UnitTest( )
        {
            TestIndex = 0;

            int FirstIndex = TestDevice1;
            int LastIndex  = TestDevice6;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, new Timer_( 1 ), new Timer_( 1 ), new Timer_( 1 ) );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
              // scenario 1
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              // scenario 2
              new List<int> { TestDevice4, TestDevice5, TestDevice6 }
            };

            TestScenarioControl.EScenario += TestScenarioControl_EScenario;

            // TURN ON
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // TURN OFF
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            Assert.AreEqual( 6, TestIndex );
        }

        private void TestScenarioControl_EScenario( object sender, ScenarioEventArgs e )
        {
            switch( TestIndex )
            {
                case 0:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice1, e.Index );
                Assert.AreEqual( PowerState.ON, e.Value );
                break;
                case 1:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice2, e.Index );
                Assert.AreEqual( PowerState.ON, e.Value );
                break;
                case 2:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice3, e.Index );
                Assert.AreEqual( PowerState.ON, e.Value );
                break;
                case 3:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice1, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;
                case 4:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice2, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;
                case 5:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice3, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;
            }
            TestIndex++;
        }

        [TestMethod]
        public void Test_ScenarioStep_ByTrigger( )
        {
            TestIndex = 0;
            int FirstIndex = TestDevice1;
            int LastIndex  = TestDevice6;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, _MockNextScenario.Object, _MockAutoScenario.Object, _MockIdleScenario.Object );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              new List<int> { TestDevice4, TestDevice5, TestDevice6 }
            };

            TestScenarioControl.EScenario += TestScenarioControl_EScenarioChange;

            // TURN ON
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            // NEXT SCENARIO
            _MockNextScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // TURN OFF
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // HOLD "BUTTON" Longer ... - no effect at all
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            _MockNextScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            _MockIdleScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // TURN ON AGAIN
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // HOLD "BUTTON" Longer ... - no effect at all
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            _MockNextScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            _MockIdleScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // TURN OFF AGAIN
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            Assert.AreEqual( 15, TestIndex );
        }

        private void TestScenarioControl_EScenarioChange( object sender, ScenarioEventArgs e )
        {
            switch( TestIndex )
            {
                // SCENARIO 0 OFF - that means that previous scenario is cleared
                case 0:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice1, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;

                case 1:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice2, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;

                case 2:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice3, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;
                // SCENARIO 1 ON
                case 3:
                Assert.AreEqual( TestSecondScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice4, e.Index );
                Assert.AreEqual( PowerState.ON, e.Value );
                break;
                case 4:
                Assert.AreEqual( TestSecondScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice5, e.Index );
                Assert.AreEqual( PowerState.ON, e.Value );
                break;
                case 5:
                Assert.AreEqual( TestSecondScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice6, e.Index );
                Assert.AreEqual( PowerState.ON, e.Value );
                break;
                // SCENARIO 1 OFF
                case 6:
                Assert.AreEqual( TestSecondScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice4, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;
                case 7:
                Assert.AreEqual( TestSecondScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice5, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;
                case 8:
                Assert.AreEqual( TestSecondScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice6, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;
                // SCENARIO 1 ON AGAIN
                case 9:
                Assert.AreEqual( TestSecondScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice4, e.Index );
                Assert.AreEqual( PowerState.ON, e.Value );
                break;
                case 10:
                Assert.AreEqual( TestSecondScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice5, e.Index );
                Assert.AreEqual( PowerState.ON, e.Value );
                break;
                case 11:
                Assert.AreEqual( TestSecondScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice6, e.Index );
                Assert.AreEqual( PowerState.ON, e.Value );
                break;
                // SCENARIO 1 OFF AGAIN
                case 12:
                Assert.AreEqual( TestSecondScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice4, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;
                case 13:
                Assert.AreEqual( TestSecondScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice5, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;
                case 14:
                Assert.AreEqual( TestSecondScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice6, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;
            }
            TestIndex++;
        }

        [TestMethod]
        public void Test_ScenarioStep_Idle_ON( )
        {
            TestIndex = 0;
            int FirstIndex = TestDevice1;
            int LastIndex  = TestDevice6;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, _MockNextScenario.Object, _MockAutoScenario.Object, _MockIdleScenario.Object );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              new List<int> { TestDevice4, TestDevice5, TestDevice6 }
            };

            TestScenarioControl.EScenario += TestScenarioControl_EIdle;


            // HOLD "BUTTON" Longer ... - no effect at all
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            _MockNextScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            _MockIdleScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // TURN ON 
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

        }

        private void TestScenarioControl_EIdle( object sender, ScenarioEventArgs e )
        {
            switch( TestIndex )
            {
                case 0:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice1, e.Index );
                Assert.AreEqual( PowerState.ON, e.Value );
                break;

                case 1:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice2, e.Index );
                Assert.AreEqual( PowerState.ON, e.Value );
                break;

                case 2:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice3, e.Index );
                Assert.AreEqual( PowerState.ON, e.Value );
                break;
            }
            TestIndex++;
        }

        [TestMethod]
        public void Test_ScenarioStep_Idle_OFF( )
        {
            TestIndex = 0;
            int FirstIndex = TestDevice1;
            int LastIndex  = TestDevice6;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, _MockNextScenario.Object, _MockAutoScenario.Object, _MockIdleScenario.Object );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              new List<int> { TestDevice4, TestDevice5, TestDevice6 }
            };

            TestScenarioControl.EScenario += TestScenarioControl_EIdleOFF;

            // TURN ON 
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // HOLD "BUTTON" Longer ... - no effect at all
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            _MockNextScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            _MockIdleScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // TURN OFF 
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

        }

        private void TestScenarioControl_EIdleOFF( object sender, ScenarioEventArgs e )
        {
            switch( TestIndex )
            {
                case 3:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice1, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;

                case 4:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice2, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;

                case 5:
                Assert.AreEqual( TestFirstScenario_ON_OFF, e.Number );
                Assert.AreEqual( TestDevice3, e.Index );
                Assert.AreEqual( PowerState.OFF, e.Value );
                break;
            }
            TestIndex++;
        }

        int _ScenarioNumber;
        [TestMethod]
        public void Test_ScenarioStep_Overrun( )
        {
            TestIndex = 0;
            int FirstIndex = TestDevice1;
            int LastIndex  = TestDevice6;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, _MockNextScenario.Object, _MockAutoScenario.Object, _MockIdleScenario.Object );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              new List<int> { TestDevice4, TestDevice5, TestDevice6 },
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              new List<int> { TestDevice4, TestDevice5, TestDevice6 },
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              new List<int> { TestDevice4, TestDevice5, TestDevice6 }
            };

            TestScenarioControl.EScenario += TestScenarioControl_ETestOverrun;

            for( int i = 0; i < TestScenarioControl.Scenarios.Count; i++ )
            {
                TestScenarioControl.WatchForInputValueChange( Edge.Rising );
                _MockNextScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                TestScenarioControl.WatchForInputValueChange( Edge.Falling );
            }

            Assert.AreEqual( 0, _ScenarioNumber );
        }

        private void TestScenarioControl_ETestOverrun( object sender, ScenarioEventArgs e )
        {
            _ScenarioNumber = e.Number;
        }

        int TestNumber;
        [TestMethod]
        public void ScenarioEmptyList_UnitTest( )
        {
            TestIndex = 0;
            int FirstIndex = TestDevice1;
            int LastIndex  = TestDevice6;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, _MockNextScenario.Object, _MockAutoScenario.Object, _MockIdleScenario.Object );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
            };

            TestScenarioControl.EScenario += TestScenarioControl_EScenarioEmptyList;

            // TURN ON
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            // NEXT SCENARIO
            _MockNextScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            Assert.AreEqual( 0, TestNumber );
        }

        [TestMethod]
        public void ScenarioEmptydeviceList_UnitTest( )
        {
            TestIndex = 0;
            int FirstIndex = TestDevice1;
            int LastIndex  = TestDevice6;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, _MockNextScenario.Object, _MockAutoScenario.Object, _MockIdleScenario.Object );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
                new List<int> {  },
                new List<int> {  }
            };

            TestScenarioControl.EScenario += TestScenarioControl_EScenarioEmptyList;

            // TURN ON
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            // NEXT SCENARIO
            _MockNextScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            Assert.AreEqual( 0, TestNumber );
        }

        [TestMethod]
        public void ScenarioOneDeviceList_UnitTest( )
        {
            TestIndex = 0;
            int FirstIndex = TestDevice1;
            int LastIndex  = TestDevice6;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, _MockNextScenario.Object, _MockAutoScenario.Object, _MockIdleScenario.Object );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
                new List<int> { 0 },
                new List<int> { 0 }
            };

            TestScenarioControl.EScenario += TestScenarioControl_EScenarioEmptyList;

            // TURN ON
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            // NEXT SCENARIO
            _MockNextScenario.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            Assert.AreEqual( 0, TestNumber );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetScenarioOutOfRange_UnitTest()
        {
            TestIndex = 0;
            int TestValueFirstScenario = 1;
            int TestValueSecondScenario = 2;
            int FirstIndex = TestDevice1;
            int LastIndex = TestDevice6;
            int InvalidScenarioNumber = 99;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, _MockNextScenario.Object, _MockAutoScenario.Object, _MockIdleScenario.Object );
            TestScenarioControl.Number = InvalidScenarioNumber;
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
                new List<int> { TestValueFirstScenario },
                new List<int> { TestValueSecondScenario }
            };

            TestScenarioControl.Scenarios[TestScenarioControl.Number][0] = 1;
        }

        [TestMethod]
        public void SetFirstScenario_UnitTest()
        {
            TestIndex = 0;
            int TestValueFirstScenario  = 1;
            int TestValueSecondScenario = 2;
            int InvalidScenarioNumber = 99;
            int FirstIndex = TestDevice1;
            int LastIndex = TestDevice6;
            int TestSelectedScenarioNumber = 0;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, _MockNextScenario.Object, _MockAutoScenario.Object, _MockIdleScenario.Object );
            TestScenarioControl.Number = InvalidScenarioNumber;
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
                new List<int> { TestValueFirstScenario },
                new List<int> { TestValueSecondScenario }
            };

            TestScenarioControl.SetSecenarioNumber( TestSelectedScenarioNumber );

            Assert.AreEqual( TestSelectedScenarioNumber, TestScenarioControl.Number );
        }

        private void TestScenarioControl_EScenarioEmptyList( object sender, ScenarioEventArgs e )
        {
            TestNumber = e.Number;
        }

        [TestMethod]
        public void TurnScenarioDevicesOnVia_HardwareWatcher_UnitTest()
        {
            TestIndex = 0;

            int FirstIndex = TestDevice1;
            int LastIndex = TestDevice6;
            bool TestValue1=false, TestValue2=false, TestValue3=false;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, new Timer_( 1 ), new Timer_( 1 ), new Timer_( 1 ) );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
              // scenario 1
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              // scenario 2
              new List<int> { TestDevice4, TestDevice5, TestDevice6 }
            };

            TestScenarioControl.EScenario += ( sender, e ) =>
            {
                TestIndex = e.Index;
                switch( TestIndex )
                {
                    case 1:
                        TestValue1 = e.Value;
                        break;
                    case 2:
                        TestValue2 = e.Value;
                        break;
                    case 3:
                        TestValue3 = e.Value;
                        break;
                }
            };

            // TURN ON
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            Assert.IsTrue( TestValue1 );
            Assert.IsTrue( TestValue2 );
            Assert.IsTrue( TestValue3 );

        }

        [TestMethod]
        public void TurnScenarioDevicesOffVia_HardwareWatcher_UnitTest()
        {
            TestIndex = 0;

            int FirstIndex = TestDevice1;
            int LastIndex = TestDevice6;
            bool TestValue1 = true, TestValue2 = true, TestValue3 = true;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, new Timer_( 1 ), new Timer_( 1 ), new Timer_( 1 ) );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
              // scenario 1
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              // scenario 2
              new List<int> { TestDevice4, TestDevice5, TestDevice6 }
            };

            TestScenarioControl.EScenario += ( sender, e ) =>
            {
                TestIndex = e.Index;
                switch (TestIndex)
                {
                    case 1:
                        TestValue1 = e.Value;
                        break;
                    case 2:
                        TestValue2 = e.Value;
                        break;
                    case 3:
                        TestValue3 = e.Value;
                        break;
                }
            };

            // TURN ON
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // TURN OFF
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            Assert.IsFalse( TestValue1 );
            Assert.IsFalse( TestValue2 );
            Assert.IsFalse( TestValue3 );
        }

        [TestMethod]
        public void TurnScenarioDevicesOnVia_HardwareWatcher_OffViaRemoteControl_UnitTest()
        {
            TestIndex = 0;

            int FirstIndex = TestDevice1;
            int LastIndex = TestDevice6;
            bool TestValue1 = false, TestValue2 = false, TestValue3 = false;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, new Timer_( 1 ), new Timer_( 1 ), new Timer_( 1 ) );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
              // scenario 1
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              // scenario 2
              new List<int> { TestDevice4, TestDevice5, TestDevice6 }
            };

            TestScenarioControl.EScenario += ( sender, e ) =>
            {
                TestIndex = e.Index;
                switch (TestIndex)
                {
                    case TestDevice1:
                        TestValue1 = e.Value;
                        break;
                    case TestDevice2:
                        TestValue2 = e.Value;
                        break;
                    case TestDevice3:
                        TestValue3 = e.Value;
                        break;
                }
            };

            // TURN ON
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // TURN OFF
            TestScenarioControl.TurnScenario( false, 1 );

            Assert.IsFalse( TestValue1 );
            Assert.IsFalse( TestValue2 );
            Assert.IsFalse( TestValue3 );
        }

        [TestMethod]
        public void TurnScenarioDevicesOnVia_HardwareWatcher_OffViaRemoteControl_OnAgainViaHardwareWatch_UnitTest()
        {
            TestIndex = 0;

            int FirstIndex = TestDevice1;
            int LastIndex = TestDevice6;
            bool TestValue1Scenario1 = false, TestValue2Scenario1 = false, TestValue3 = false;

            TestScenarioControl = new DeviceScenarioControl( FirstIndex, LastIndex, new Timer_( 1 ), new Timer_( 1 ), new Timer_( 1 ) );
            TestScenarioControl.Scenarios = new List<List<int>>( )
            {
              // scenario 1
              new List<int> { TestDevice1, TestDevice2, TestDevice3 },
              // scenario 2
              new List<int> { TestDevice4, TestDevice5, TestDevice6 }
            };

            TestScenarioControl.EScenario += ( sender, e ) =>
            {
                TestIndex = e.Index;
                switch (TestIndex)
                {
                    case TestDevice1:
                        TestValue1Scenario1 = e.Value;
                        break;
                    case TestDevice2:
                        TestValue2Scenario1 = e.Value;
                        break;
                    case TestDevice3:
                        TestValue3 = e.Value;
                        break;
                }
            };

            // TURN ON
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            // TURN OFF
            TestScenarioControl.TurnScenario( false, 1 );

            // TURN ON
            TestScenarioControl.WatchForInputValueChange( Edge.Rising );
            TestScenarioControl.WatchForInputValueChange( Edge.Falling );

            Assert.IsTrue( TestValue1Scenario1 );
            Assert.IsTrue( TestValue2Scenario1 );
            Assert.IsTrue( TestValue3 );
        }


    }
}
