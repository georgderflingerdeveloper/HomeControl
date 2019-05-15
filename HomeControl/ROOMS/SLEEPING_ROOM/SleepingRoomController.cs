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
        DeviceScenarioControl _DeviceScenarioControl;
        ExtendedLightCommander _LightCommander;
        IDeviceControlTimer _DeviceControlTimer;

        IIOHandler _IOHandler;
        IUdpBasic _Communicator;
        int    _ScenarioNumber;
        double TimeTurnOn;
        double TimeTurnAutomaticOff;
        double TimeTurnFinalOff;
        int    Startindex;
        int    Lastindex;
        double TimeNextScenario;
        double IdleScenario;
        double NotUsed;


        #endregion

        public SleepingRoomController(SleepingRoomConfiguration config, IIOHandler IOHandler, IUdpBasic Communicator) : base()
        {
            Constructor(config);
            _IOHandler = IOHandler;
            _IOHandler.EDigitalInputChanged  += IOHandler_EDigitalInputChanged;
            _IOHandler.EDigitalOutputChanged += IOHandler_EDigitalOutputChanged;
            _Communicator = Communicator;
            _Communicator.EDataReceived += Communicator_EDataReceived;
        }

        void Constructor(BaseConfiguration config)
        {
            #region LIGHTCOMMANDER_ANTEROOM
            _config = (SleepingRoomConfiguration) config;
            TimeTurnOn           = _config.RoomConfig.LightCommanderConfiguration.DelayTimeAllOn;
            TimeTurnAutomaticOff = _config.RoomConfig.LightCommanderConfiguration.DelayTimeOffByMissingTriggerSignal;
            TimeTurnFinalOff     = _config.RoomConfig.LightCommanderConfiguration.DelayTimeFinalOff;
            Startindex           = _config.RoomConfig.LightCommanderConfiguration.Startindex;
            Lastindex            = _config.RoomConfig.LightCommanderConfiguration.Lastindex;
            TimeNextScenario     = _config.RoomConfig.ScenarioConfiguration.DelayTimeNextScenario;
            IdleScenario         = _config.RoomConfig.LightCommanderConfiguration.DelayTimeDoingNothing;
            NotUsed              = 1; // TODO

            _DeviceScenarioControl             = new DeviceScenarioControl(Startindex, Lastindex, new Timer_(TimeNextScenario), new Timer_(NotUsed), new Timer_(IdleScenario));
            _DeviceControlTimer                = new DeviceControlTimer(new Timer_(TimeTurnOn), new Timer_(TimeTurnAutomaticOff), new Timer_(TimeTurnFinalOff));
            _LightCommander                    = new ExtendedLightCommander(_config.RoomConfig.LightCommanderConfiguration, _DeviceControlTimer, _DeviceScenarioControl);
            _DeviceScenarioControl.Scenarios   = _config.RoomConfig.ScenarioConfiguration.Scenarios;
            _LightCommander.AvailableScenarios = _config.RoomConfig.ScenarioConfiguration.Scenarios;
            _LightCommander.ExtUpdate += _LightCommander_ExtUpdate; ;
            #endregion
        }

        void RoomController(int index, bool value)
        {
            switch (index)
            {
                case IOAssignmentControllerSleepingRoom.indDigitalInputMainButton:
                    _ScenarioNumber = (int)_LightCommander?.ScenarioTrigger(value);
                    break;
            }
        }



        #region EVENTHANDLERS
        private void _LightCommander_ExtUpdate(object sender, UpdateEventArgs e)
        {
            _IOHandler?.UpdateDigitalOutputs(e.Index, e.Value);
        }

        private void Communicator_EDataReceived(object sender, DataReceivingEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void IOHandler_EDigitalInputChanged(object sender, DigitalInputEventargs e)
        {
            RoomController(e.Index, e.Value);
        }

        private void IOHandler_EDigitalOutputChanged(object sender, DigitalOutputEventargs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region PROPERTIES
        public int ScenarioNumber { get => _ScenarioNumber; set => _ScenarioNumber = value; }
        #endregion
    }
}
