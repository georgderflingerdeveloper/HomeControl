using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace HomeControl.ROOMS.CONFIGURATION.UNIT_TEST
{
    [TestClass]
    public class AnteBathWashRoomConfiguration_UnitTest
    {
        [TestMethod]
        public void TestDeviceName_DigitalInput0()
        {
            string TestDeviceName = IOAssignmentControllerAnteBathWashRoom.GetOutputDeviceName(0);
            Assert.AreEqual(nameof(IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomMainLight), TestDeviceName);
        }
    }
}
