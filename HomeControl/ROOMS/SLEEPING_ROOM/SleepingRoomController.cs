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
        DeviceScenarioControl     _DeviceScenarioControl;
        ExtendedLightCommander    _LightCommander;

        IDeviceControlTimer       DeviceControlTimer;
        IIOHandler                IOHandler;
        IUdpBasic                 Communicator;
        IExtendedLightCommander   LightCommander;
        IDeviceScenarioControl    DeviceScenarioControl;
        IHeaterCommander          Heater;

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
        public SleepingRoomController( SleepingRoomConfiguration config, 
                                       IIOHandler                iOHandler, 
                                       IUdpBasic                 communicator,
                                       IExtendedLightCommander   lightCommander, 
                                       IDeviceScenarioControl    deviceScenarioControl,
                                       IHeaterCommander          heaterCommander ) : base()
        {
            IOHandler = iOHandler;
            LightCommander = lightCommander;
            Heater = heaterCommander;
            Constructor(config);
            IOHandler.EDigitalInputChanged  += DigitalInputChanged;
            IOHandler.EDigitalOutputChanged += DigitalOutputChanged;
            Communicator = communicator;
            Communicator.EDataReceived += DataReceived;
            Heater.EUpdate += ExtUpdate;
        }

        void Constructor(BaseConfiguration config)
        {
            #region LIGHTCOMMANDER_ANTEROOM
            _config = (SleepingRoomConfiguration) config;
            TimeTurnOn           =  _config.RoomConfig.LightCommanderConfiguration.DelayTimeAllOn;
            TimeTurnAutomaticOff = _config.RoomConfig.LightCommanderConfiguration.DelayTimeOffByMissingTriggerSignal;
            TimeTurnFinalOff     = _config.RoomConfig.LightCommanderConfiguration.DelayTimeFinalOff;
            Startindex           = _config.RoomConfig.LightCommanderConfiguration.Startindex;
            Lastindex            = _config.RoomConfig.LightCommanderConfiguration.Lastindex;
            TimeNextScenario     = _config.RoomConfig.ScenarioConfiguration.DelayTimeNextScenario;
            IdleScenario         = _config.RoomConfig.LightCommanderConfiguration.DelayTimeDoingNothing;
            NotUsed              = 1; // TODO

            _DeviceScenarioControl             = new DeviceScenarioControl(Startindex, 
                                                                           Lastindex,
                                                                           new Timer_(TimeNextScenario), 
                                                                           new Timer_(NotUsed), 
                                                                           new Timer_(IdleScenario));

            DeviceControlTimer                 = new DeviceControlTimer(new Timer_(TimeTurnOn), 
                                                                        new Timer_(TimeTurnAutomaticOff), 
                                                                        new Timer_(TimeTurnFinalOff));
            //_LightCommander                    = new ExtendedLightCommander(_config.RoomConfig.LightCommanderConfiguration, DeviceControlTimer, _DeviceScenarioControl);
            _DeviceScenarioControl.Scenarios   = _config.RoomConfig.ScenarioConfiguration.Scenarios;
            //LightCommander.AvailableScenarios = _config.RoomConfig.ScenarioConfiguration.Scenarios;
            LightCommander.ExtUpdate += ExtUpdate; ;
            #endregion
        }

        void RoomController(int index, bool value)
        {
            switch (index)
            {
                case IOAssignmentControllerSleepingRoom.indDigitalInputMainButton:
                    _ScenarioNumber = (int)LightCommander?.ScenarioTrigger( value );
                    Heater?.MainTrigger( value );
                    break;
            }
        }



        #region EVENTHANDLERS
        private void ExtUpdate(object sender, UpdateEventArgs e)
        {
            IOHandler?.UpdateDigitalOutputs(e.Index, e.Value);
        }

        private void DataReceived(object sender, DataReceivingEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DigitalInputChanged(object sender, DigitalInputEventargs e)
        {
            RoomController(e.Index, e.Value);
        }

        private void DigitalOutputChanged(object sender, DigitalOutputEventargs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region PROPERTIES
        public int ScenarioNumber { get => _ScenarioNumber; set => _ScenarioNumber = value; }
        #endregion
    }
}
