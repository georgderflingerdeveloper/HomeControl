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
using HomeControl.ROOMS.SLEEPING_ROOM.INTERFACE;

namespace HomeControl.ROOMS.SLEEPING_ROOM
{
    class SleepingRoomController : Controller, ISleepingRoomController
    {
        #region DECLARATION
        UpdateEventArgs           _FeedbackArgs = new UpdateEventArgs();
        DataReceivingEventArgs    _FeedbackReceivedArgs = new DataReceivingEventArgs();

        IIOHandler                IOHandler;
        IUdpBasic                 Communicator;
        IExtendedLightCommander   LightCommander;
        IHeaterCommander          Heater;

        int    _ScenarioNumber;

        #endregion
        public SleepingRoomController( SleepingRoomConfiguration config, 
                                       IIOHandler                iOHandler, 
                                       IUdpBasic                 communicator,
                                       IExtendedLightCommander   lightCommander, 
                                       IHeaterCommander          heaterCommander ) : base()
        {
            IOHandler      = iOHandler;
            LightCommander = lightCommander;
            Heater         = heaterCommander;
            Communicator   = communicator;
            IOHandler.EDigitalInputChanged  += DigitalInputChanged;
            IOHandler.EDigitalOutputChanged += DigitalOutputChanged;
            Communicator.EDataReceived      += DataReceived;
            Heater.EUpdate                  += ExtUpdate;
            LightCommander.ExtUpdate        += ExtUpdate;
        }

        

        #region PRIVATE
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

        private DataReceivingEventArgs _RemoteControl(DataReceivingEventArgs e)
        {
            LightCommander?.Reset();
            switch (e.Message)
            {
                case ComandoString.TURN_ALL_LIGHTS_KIDROOM_ON:
                    LightCommander?.ScenarioTriggerPersitent(TurnDevice.ON, ScenarioConstantsSleepingRoom.ScenarionAllLights);
                    break;

                case ComandoString.TURN_ALL_LIGHTS_KIDROOM_OFF:
                    LightCommander?.ScenarioTriggerPersitent(TurnDevice.OFF, ScenarioConstantsSleepingRoom.ScenarionAllLights);
                    break;

                case ComandoString.TURN_LIGHT_KIDROOM1_ON:
                    LightCommander?.TurnSingleDevice(TurnDevice.ON, IOAssignmentControllerSleepingRoom.indDigitalOutputLightCeiling);
                    break;

                case ComandoString.TURN_LIGHT_KIDROOM1_OFF:
                    LightCommander?.TurnSingleDevice(TurnDevice.OFF, IOAssignmentControllerSleepingRoom.indDigitalOutputLightCeiling);
                    break;

                default:
                    return (e);
            }
            return (e);
        }
        #endregion



        #region EVENTHANDLERS
        private void ExtUpdate(object sender, UpdateEventArgs e)
        {
            IOHandler?.UpdateDigitalOutputs(e.Index, e.Value);
        }

        private void DataReceived(object sender, DataReceivingEventArgs e)
        {
            _FeedbackReceivedArgs = _RemoteControl(e);
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
        public DataReceivingEventArgs FeedbackReceivedArgs { get => _FeedbackReceivedArgs; set => _FeedbackReceivedArgs = value; }
        #endregion

        #region PUBLIC
        public DataReceivingEventArgs RemoteControl(DataReceivingEventArgs e)
        {
            _RemoteControl(e);
            return ( e );
        }
        #endregion
    }
}
