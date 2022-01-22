using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeControl.ROOMS;
using HomeControl.BASIC_COMPONENTS;
using SystemServices;
using HomeControl.ROOMS.CONFIGURATION;

namespace HomeControl.UNIT_TESTS.ADVANCED_COMPONENTS
{
   [TestClass]
   public class unittest_datahandling_anteroom
    {
        AnteBathWashRoomConfiguration AnteBathWashRoomConfiguration_       = new AnteBathWashRoomConfiguration();
        AnteBathWashRoomConfiguration LoadedAnteBathWashRoomConfiguration_ = new AnteBathWashRoomConfiguration();
        DataHandler DataHandler_ = new DataHandler();
        ControllerConfiguration Controller = new ControllerConfiguration();
        string FileNameAnteRoom   = "anteroom.xml";
        string FileNameController = "controller.xml";

        // create xml file only
        [TestMethod]
        public void StoreTestData()
        {
           DataHandler_.Store( AnteBathWashRoomConfiguration_, FileNameAnteRoom );
        }

        //[TestMethod]
        //public void LoadTestData( )
        //{
        //    object readdata =  DataHandler_.Load( LoadedAnteBathWashRoomConfiguration_, FileNameAnteRoom );
        //    LoadedAnteBathWashRoomConfiguration_ = readdata as AnteBathWashRoomConfiguration;
        //    Assert.IsTrue( Memory.CompareObjects( AnteBathWashRoomConfiguration_, LoadedAnteBathWashRoomConfiguration_ ) );
        //}

        // create xml file only
        [TestMethod]
        public void StoreTestData_Commander( )
        {
            Controller.IpAdressServer = "192.168.0.111";
            Controller.PortServer = 55100;
            Controller.SelectedRoom = "ANTEROOM";
            DataHandler_.Store( Controller, FileNameController );
        }
    }

    [TestClass]
    public class unittest_datahandling_sleepingroom
{
    SleepingRoomConfiguration SleepingRoomConfiguration_ = new SleepingRoomConfiguration();
    SleepingRoomConfiguration LoadedSleepingRoomconfig_ = new SleepingRoomConfiguration();
    DataHandler DataHandler_ = new DataHandler();
    ControllerConfiguration Controller = new ControllerConfiguration();
    string FileNameAnteRoom = "sleepingroom.xml";
    string FileNameController = "sleepingroomcontroller.xml";

    // create xml file only
    [TestMethod]
    public void StoreTestData()
    {
        DataHandler_.Store(SleepingRoomConfiguration_, FileNameAnteRoom);
    }

    //[TestMethod]
    //public void LoadTestData()
    //{
    //    object readdata = DataHandler_.Load(LoadedSleepingRoomconfig_, FileNameAnteRoom);
    //    LoadedSleepingRoomconfig_ = readdata as SleepingRoomConfiguration;
    //    Assert.IsTrue(Memory.CompareObjects(SleepingRoomConfiguration_, LoadedSleepingRoomconfig_));
    //}

    // create xml file only
    [TestMethod]
    public void StoreTestData_Commander()
    {
        Controller.IpAdressServer = "192.168.0.111";
        Controller.PortServer = 55100;
        Controller.SelectedRoom = "SLEEPINGROOM";
        DataHandler_.Store(Controller, FileNameController);
    }
  }

}