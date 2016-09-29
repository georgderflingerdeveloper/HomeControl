using Moq;
using NUnit.Framework;
using BASIC_COMPONENTS;

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

        void SetupPwmController( )
        {
            TestController                    = new PwmController( MockedTimerOn.Object, MockedTimerOff.Object, TestTimeOn, TestTimeOff, Mode.eStartWithOn );
            TestPwmControllerEventArgs        = new PwmControllerEventArgs( );
            TestController.EAnyStatusChanged += TestController_EAnyStatusChanged;
        }


        [Test]
        public void TestPwmController_PwmStatus_On_Start( )
        {
            SetupPwmController( );

            TestController.Start( );

            Assert.AreEqual( PwmStatus.eActive, TestPwmControllerEventArgs.Status );
        }

        [Test]
        public void TestPwmController_SwitchStatus_On_Start( )
        {
            SetupPwmController( );

            TestController.Start( );

            Assert.AreEqual( SwitchStatus.eOn, TestPwmControllerEventArgs.SwitchStatus );
        }

        private void TestController_EAnyStatusChanged( object sender, PwmControllerEventArgs e )
        {
            TestPwmControllerEventArgs.Status        = e.Status;
            TestPwmControllerEventArgs.SwitchStatus  = e.SwitchStatus;
        }
    }
}
