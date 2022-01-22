using BASIC_COMPONENTS;
using BASIC_CONTROL_LOGIC;
using HardConfig;
using HomeAutomationProtocoll;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.ADVANCED_COMPONENTS.Interfaces;
using HomeControl.BASIC_COMPONENTS.Interfaces;
using HomeControl.BASIC_CONSTANTS;
using HomeControl.ROOMS.ANTE_BATH_WASH_ROOM_CONTROLLER.INTERFACE;
using LibUdp.BASIC.INTERFACE;
using LibUdp.BASIC.RECEIVE;
using NUnit.Framework;
using System;
using SystemServices;
using System.Collections.Generic;
using System.ComponentModel;

namespace HomeControl.ROOMS
{
    class AnteBathWashRoomController : Controller, IAnteBathWashRoomController
    {
        #region DECLARATION
        AnteBathWashRoomConfiguration _config;
        IIOHandler                    _IOHandler;
        IIOHandler[]                  _IOHandlerMulti;
        ExtendedLightCommander        _LightCommanderAnteRoom;
        ExtendedLightCommander        _LightCommanderBathRoom;
        HeaterCommander               _HeaterCommanderBathRoom;
        IDeviceControlTimer           _DeviceControlTimerHeaterBathRoom;
        ExtendedLightCommander        _LightCommanderWashRoom;
        DeviceScenarioControl         _DeviceScenarioControlAnteRoom;
        IDeviceControlTimer           _DeviceControlTimerAnteRoom;
        DeviceScenarioControl         _DeviceScenarioControlBathRoom;
        IDeviceControlTimer           _DeviceControlTimerBathRoom;
        DeviceScenarioControl         _DeviceScenarioControlWashRoom;
        IDeviceControlTimer           _DeviceControlTimerWashRoom;
        IDeviceBlinker                _HeartBeat;
        LightCommander                _PresenceLight;
        IDeviceControlTimer           _DeviceControlTimerPresenceLight;
        IUdpBasic                     _Communicator;
        UpdateEventArgs               _FeedbackArgs = new UpdateEventArgs();

        double TimeTurnOn;
        double TimeTurnAutomaticOff;
        double TimeTurnFinalOff;
        int Startindex;
        int Lastindex;
        double TimeNextScenario;
        double IdleScenario;
        double NotUsed;
        bool MultiIOCardsAvailable = false;
        int _ScenarioNumberAnteRoom;
        int _ScenarioNumberBathRoom;
        int _ScenarioNumberWashRoom;
        const int InvalidIndex = 999;
        #endregion

        #region CONSTRUCTOR
        public AnteBathWashRoomController(AnteBathWashRoomConfiguration config,
                                          IDeviceBlinker HeartBeat,
                                          IIOHandler     IOHandler,
                                          IUdpBasic      Communicator) : base()
        {
            _HeartBeat = HeartBeat;
            _config    = config;
            _IOHandler = IOHandler;
            _IOHandler.EDigitalInputChanged  += DigitalInputChanged;
            _IOHandler.EDigitalOutputChanged += DigitalOutputChanged;
            Constructor();
            HeartBeat.EUpdate += Update;
            _Communicator = Communicator;
            _Communicator.EDataReceived += DataReceived;

            HeartBeat.Start();
        }

        public AnteBathWashRoomController(AnteBathWashRoomConfiguration config,
                                          IDeviceBlinker HeartBeat,
                                          IIOHandler[] IOHandler) : base()
        {
            MultiIOCardsAvailable = true;
            _HeartBeat      = HeartBeat;
            _config         = config;
            _IOHandlerMulti = IOHandler;

            for (int i = 0; i < IOHandler.Length; i++)
            {
                _IOHandlerMulti[i].EDigitalInputChanged += DigitalInputChanged;
            }

            Constructor();
            HeartBeat.EUpdate += Update;
            HeartBeat.Start();
        }
        #endregion

        #region PROPERTIES
        public int ScenarioNumberAnteRoom
        {
            get
            {
                return _ScenarioNumberAnteRoom;
            }

            set
            {
                _ScenarioNumberAnteRoom = value;
            }
        }

        public int ScenarioNumberBathRoom
        {
            get
            {
                return _ScenarioNumberBathRoom;
            }

            set
            {
                _ScenarioNumberBathRoom = value;
            }
        }

        public int ScenarioNumberWashRoom
        {
            get
            {
                return _ScenarioNumberWashRoom;
            }

            set
            {
                _ScenarioNumberWashRoom = value;
            }
        }
        #endregion

        #region PRIVATE_METHODS
        void Constructor()
        {
            #region LIGHTCOMMANDER_ANTEROOM
            TimeTurnOn           = _config.AnteRoom.LightCommanderConfiguration.DelayTimeAllOn;
            TimeTurnAutomaticOff = _config.AnteRoom.LightCommanderConfiguration.DelayTimeOffByMissingTriggerSignal;
            TimeTurnFinalOff     = _config.AnteRoom.LightCommanderConfiguration.DelayTimeFinalOff;
            Startindex           = _config.AnteRoom.LightCommanderConfiguration.Startindex;
            Lastindex            = _config.AnteRoom.LightCommanderConfiguration.Lastindex;
            TimeNextScenario     = _config.AnteRoom.ScenarioConfiguration.DelayTimeNextScenario;
            IdleScenario         = _config.AnteRoom.LightCommanderConfiguration.DelayTimeDoingNothing;
            NotUsed = 1; // TODO

            _DeviceScenarioControlAnteRoom = 
                new DeviceScenarioControl(Startindex, 
                                          Lastindex, 
                                          new Timer_(TimeNextScenario), 
                                          new Timer_(NotUsed), 
                                          new Timer_(IdleScenario));

            _DeviceScenarioControlAnteRoom.Scenarios = _config.AnteRoom.ScenarioConfiguration.Scenarios;

            _DeviceControlTimerAnteRoom = 
                new DeviceControlTimer(new Timer_(TimeTurnOn), 
                                       new Timer_(TimeTurnAutomaticOff), 
                                       new Timer_(TimeTurnFinalOff));
            _LightCommanderAnteRoom = 
                new ExtendedLightCommander(_config.AnteRoom.LightCommanderConfiguration,
                                           _DeviceControlTimerAnteRoom, 
                                           _DeviceScenarioControlAnteRoom);

            _LightCommanderAnteRoom.ExtUpdate += Update;
            _LightCommanderAnteRoom.AvailableScenarios = _config.AnteRoom.ScenarioConfiguration.Scenarios;
            
            #endregion

            #region LIGHTCOMMANDER_BATHROOM
            TimeTurnOn           = _config.BathRoom.LightCommanderConfiguration.DelayTimeAllOn;
            TimeTurnAutomaticOff = _config.BathRoom.LightCommanderConfiguration.DelayTimeOffByMissingTriggerSignal;
            TimeTurnFinalOff     = _config.BathRoom.LightCommanderConfiguration.DelayTimeFinalOff;
            Startindex           = _config.BathRoom.LightCommanderConfiguration.Startindex;
            Lastindex            = _config.BathRoom.LightCommanderConfiguration.Lastindex;
            TimeNextScenario     = _config.BathRoom.ScenarioConfiguration.DelayTimeNextScenario;
            IdleScenario         = _config.BathRoom.LightCommanderConfiguration.DelayTimeDoingNothing;
            NotUsed = 1; // TODO

            _DeviceScenarioControlBathRoom 
                = new DeviceScenarioControl(Startindex, 
                                            Lastindex, 
                                            new Timer_(TimeNextScenario), 
                                            new Timer_(NotUsed), 
                                            new Timer_(IdleScenario));

            _DeviceScenarioControlBathRoom.Scenarios 
                = _config.BathRoom.ScenarioConfiguration.Scenarios;

            _DeviceControlTimerBathRoom 
                = new DeviceControlTimer(new Timer_(TimeTurnOn), 
                                         new Timer_(TimeTurnAutomaticOff), 
                                         new Timer_(TimeTurnFinalOff));

            _LightCommanderBathRoom 
                = new ExtendedLightCommander(
                    _config.BathRoom.LightCommanderConfiguration,
                    _DeviceControlTimerBathRoom, 
                    _DeviceScenarioControlBathRoom);

            _LightCommanderBathRoom.ExtUpdate += Update;
            #endregion

            #region LIGHTCOMMANDER_WASHROOM
            TimeTurnOn           = _config.WashRoom.LightCommanderConfiguration.DelayTimeAllOn;
            TimeTurnAutomaticOff = _config.WashRoom.LightCommanderConfiguration.DelayTimeOffByMissingTriggerSignal;
            TimeTurnFinalOff     = _config.WashRoom.LightCommanderConfiguration.DelayTimeFinalOff;
            Startindex           = _config.WashRoom.LightCommanderConfiguration.Startindex;
            Lastindex            = _config.WashRoom.LightCommanderConfiguration.Lastindex;
            TimeNextScenario     = _config.WashRoom.ScenarioConfiguration.DelayTimeNextScenario;
            IdleScenario         = _config.WashRoom.LightCommanderConfiguration.DelayTimeDoingNothing;
            NotUsed = 1; // TODO

            _DeviceScenarioControlWashRoom 
                = new DeviceScenarioControl(Startindex, 
                                            Lastindex, 
                                            new Timer_(TimeNextScenario), 
                                            new Timer_(NotUsed), 
                                            new Timer_(IdleScenario));

            _DeviceScenarioControlWashRoom.Scenarios = _config.WashRoom.ScenarioConfiguration.Scenarios;

            _DeviceControlTimerWashRoom 
                = new DeviceControlTimer(new Timer_(TimeTurnOn), 
                                         new Timer_(TimeTurnAutomaticOff),
                                         new Timer_(TimeTurnFinalOff));

            _LightCommanderWashRoom 
                = new ExtendedLightCommander(_config.WashRoom.LightCommanderConfiguration, 
                                             _DeviceControlTimerWashRoom, 
                                             _DeviceScenarioControlWashRoom);
            _LightCommanderWashRoom.ExtUpdate += Update;
            #endregion

            #region LIGHTCOMMANDER_PRESENCE
            TimeTurnAutomaticOff = _config.PresenceLightConfiguration.DelayTimeOffByMissingTriggerSignal;
            _DeviceControlTimerPresenceLight = new DeviceControlTimer(new Timer_(TimeTurnAutomaticOff));
            _PresenceLight = new LightCommander(_config.PresenceLightConfiguration, 
                                                _DeviceControlTimerPresenceLight);
            _PresenceLight.EUpdate += Update;
            #endregion

            #region HEATERCOMMANDER_BATHROOM
            TimeTurnOn           = _config.HeaterBathRoom.DelayTimeAllOn;
            TimeTurnAutomaticOff = _config.HeaterBathRoom.DelayTimeFinalOff;
            _DeviceControlTimerHeaterBathRoom
                = new DeviceControlTimer(new Timer_(TimeTurnOn),
                                         new Timer_(TimeTurnAutomaticOff));

            _HeaterCommanderBathRoom = new HeaterCommander(_config.HeaterBathRoom, 
                                                           _DeviceControlTimerHeaterBathRoom);
            _HeaterCommanderBathRoom.EUpdate += Update;
            #endregion
        }

        // ALTERNATIVE WITH POLYMORPHISM

        //Remote = new RemoteController(_LightCommanderAnteRoom);

        //interface IRemote
        //{
        //    UpdateEventArgs Execute();
        //}

        //class MainLightOn : IRemote
        //{
        //    IExtendedLightCommander _Commander;
        //    public MainLightOn( IExtendedLightCommander Commander )
        //    {
        //        _Commander = Commander;

        //    }
        //    public UpdateEventArgs Execute()
        //    {
        //        return _Commander.TurnSingleDevice(TurnDevice.ON,
        //                                                 IOAssignmentControllerAnteBathWashRoom
        //                                                 .indDigitalOutputAnteRoomMainLight);
        //    }
        //}

        //class HeaterBath : IRemote
        //{
        //    IHeaterCommander _Commander;
        //    public HeaterBath(IHeaterCommander Commander)
        //    {
        //        _Commander = Commander;

        //    }
        //    public UpdateEventArgs Execute()
        //    {
        //        _Commander.Reset();
        //        _Commander.MainTrigger(TurnDevice.ON);
        //        return null;
        //    }
        //}

        //class RemoteController
        //{
        //    Dictionary<string, IRemote> Controller = new Dictionary<string, IRemote>();

        //    IExtendedLightCommander _LightCommander;
        //    IHeaterCommander _HeaterCommander;
        //    public RemoteController(IExtendedLightCommander LightCommander )
        //    {
        //        _LightCommander = LightCommander;
        //        Controller.Add(ComandoString.TURN_LIGHT_ANTEROOM_MAIN_ON, new MainLightOn(_LightCommander));
        //    }

        //    public RemoteController(IExtendedLightCommander LightCommander, IHeaterCommander HeaterCommander)
        //    {
        //        _HeaterCommander = HeaterCommander;
        //        _LightCommander = LightCommander;
        //        Controller.Add(ComandoString.TURN_LIGHT_ANTEROOM_MAIN_ON, new MainLightOn(_LightCommander));
        //        Controller.Add(ComandoString.TURN_HEATER_BATH_ON, new HeaterBath(_HeaterCommander));
        //    }

        //    public UpdateEventArgs Execute( string command )
        //    {
        //        return Controller[command].Execute();
        //    }

        //}

        // call this at the end
        // _FeedbackArgs = Remote.Execute(e.Message);

        private UpdateEventArgs _RemoteControl(DataReceivingEventArgs e)
        {
            _FeedbackArgs.Index = InvalidIndex;
            _FeedbackArgs.Value = false;

            switch (e.Message)
            {
                case ComandoString.TURN_LIGHT_ANTEROOM_MAIN_ON:
                    _FeedbackArgs = _LightCommanderAnteRoom.TurnSingleDevice(TurnDevice.ON,
                                                         IOAssignmentControllerAnteBathWashRoom
                                                         .indDigitalOutputAnteRoomMainLight);
                    break;

                case ComandoString.TURN_LIGHT_ANTEROOM_MAIN_OFF:
                    _FeedbackArgs = _LightCommanderAnteRoom.TurnSingleDevice(TurnDevice.OFF,
                                                         IOAssignmentControllerAnteBathWashRoom
                                                         .indDigitalOutputAnteRoomMainLight);
                    break;

                case ComandoString.TURN_LIGHT_ANTEROOM_BACK_ON:
                    _FeedbackArgs = _LightCommanderAnteRoom.TurnSingleDevice(TurnDevice.ON,
                                                         IOAssignmentControllerAnteBathWashRoom
                                                         .indDigitalOutputAnteRoomBackSide);
                    break;

                case ComandoString.TURN_LIGHT_ANTEROOM_MIDDLE_OFF:
                    _FeedbackArgs = _LightCommanderAnteRoom.TurnSingleDevice(TurnDevice.OFF,
                                                         IOAssignmentControllerAnteBathWashRoom
                                                         .indDigitalOutputAnteRoomNightLight);
                    break;

                case ComandoString.TURN_LIGHT_ANTEROOM_MIDDLE_ON:
                    _FeedbackArgs = _LightCommanderAnteRoom.TurnSingleDevice(TurnDevice.ON,
                                                         IOAssignmentControllerAnteBathWashRoom
                                                         .indDigitalOutputAnteRoomNightLight);
                    break;

                case ComandoString.TURN_LIGHT_ANTEROOM_BACK_OFF:
                    _FeedbackArgs = _LightCommanderAnteRoom.TurnSingleDevice(TurnDevice.OFF,
                                                         IOAssignmentControllerAnteBathWashRoom
                                                         .indDigitalOutputAnteRoomBackSide);
                    break;


                case ComandoString.TURN_LIGHT_FLOOR_UP1_ON:
                    _FeedbackArgs = _LightCommanderAnteRoom.TurnSingleDevice(TurnDevice.ON,
                                                          IOAssignmentControllerAnteBathWashRoom
                                                         .indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle1);
                    break;

                case ComandoString.TURN_LIGHT_FLOOR_UP2_ON:
                    _FeedbackArgs = _LightCommanderAnteRoom.TurnSingleDevice(TurnDevice.ON,
                                                          IOAssignmentControllerAnteBathWashRoom
                                                          .indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle2);
                    break;

                case ComandoString.TURN_LIGHT_FLOOR_UP1_OFF:
                    _FeedbackArgs = _LightCommanderAnteRoom.TurnSingleDevice(TurnDevice.OFF,
                                                         IOAssignmentControllerAnteBathWashRoom
                                                         .indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle1);
                    break;

                case ComandoString.TURN_LIGHT_FLOOR_UP2_OFF:

                    _FeedbackArgs = _LightCommanderAnteRoom.TurnSingleDevice(TurnDevice.OFF,
                                                         IOAssignmentControllerAnteBathWashRoom
                                                         .indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle2);

                    break;

                case ComandoString.TURN_LIGHT_WASHROOM_ON:
                    _FeedbackArgs = _LightCommanderAnteRoom.TurnSingleDevice(TurnDevice.ON,
                                                         IOAssignmentControllerAnteBathWashRoom
                                                         .indDigitalOutputWashRoomMainLight);
                    break;

                case ComandoString.TURN_LIGHT_WASHROOM_OFF:
                    _FeedbackArgs = _LightCommanderAnteRoom.TurnSingleDevice(TurnDevice.OFF,
                                                         IOAssignmentControllerAnteBathWashRoom
                                                         .indDigitalOutputWashRoomMainLight);
                    break;

                case ComandoString.TURN_LIGHT_BATHROOM_ALL_ON:
                    _LightCommanderBathRoom.ScenarioTriggerPersitent(TurnDevice.ON, ScenarioConstantsBathRoom.ScenarioAllLights);
                    break;

                case ComandoString.TURN_LIGHT_BATHROOM_ALL_OFF:
                    _LightCommanderBathRoom.ScenarioTriggerPersitent(TurnDevice.OFF, ScenarioConstantsBathRoom.ScenarioAllLights);
                    break;

                case ComandoString.TURN_HEATER_BATH_ON:
                    _HeaterCommanderBathRoom.Reset();
                    _HeaterCommanderBathRoom.MainTrigger(TurnDevice.ON);
                    break;

                case ComandoString.TURN_HEATER_BATH_OFF:
                    _HeaterCommanderBathRoom.Reset();
                    _HeaterCommanderBathRoom.MainTrigger(TurnDevice.OFF);
                    break;

                default:
                    break;


            }

            return _FeedbackArgs;
        }
        #endregion

        #region EVENTHANDLERS
        private void Update(object sender, UpdateEventArgs e)
        {
            if (MultiIOCardsAvailable)
            {
                if (e.IndexSelectedHardware < _IOHandlerMulti?.Length)
                {
                    _IOHandlerMulti?[e.IndexSelectedHardware]?.UpdateDigitalOutputs(e.Index, e.Value);
                }
            }
            else
            {
                _IOHandler?.UpdateDigitalOutputs(e.Index, e.Value);
            }
        }

        void Controller(int index, bool value)
        {
            switch (index)
            {
                case IOAssignmentControllerAnteBathWashRoom.indDigitalInputAnteRoomMainButton:
                    _ScenarioNumberAnteRoom = (int)_LightCommanderAnteRoom?.ScenarioTrigger(value);
                    break;

                case IOAssignmentControllerAnteBathWashRoom.indDigitalInputBathRoomMainButton:
                    _ScenarioNumberBathRoom = (int)_LightCommanderBathRoom?.ScenarioTrigger(value);
                    _HeaterCommanderBathRoom?.MainTrigger(value);
                    break;

                case IOAssignmentControllerAnteBathWashRoom.indDigitalInputWashRoomMainButton:
                    _ScenarioNumberWashRoom = (int)_LightCommanderWashRoom?.ScenarioTrigger(value);
                    break;

                case IOAssignmentControllerAnteBathWashRoom.indDigitalInputAnteRoomPresenceDetector:
                    _LightCommanderAnteRoom?.PresenceTrigger(value);
                    _PresenceLight?.PresenceTrigger(value);
                    break;

                case IOAssignmentControllerAnteBathWashRoom.indDigitalInputWindow:
                    bool WindowIsOpen = value == false;
                    if (WindowIsOpen)
                    {
                        _HeaterCommanderBathRoom.EventSwitch(PowerState.OFF);
                    }
                    break;
            }
        }

        private void DigitalInputChanged(object sender, DigitalInputEventargs e)
        {
            try
            {   
                Controller(e.Index, e.Value);
                string DeviceName = IOAssignmentControllerAnteBathWashRoom.GetInputDeviceName(e.Index);
                Console.WriteLine(TimeUtil.GetTimestamp_() +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       InfoString.DeviceDigitalInput +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       InfoString.BraceOpen +
                       e.Index.ToString() +
                       InfoString.BraceClose +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       DeviceName +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       InfoString.Is +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       e.Value.ToString());
                 string SendData = TimeUtil.GetTimestamp_() + "_" + DeviceName + "_" + e.Value.ToString();
                _Communicator?.SendString(SendData);          
            }
            catch (Exception LogException)
            {
                Console.WriteLine(TimeUtil.GetTimestamp_() + LogException.ToString());
            }
        }

        private void DigitalOutputChanged(object sender, DigitalOutputEventargs e)
        {
            try
            {
                if (e.Index == IOAssignmentControllerAnteBathWashRoom.indDigitalOutputReserverdForHeartBeat)
                {
                    return;
                }
                string DeviceName = IOAssignmentControllerAnteBathWashRoom.GetOutputDeviceName(e.Index);

                Console.WriteLine(TimeUtil.GetTimestamp_() +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       InfoString.DeviceDigialOutput +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       InfoString.BraceOpen +
                       e.Index.ToString() +
                       InfoString.BraceClose +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       DeviceName +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       InfoString.Is +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       e.Value.ToString());

                string SendData = TimeUtil.GetTimestamp_() + "_" + DeviceName + "_" + e.Value.ToString();
                _Communicator?.SendString(SendData);

            }
            catch (Exception LogException)
            {
                Console.WriteLine(TimeUtil.GetTimestamp_() + LogException.ToString());
            }
        }

        private void DataReceived(object sender, DataReceivingEventArgs e)
        {
            _RemoteControl(e);
            Console.WriteLine(TimeUtil.GetTimestamp_() + " Received telegramm: " + e.Message + " from " + e.Adress + ":" + e.Port);
        }
        #endregion

        #region PUBLIC
        public UpdateEventArgs RemoteControl(DataReceivingEventArgs e)
        {
            return (_FeedbackArgs = _RemoteControl(e));
        }
        #endregion
    }
}
