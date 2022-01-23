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

        [TestMethod]
        public void TestDeviceName_DigitalInput_InvalidNegaive()
        {
            string TestDeviceName = IOAssignmentControllerAnteBathWashRoom.GetOutputDeviceName(-1);
            string ExpectedInfo = "Key" + "-1" + "is not found";
            Assert.AreEqual(ExpectedInfo, TestDeviceName);
        }
    }
}