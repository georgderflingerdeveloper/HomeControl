using Moq;
using HomeControl.ADVANCED_COMPONENTS;
using BASIC_COMPONENTS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Timers;

namespace HomeControl.UNIT_TESTS.ADVANCED_COMPONENTS
{
    [TestClass]
    public class unittest_devicecontroltimer_basic
    {
        Mock<ITimer> MockTimer_AllOn         = new Mock<ITimer>();
        Mock<ITimer> MockTimer_SingleTurnOff = new Mock<ITimer>();

        bool TestFlagAllOn = false;

        [TestMethod]
        public void Test_AllOn()
        {
            DeviceControlTimer TestControlTimer = new DeviceControlTimer( MockTimer_AllOn.Object, MockTimer_SingleTurnOff.Object );
            TestControlTimer.EControlOn += TestControlTimer_AllOn_;
            TestControlTimer.StartOn( );
            MockTimer_AllOn.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( TestFlagAllOn );
        }

        private void TestControlTimer_AllOn_( object sender, ElapsedEventArgs e )
        {
            Assert.IsNotNull( sender );
            TestFlagAllOn = true;
        }
    }
}
