using BASIC_COMPONENTS;
using HomeControl.ADVANCED_COMPONENTS.Interfaces;
using HomeControl.BASIC_COMPONENTS.Interfaces;
using HomeControl.ADVANCED_COMPONENTS;
using BASIC_CONTROL_LOGIC;
using HomeControl.BASIC_CONSTANTS;
using HomeControl.ROOMS.ANTE_BATH_WASH_ROOM_CONTROLLER.INTERFACE;
using LibUdp.BASIC.INTERFACE;
using LibUdp.BASIC.RECEIVE;
using HomeAutomationProtocoll;
using System;
using SystemServices;
using HardConfig;
using HomeControl.ROOMS.CONFIGURATION;

namespace HomeControl.ROOMS.SLEEPING_ROOM
{
    class SleepingRoomController : Controller
    {
        #region DECLARATION
        SleepingRoomConfiguration _config;
        IIOHandler _IOHandler;
        IUdpBasic _Communicator;
        int _ScenarioNumber;

        #endregion

        public SleepingRoomController(SleepingRoomConfiguration config, IIOHandler IOHandler, IUdpBasic Communicator) : base()
        {
            _config = config;
            _IOHandler = IOHandler;
            _IOHandler.EDigitalInputChanged  += IOHandler_EDigitalInputChanged;
            _IOHandler.EDigitalOutputChanged += IOHandler_EDigitalOutputChanged;
            //Constructor();
            _Communicator = Communicator;
            _Communicator.EDataReceived += Communicator_EDataReceived;
        }

        private void Communicator_EDataReceived(object sender, DataReceivingEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void IOHandler_EDigitalInputChanged(object sender, DigitalInputEventargs e)
        {
            throw new NotImplementedException();
        }

        private void IOHandler_EDigitalOutputChanged(object sender, DigitalOutputEventargs e)
        {
            throw new NotImplementedException();
        }

        #region PROPERTIES
        public int ScenarioNumber { get => _ScenarioNumber; set => _ScenarioNumber = value; }
        #endregion
    }
}
