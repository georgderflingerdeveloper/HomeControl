
using System.Timers;
using HomeControl.BASIC_CONSTANTS;
using HomeControl.ADVANCED_COMPONENTS.Interfaces;


namespace HomeControl.ADVANCED_COMPONENTS
{
    class LightCommander : DeviceCommander, ILightCommander
    {
        #region CONSTRUCTOR
        public LightCommander( CommanderConfiguration config, IDeviceControlTimer devicecontroltimer ) : base( config, devicecontroltimer )
        {
            devicecontroltimer.EControlOff      += Devicecontroltimer_EControlOff;
            devicecontroltimer.EControlFinalOff += Devicecontroltimer_EControlFinalOff;
        }
        #endregion

        #region INTERFACE_IMPLEMENTATION
        public override void MainTrigger( bool edge )
        {
            switch( _commando )
            {
               case DeviceCommandos.SingleLight:
                    if( edge == Edge.Falling )
                    {
                        if( ToggleSingleDevice( _config.Startindex ) == true )
                        {
                            _inhibitpresencedetection = false;
                        }
                    }
                    break;
            }
            ControlAutomaticOff();
        }

        public override void PresenceTrigger( bool edge )
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

                case DeviceCommandos.OnWithDelayedOffByFallingEdge:
                     if( edge == Edge.Falling )
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
                                case DeviceCommandos.ScenarioLight:
                                case DeviceCommandos.SingleLight:
                                     _devicecontroltimer.RestartOff( );
                                     break;
                             }
                         }
                         break;
            }
        }
        #endregion

        #region PROTECTED_METHODS
        protected void ControlAutomaticOff()
        {
            switch( _config.ModesAutomaticoff )
            {
                    case DeviceCommanderAutomaticOff.WithMainTrigger:
                    case DeviceCommanderAutomaticOff.WithMainAndPresenceTrigger:
                    case DeviceCommanderAutomaticOff.WithPresenceTrigger:
                         _devicecontroltimer.RestartOff( );
                         break;
            }
        }
        #endregion

        #region EVENTHANDLERS
        private void Devicecontroltimer_EControlOff( object sender, ElapsedEventArgs e )
        {
            switch( _commando )
            {
                case DeviceCommandos.SingleLight:
                     TurnSingleDevice( _config.Startindex, PowerState.OFF );
                     PresetToggling( _config.Startindex, PowerState.OFF );
                     _inhibitpresencedetection = true;
                     break;

                case DeviceCommandos.OnWithDelayedOffByFallingEdge:
                case DeviceCommandos.OnWithDelayedOffByRisingEdge:
                     TurnSingleDevice( _config.Startindex, PowerState.OFF );
                     PresetToggling( _config.Startindex, PowerState.OFF );
                     break;

                case DeviceCommandos.ScenarioLight:
                     for( int i = _config.Startindex; i <= _config.Lastindex; i++ )
                     {
                        if( _config.DeviceRemainOnAfterAutomaticOff.Contains( i ) == false )
                        {
                            TurnSingleDevice( i, PowerState.OFF );
                            _inhibitpresencedetection = true; 
                        }
                     }
                     break;
            }
        }

        private void Devicecontroltimer_EControlFinalOff( object sender, ElapsedEventArgs e )
        {
            switch( _commando )
            {
               case DeviceCommandos.ScenarioLight:
                    for( int i = _config.Startindex; i <= _config.Lastindex; i++ )
                    {
                         TurnSingleDevice( i, PowerState.OFF );
                         _inhibitpresencedetection = true; // TODO-CHECK!
                    }
                    break;
            }
        }
        #endregion
    }
 
}
