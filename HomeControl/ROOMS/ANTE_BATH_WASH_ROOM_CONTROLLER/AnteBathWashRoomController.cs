﻿using BASIC_COMPONENTS;
using HomeControl.ADVANCED_COMPONENTS.Interfaces;
using HomeControl.BASIC_COMPONENTS.Interfaces;
using HomeControl.ADVANCED_COMPONENTS;
using BASIC_CONTROL_LOGIC;
using HomeControl.BASIC_CONSTANTS;
using HomeControl.ROOMS.ANTE_BATH_WASH_ROOM_CONTROLLER.INTERFACE;

namespace HomeControl.ROOMS
{
    class AnteBathWashRoomController : Controller, IAnteBathWashRoomController
    {
        #region DECLARATION
        AnteBathWashRoomConfiguration  _config;
        IIOHandler                     _IOHandler;
        IIOHandler[]                   _IOHandlerMulti;
        ExtendedLightCommander         _LightCommanderAnteRoom;
        ExtendedLightCommander         _LightCommanderBathRoom;
        HeaterCommander                _HeaterCommanderBathRoom;
        IDeviceControlTimer            _DeviceControlTimerHeaterBathRoom;
        ExtendedLightCommander         _LightCommanderWashRoom; 
        DeviceScenarioControl          _DeviceScenarioControlAnteRoom;
        IDeviceControlTimer            _DeviceControlTimerAnteRoom;
        DeviceScenarioControl          _DeviceScenarioControlBathRoom;
        IDeviceControlTimer            _DeviceControlTimerBathRoom;
        DeviceScenarioControl          _DeviceScenarioControlWashRoom;
        IDeviceControlTimer            _DeviceControlTimerWashRoom;
        IDeviceBlinker                 _HeartBeat;
        LightCommander                 _PresenceLight;
        IDeviceControlTimer            _DeviceControlTimerPresenceLight;

        double TimeTurnOn;
        double TimeTurnAutomaticOff;
        double TimeTurnFinalOff;
        int    Startindex;
        int    Lastindex;
        double TimeNextScenario;
        double IdleScenario;
        double NotUsed;
        bool   MultiIOCardsAvailable = false;
        int _ScenarioNumberAnteRoom;
        int _ScenarioNumberBathRoom;
        int _ScenarioNumberWashRoom;

        #endregion

        #region CONSTRUCTOR
        public AnteBathWashRoomController( AnteBathWashRoomConfiguration config, IDeviceBlinker HeartBeat, IIOHandler IOHandler ) : base()
        {
            _HeartBeat = HeartBeat;
            _config    = config;
            _IOHandler = IOHandler;
            _IOHandler.EDigitalInputChanged += IOHandler__EDigitalInputChanged;
            Constructor( );
            HeartBeat.EUpdate += _Commander_ExtUpdate;
            HeartBeat.Start( );
        }

        public AnteBathWashRoomController( AnteBathWashRoomConfiguration config, IDeviceBlinker HeartBeat, IIOHandler[] IOHandler ) : base( )
        {
            MultiIOCardsAvailable = true;
            _HeartBeat            = HeartBeat;
            _config               = config;
            _IOHandlerMulti       = IOHandler;

            for( int i = 0; i < IOHandler.Length; i++ )
            {
                 _IOHandlerMulti[i].EDigitalInputChanged += IOHandler__EDigitalInputChanged;
            }

            Constructor( );
            HeartBeat.EUpdate += _Commander_ExtUpdate;
            HeartBeat.Start( );
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

            _DeviceScenarioControlAnteRoom           = new DeviceScenarioControl( Startindex, Lastindex, new Timer_( TimeNextScenario ), new Timer_( NotUsed ), new Timer_( IdleScenario ) );
            _DeviceScenarioControlAnteRoom.Scenarios = _config.AnteRoom.ScenarioConfiguration.Scenarios;

            _DeviceControlTimerAnteRoom        = new DeviceControlTimer( new Timer_( TimeTurnOn ), new Timer_( TimeTurnAutomaticOff ), new Timer_( TimeTurnFinalOff ) );
            _LightCommanderAnteRoom            = new ExtendedLightCommander( _config.AnteRoom.LightCommanderConfiguration, _DeviceControlTimerAnteRoom, _DeviceScenarioControlAnteRoom );
            _LightCommanderAnteRoom.ExtUpdate += _Commander_ExtUpdate;
            _LightCommanderAnteRoom.AvailableScenarios = _config.AnteRoom.ScenarioConfiguration.Scenarios;

            #endregion

            #region LIGHTCOMMANDER_BATHROOM
            TimeTurnOn            = _config.BathRoom.LightCommanderConfiguration.DelayTimeAllOn;
            TimeTurnAutomaticOff  = _config.BathRoom.LightCommanderConfiguration.DelayTimeOffByMissingTriggerSignal;
            TimeTurnFinalOff      = _config.BathRoom.LightCommanderConfiguration.DelayTimeFinalOff;
            Startindex            = _config.BathRoom.LightCommanderConfiguration.Startindex;
            Lastindex             = _config.BathRoom.LightCommanderConfiguration.Lastindex;
            TimeNextScenario      = _config.BathRoom.ScenarioConfiguration.DelayTimeNextScenario;
            IdleScenario          = _config.BathRoom.LightCommanderConfiguration.DelayTimeDoingNothing;
            NotUsed = 1; // TODO

            _DeviceScenarioControlBathRoom           = new DeviceScenarioControl( Startindex, Lastindex, new Timer_( TimeNextScenario ), new Timer_( NotUsed ), new Timer_( IdleScenario ) );
            _DeviceScenarioControlBathRoom.Scenarios = _config.BathRoom.ScenarioConfiguration.Scenarios;

            _DeviceControlTimerBathRoom        = new DeviceControlTimer( new Timer_( TimeTurnOn ), new Timer_( TimeTurnAutomaticOff ), new Timer_( TimeTurnFinalOff ) );
            _LightCommanderBathRoom            = new ExtendedLightCommander( _config.BathRoom.LightCommanderConfiguration, _DeviceControlTimerBathRoom, _DeviceScenarioControlBathRoom );
            _LightCommanderBathRoom.ExtUpdate += _Commander_ExtUpdate;
            #endregion

            #region LIGHTCOMMANDER_WASHROOM
            TimeTurnOn              = _config.WashRoom.LightCommanderConfiguration.DelayTimeAllOn;
            TimeTurnAutomaticOff    = _config.WashRoom.LightCommanderConfiguration.DelayTimeOffByMissingTriggerSignal;
            TimeTurnFinalOff        = _config.WashRoom.LightCommanderConfiguration.DelayTimeFinalOff;
            Startindex              = _config.WashRoom.LightCommanderConfiguration.Startindex;
            Lastindex               = _config.WashRoom.LightCommanderConfiguration.Lastindex;
            TimeNextScenario        = _config.WashRoom.ScenarioConfiguration.DelayTimeNextScenario;
            IdleScenario            = _config.WashRoom.LightCommanderConfiguration.DelayTimeDoingNothing;
            NotUsed = 1; // TODO

            _DeviceScenarioControlWashRoom           = new DeviceScenarioControl( Startindex, Lastindex, new Timer_( TimeNextScenario ), new Timer_( NotUsed ), new Timer_( IdleScenario ) );
            _DeviceScenarioControlWashRoom.Scenarios = _config.WashRoom.ScenarioConfiguration.Scenarios;

            _DeviceControlTimerWashRoom        = new DeviceControlTimer( new Timer_( TimeTurnOn ), new Timer_( TimeTurnAutomaticOff ), new Timer_( TimeTurnFinalOff ) );
            _LightCommanderWashRoom            = new ExtendedLightCommander( _config.WashRoom.LightCommanderConfiguration, _DeviceControlTimerWashRoom, _DeviceScenarioControlWashRoom );
            _LightCommanderWashRoom.ExtUpdate += _Commander_ExtUpdate;
            #endregion

            #region LIGHTCOMMANDER_PRESENCE
            TimeTurnAutomaticOff             = _config.PresenceLightConfiguration.DelayTimeOffByMissingTriggerSignal;
            _DeviceControlTimerPresenceLight = new DeviceControlTimer( new Timer_( TimeTurnAutomaticOff ) );
            _PresenceLight                   = new LightCommander( _config.PresenceLightConfiguration, _DeviceControlTimerPresenceLight );
            _PresenceLight.EUpdate += _Commander_ExtUpdate;
            #endregion

            #region HEATERCOMMANDER_BATHROOM
            TimeTurnOn                        = _config.HeaterBathRoom.DelayTimeAllOn;
            TimeTurnAutomaticOff              = _config.HeaterBathRoom.DelayTimeFinalOff;
            _DeviceControlTimerHeaterBathRoom = new DeviceControlTimer( new Timer_( TimeTurnOn ), new Timer_( TimeTurnAutomaticOff ) );
            _HeaterCommanderBathRoom          = new HeaterCommander( _config.HeaterBathRoom, _DeviceControlTimerHeaterBathRoom );
            _HeaterCommanderBathRoom.EUpdate += _Commander_ExtUpdate;
            #endregion
        }
        #endregion

        #region EVENTHANDLERS
        private void _Commander_ExtUpdate( object sender, UpdateEventArgs e )
        {
            if( MultiIOCardsAvailable )
            {
                if( e.IndexSelectedHardware < _IOHandlerMulti?.Length )
                {
                    _IOHandlerMulti?[e.IndexSelectedHardware]?.UpdateDigitalOutputs( e.Index, e.Value );
                }
            }
            else
            {
                _IOHandler?.UpdateDigitalOutputs( e.Index, e.Value );
            }
        }

        void RoomController( int index, bool value )
        {
            switch( index )
            {
                case IOAssignmentControllerAnteBathWashRoom.indDigitalInputAnteRoomMainButton:
                     _ScenarioNumberAnteRoom = (int) _LightCommanderAnteRoom?.ScenarioTrigger( value );
                     break;

                case IOAssignmentControllerAnteBathWashRoom.indDigitalInputBathRoomMainButton:
                     _ScenarioNumberBathRoom = (int) _LightCommanderBathRoom?.ScenarioTrigger( value );
                     _HeaterCommanderBathRoom?.MainTrigger( value );
                     break;

                case IOAssignmentControllerAnteBathWashRoom.indDigitalInputWashRoomMainButton:
                     _ScenarioNumberWashRoom = (int) _LightCommanderWashRoom?.ScenarioTrigger( value );
                     break;

                case IOAssignmentControllerAnteBathWashRoom.indDigitalInputAnteRoomPresenceDetector:
                     _LightCommanderAnteRoom?.PresenceTrigger( value );
                     _PresenceLight?.PresenceTrigger( value );
                     break;

                case IOAssignmentControllerAnteBathWashRoom.indDigitalInputWindow:
                     bool WindowIsOpen = (value == true);
                     if( WindowIsOpen )
                     {
                        _HeaterCommanderBathRoom.EventSwitch( PowerState.OFF );
                     }
                     else
                     {
                        _HeaterCommanderBathRoom.EventSwitch( PowerState.ON );
                     }
                     break;
            }
        }

        private void IOHandler__EDigitalInputChanged( object sender, DigitalInputEventargs e )
        {
            RoomController( e.Index, e.Value );
        }
        #endregion
    }
}