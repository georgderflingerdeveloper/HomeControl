using System;
using System.Timers;
using HomeControl.ADVANCED_COMPONENTS.Interfaces;
using HomeControl.BASIC_CONSTANTS;

namespace HomeControl.ADVANCED_COMPONENTS
{
    public delegate void Update( object sender, UpdateEventArgs e );

    public class UpdateEventArgs : EventArgs
    {
        int _index;
        bool value;

        int _indexselectedhardware;

        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                _index = value;
            }
        }

        public bool Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public int IndexSelectedHardware
        {
            get
            {
                return _indexselectedhardware;
            }

            set
            {
                _indexselectedhardware = value;
            }
        }
    }

    enum DeviceState
    {
        Inactive,
        Activated
    }

    class DeviceCommander : IDeviceCommander
    {
        public event Update               EUpdate;
        protected bool[] _ToggleOutput  = new bool[CommanderConfiguration.NumberOfOutputs];
        protected bool                   _inhibitpresencedetection;
        protected DeviceCommandos        _commando;
        protected UpdateEventArgs        _UpdateEventArgs = new UpdateEventArgs();
        protected CommanderConfiguration _config;
        protected IDeviceControlTimer    _devicecontroltimer;

        #region CONSTRUCTOR
        public DeviceCommander( CommanderConfiguration config, IDeviceControlTimer devicecontroltimer )
        {
            _config             = config;
            _commando           = _config.Modes;
            _UpdateEventArgs.IndexSelectedHardware = _config.IndexSelectedHardware;
            _devicecontroltimer = devicecontroltimer;
            devicecontroltimer.EControlOff += Devicecontroltimer_EControlOff;
            devicecontroltimer.EControlOn  += Devicecontroltimer_EControlOn;
            State = DeviceState.Inactive;
        }
        #endregion

        #region INTERFACE_IMPLEMENTATION
        public virtual void MainTrigger( bool edge )
        {
            switch( _commando )
            {
               case DeviceCommandos.SingleDeviceOnOffFallingEdge:
                    if( edge == Edge.Falling )
                    {
                        if( _config.Modesdelayedon == DeviceCommanderDelayedOn.None )
                        {
                            if( ToggleSingleDevice( _config.Startindex ) == true )
                            {
                                _inhibitpresencedetection = false;
                            }
                        }
                        else
                        {
                           if( _ToggleOutput[_config.Startindex] == false )
                           {
                               TurnSingleDevice( _config.Startindex, PowerState.OFF );
                           }
                        }
                    }
                    break;
            }

            switch( _config.ModesAutomaticoff )
            {
                case DeviceCommanderAutomaticOff.None:
                     break;

                case DeviceCommanderAutomaticOff.WithMainTrigger:
                case DeviceCommanderAutomaticOff.WithMainAndPresenceTrigger:
                     _devicecontroltimer.RestartOff( );
                     break;
            }

            switch( _config.Modesdelayedon )
            {
                case DeviceCommanderDelayedOn.None:
                     break;

                case DeviceCommanderDelayedOn.WithMainTrigger:
                case DeviceCommanderDelayedOn.WithMainAndPresenceTrigger:
                     if( edge == Edge.Falling )
                     {
                         _devicecontroltimer.StopOn( );
                     }
                     else
                     {
                         _devicecontroltimer.StartOn( );
                     }
                     break;
            }
        }

        public virtual void PresenceTrigger( bool edge )
        {
            if( _inhibitpresencedetection )
            {
                return;
            }

            switch( _commando )
            {
               case DeviceCommandos.None:
                    break;

               case DeviceCommandos.OnWithDelayedOffByRisingEdge:
                    if( edge == Edge.Rising )
                    {
                        TurnSingleDevice( _config.Startindex, PowerState.ON );
                        _devicecontroltimer.RestartOff( );
                    }
                    break;

                default:
                    break;
            }

            switch( _config.ModesAutomaticoff )
            {
               case DeviceCommanderAutomaticOff.None:
                    break;

               case DeviceCommanderAutomaticOff.WithPresenceTrigger:
               case DeviceCommanderAutomaticOff.WithMainAndPresenceTrigger:
                    if( edge == Edge.Falling )
                    {
                        switch( _commando )
                        {
                            case DeviceCommandos.SingleDeviceOnOffFallingEdge:
                                 _devicecontroltimer.RestartOff( );
                                 break;
                        }
                    }
                    break;
            }
        }

        public void Reset( )
        {
            _inhibitpresencedetection = false;
        }
        #endregion

        #region PROTECTED_METHODS
        protected void TurnSingleDevice( int index, bool value )
        {
            if( ( index < CommanderConfiguration.NumberOfOutputs ) && ( index >= 0 ) )
            {
                _UpdateEventArgs.Index = index;
                _UpdateEventArgs.Value = value;
                EUpdate?.Invoke( this, _UpdateEventArgs );
                State = value ? DeviceState.Activated : DeviceState.Inactive;
            }
        }

        protected void Update( int index, bool value )
        {
            if( ( index < CommanderConfiguration.NumberOfOutputs ) && ( index >= 0 ) )
            {
                _UpdateEventArgs.Index = index;
                _UpdateEventArgs.Value = value;
                EUpdate?.Invoke( this, _UpdateEventArgs );
            }
        }

        protected bool ToggleSingleDevice( int index )
        {
            bool state = false;
            if( ( index < CommanderConfiguration.NumberOfOutputs ) && ( index >= 0 ) )
            {
                if( !_ToggleOutput[index] )
                {
                    TurnSingleDevice( index, PowerState.ON );
                    state = true;
                }
                else
                {
                    TurnSingleDevice( index, PowerState.OFF );
                }
                _ToggleOutput[index] = !_ToggleOutput[index];
            }
            return ( state );
        }

        protected void PresetToggling( int index, bool value )
        {
            _ToggleOutput[index] = value;
        }
        #endregion

        #region PROPERTIES
        public DeviceCommandos Commando
        {
            get
            {
                return _commando;
            }

            set
            {
                _commando = value;
            }
        }

        public bool InhibitPresenceDetection
        {
            get
            {
                return _inhibitpresencedetection;
            }

            set
            {
                _inhibitpresencedetection = value;
            }
        }

        public DeviceState State { get; set; }
        #endregion

        #region EVENTHANDLERS
        private void Devicecontroltimer_EControlOff( object sender, ElapsedEventArgs e )
        {
            if( _config.ModesAutomaticoff == DeviceCommanderAutomaticOff.None )
            {
                return;
            }

            switch( _commando )
            {
               case DeviceCommandos.SingleDeviceToggleRisingEdge:
               case DeviceCommandos.SingleDeviceOnOffFallingEdge:
                    TurnSingleDevice( _config.Startindex, PowerState.OFF );
                    PresetToggling( _config.Startindex, PowerState.OFF );
                    _inhibitpresencedetection = true;
                    break;

            }
        }

        private void Devicecontroltimer_EControlOn( object sender, ElapsedEventArgs e )
        {
            if( _config.Modesdelayedon == DeviceCommanderDelayedOn.None )
            {
                return;
            }

            switch( _commando )
            {
                case DeviceCommandos.SingleDeviceOnOffFallingEdge:
                     TurnSingleDevice( _config.Startindex, PowerState.ON );
                     _ToggleOutput[_config.Startindex] = !_ToggleOutput[_config.Startindex];
                     break;

                case DeviceCommandos.SingleDeviceToggleRisingEdge:
                     ToggleSingleDevice( _config.Startindex );
                     break;
            }
            _devicecontroltimer.StopOn( );
        }
        #endregion
    }
}
