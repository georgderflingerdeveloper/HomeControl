using Moq;
using HomeControl.ADVANCED_COMPONENTS;
using BASIC_COMPONENTS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Timers;


namespace HomeControl.UNIT_TESTS.ADVANCED_COMPONENTS
{
    [TestClass]
    public class unittest_deviceblinker
    {
        Mock<ITimer> _MockTimer = new Mock<ITimer>();
        DeviceBlinker Blinker;
        BlinkerConfiguration Config;
        int TestIndex = 0;
        bool Result;

        [TestMethod]
        public void TestBlink_StartFalse_Initial( )
        {
            Result = true;
            Config = new BlinkerConfiguration( TestIndex, StartBlinker.eWithOffPeriode );
            Blinker = new DeviceBlinker( Config, _MockTimer.Object );
            Blinker.EUpdate += Blinker_EUpdate;
            Blinker.Start( );
            Assert.IsFalse( Result );
        }

        [TestMethod]
        public void TestBlink_StartFalse_FirstPulse( )
        {
            Result = true;
            Config = new BlinkerConfiguration( TestIndex, StartBlinker.eWithOffPeriode );
            Blinker = new DeviceBlinker( Config, _MockTimer.Object );
            Blinker.EUpdate += Blinker_EUpdate;
            Blinker.Start( );
            _MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( Result );
        }

        [TestMethod]
        public void TestBlink_StartFalse_OnePeriod( )
        {
            Result = true;
            Config = new BlinkerConfiguration( TestIndex, StartBlinker.eWithOffPeriode );
            Blinker = new DeviceBlinker( Config, _MockTimer.Object );
            Blinker.EUpdate += Blinker_EUpdate;
            Blinker.Start( );
            _MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );
            _MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );
            Assert.IsFalse( Result );
        }

        [TestMethod]
        public void TestBlink_StartFalse_OnePeriod_TurnOff_Next( )
        {
            Result = true;
            Config = new BlinkerConfiguration( TestIndex, StartBlinker.eWithOffPeriode );
            Blinker = new DeviceBlinker( Config, _MockTimer.Object );
            Blinker.EUpdate += Blinker_EUpdate;
            Blinker.Start( );
            _MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );
            _MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );
            _MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );
            Blinker.Stop( );
            Assert.IsFalse( Result );
        }


        [TestMethod]
        public void TestBlink_StartFalse( )
        {
            Result = true;
            Config = new BlinkerConfiguration( TestIndex, StartBlinker.eWithOffPeriode );
            Blinker = new DeviceBlinker( Config, _MockTimer.Object );
            Blinker.EUpdate += Blinker_EUpdate;
            Blinker.Start( );
            Assert.IsFalse( Result );
            _MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( Result );
            _MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );
            Assert.IsFalse( Result );
            _MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( Result );
            Blinker.Stop( );
            Assert.IsFalse( Result );
        }

        [TestMethod]
        public void TestBlink_StartTrue( )
        {
            Result = false;
            Config = new BlinkerConfiguration( TestIndex, StartBlinker.eWithOnPeriode );
            Blinker = new DeviceBlinker( Config, _MockTimer.Object );
            Blinker.EUpdate += Blinker_EUpdate;
            Blinker.Start( );
            Assert.IsTrue( Result );
            _MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );
            Assert.IsFalse( Result );
            _MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( Result );
            _MockTimer.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );
            Assert.IsFalse( Result );

            Result = true;
            Blinker.Stop( );
            Assert.IsFalse( Result );
        }

        private void Blinker_EUpdate( object sender, UpdateEventArgs e )
        {
            Result = e.Value;
        }

    }
}
