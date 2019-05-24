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
            switch (e.Message)
            {
                case ComandoString.TURN_ALL_LIGHTS_KIDROOM_ON:
                    LightCommander?.Reset();
                    LightCommander?.ScenarioTriggerPersitent(TurnDevice.ON, ScenarioConstantsSleepingRoom.ScenarionAllLights);
                    break;

                case ComandoString.TURN_ALL_LIGHTS_KIDROOM_OFF:
                    LightCommander?.Reset();
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

           Console.WriteLine(
                             TimeUtil.GetTimestamp_() + 
                             " Received data from: "  +
                             e.Adress.ToString()      + 
                             " : "                    + 
                             e.Port.ToString());
        }

        private void DigitalInputChanged(object sender, DigitalInputEventargs e)
        {
            RoomController(e.Index, e.Value);

            try
            {
                Console.WriteLine(TimeUtil.GetTimestamp_() +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       InfoString.DeviceDigitalInput +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       InfoString.BraceOpen +
                       e.Index.ToString() +
                       InfoString.BraceClose +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       IOAssignmentControllerSleepingRoom.GetInputDeviceName(e.Index) +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       InfoString.Is +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       e.Value.ToString());
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
                //if (e.Index == IOAssignmentControllerAnteBathWashRoom.indDigitalOutputReserverdForHeartBeat)
                //{
                //    return;
                //}
                Console.WriteLine(TimeUtil.GetTimestamp_() +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       InfoString.DeviceDigialOutput +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       InfoString.BraceOpen +
                       e.Index.ToString() +
                       InfoString.BraceClose +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       IOAssignmentControllerSleepingRoom.GetOutputDeviceName(e.Index) +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       InfoString.Is +
                       HardConfig.COMMON.Seperators.WhiteSpace +
                       e.Value.ToString());
            }
            catch (Exception LogException)
            {
                Console.WriteLine(TimeUtil.GetTimestamp_() + LogException.ToString());
            }
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
