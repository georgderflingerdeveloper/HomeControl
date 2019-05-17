using Moq;
using BASIC_COMPONENTS;
using System;
using System.Timers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeControl.ADVANCED_COMPONENTS.UNIT_TESTS
{
    [TestClass]
    public class UnitTest_PwmController
    {
        Mock<ITimer> MockedTimerOn  = new Mock<ITimer>();
        Mock<ITimer> MockedTimerOff = new Mock<ITimer>();

        double TestTimeOn  = 1000;
        double TestTimeOff = 2000;

        PwmController          TestController;
        PwmControllerEventArgs TestPwmControllerEventArgs;

        public UnitTest_PwmController()
        {
        }

        void SetupMockedPwmController( )
        {
            TestController                    = new PwmController( MockedTimerOn.Object, MockedTimerOff.Object, TestTimeOn, TestTimeOff );
            TestPwmControllerEventArgs        = new PwmControllerEventArgs( );
            TestController.EAnyStatusChanged += TestController_EAnyStatusChanged;
        }

        void SetupPwmControllerWithTimeBaseZero( )
        {
            TestController = new PwmController( new Timer_( 0 ), new Timer_( 0 ), TestTimeOn, TestTimeOff );
            TestPwmControllerEventArgs = new PwmControllerEventArgs( );
            TestController.EAnyStatusChanged += TestController_EAnyStatusChanged;
        }


        [TestMethod]
        public void PwmStatus_On_Start( )
        {
            SetupMockedPwmController( );

            TestController.Start( );

            Assert.AreEqual( PwmStatus.eActive, TestPwmControllerEventArgs.Status );
        }

        [TestMethod]
        public void SwitchStatus_On_Start( )
        {
            SetupMockedPwmController( );

            TestController.Start( );

            Assert.AreEqual( SwitchStatus.eOn, TestPwmControllerEventArgs.SwitchStatus );
        }

        [TestMethod]
        public void SwitchStatus_On_IsStarted( )
        {
            SetupPwmControllerWithTimeBaseZero( );

            TestController.Start( );

            Assert.IsTrue( TestController.TimerOnIsStarted );
        }

        [TestMethod]
        public void SwitchStatus_On_TimeElapsed( )
        {
            SetupMockedPwmController( );

            TestController.Start( );

            MockedTimerOn.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );

            Assert.AreEqual( SwitchStatus.eOff, TestPwmControllerEventArgs.SwitchStatus );
        }

        [TestMethod]
        public void SwitchStatus_On_TimeElapsed_TimerOnIsStopped( )
        {
            SetupMockedPwmController( );

            TestController.Start( );

            MockedTimerOn.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );

            Assert.IsFalse( TestController.TimerOnIsStarted );
        }

        private void TestController_EAnyStatusChanged( object sender, PwmControllerEventArgs e )
        {
            TestPwmControllerEventArgs.Status        = e.Status;
            TestPwmControllerEventArgs.SwitchStatus  = e.SwitchStatus;
        }
    }
}
