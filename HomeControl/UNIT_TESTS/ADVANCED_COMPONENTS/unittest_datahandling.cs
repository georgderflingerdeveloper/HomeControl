using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.BASIC_CONSTANTS;
using BASIC_COMPONENTS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeControl.ROOMS;
using HomeControl.BASIC_COMPONENTS;
using SystemServices;

namespace HomeControl.UNIT_TESTS.ADVANCED_COMPONENTS
{

    [TestClass]
    public class unittest_datahandling
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

        [TestMethod]
        public void LoadTestData( )
        {
            object readdata =  DataHandler_.Load( LoadedAnteBathWashRoomConfiguration_, FileNameAnteRoom );
            LoadedAnteBathWashRoomConfiguration_ = readdata as AnteBathWashRoomConfiguration;
            Assert.IsTrue( Memory.CompareObjects( AnteBathWashRoomConfiguration_, LoadedAnteBathWashRoomConfiguration_ ) );
        }

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
}
