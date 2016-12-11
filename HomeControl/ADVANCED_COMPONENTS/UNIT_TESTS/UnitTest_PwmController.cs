using Moq;
using NUnit.Framework;
using BASIC_COMPONENTS;
using System;
using System.Timers;

namespace HomeControl.ADVANCED_COMPONENTS.UNIT_TESTS
{
    [TestFixture]
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


        [Test]
        public void PwmStatus_On_Start( )
        {
            SetupMockedPwmController( );

            TestController.Start( );

            Assert.AreEqual( PwmStatus.eActive, TestPwmControllerEventArgs.Status );
        }

        [Test]
        public void SwitchStatus_On_Start( )
        {
            SetupMockedPwmController( );

            TestController.Start( );

            Assert.AreEqual( SwitchStatus.eOn, TestPwmControllerEventArgs.SwitchStatus );
        }

        [Test]
        public void SwitchStatus_On_IsStarted( )
        {
            SetupPwmControllerWithTimeBaseZero( );

            TestController.Start( );

            Assert.True( TestController.TimerOnIsStarted );
        }

        [Test]
        public void SwitchStatus_On_TimeElapsed( )
        {
            SetupMockedPwmController( );

            TestController.Start( );

            MockedTimerOn.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );

            Assert.AreEqual( SwitchStatus.eOff, TestPwmControllerEventArgs.SwitchStatus );
        }

        [Test]
        public void SwitchStatus_On_TimeElapsed_TimerOnIsStopped( )
        {
            SetupMockedPwmController( );

            TestController.Start( );

            MockedTimerOn.Raise( timer => timer.Elapsed += null, new EventArgs( ) as ElapsedEventArgs );

            Assert.False( TestController.TimerOnIsStarted );
        }

        private void TestController_EAnyStatusChanged( object sender, PwmControllerEventArgs e )
        {
            TestPwmControllerEventArgs.Status        = e.Status;
            TestPwmControllerEventArgs.SwitchStatus  = e.SwitchStatus;
        }
    }
}
