using HomeControl.ADVANCED_COMPONENTS.Interfaces;
using System.Timers;
using System.Collections.Generic;

namespace HomeControl.ADVANCED_COMPONENTS
{
    public delegate void ExtUpdate( object sender, UpdateEventArgs e );

    class ExtendedLightCommander : LightCommander, IExtendedLightCommander
    {
        #region DECLARATIONS
        IDeviceScenarioControl  _devicescenariocontrol;
        UpdateEventArgs         UpdateArgs = new UpdateEventArgs();
        CommanderConfiguration  _configuration;
        public event ExtUpdate  ExtUpdate;
        #endregion

        #region CONSTRUCTOR
        public ExtendedLightCommander( CommanderConfiguration config, IDeviceControlTimer devicecontroltimer, IDeviceScenarioControl devicescenariocontrol ) : base( config, devicecontroltimer )
        {
            _configuration                               = config;
            _devicescenariocontrol                       = devicescenariocontrol;
            _devicescenariocontrol.EScenario            += _devicescenariocontrol_EScenario;
            _devicescenariocontrol.EScenarioControlInfo += _devicescenariocontrol_EScenarioControlInfo;
            EUpdate                                     += ExtendedLightCommander_EUpdate;
            _devicecontroltimer.EControlFinalOff        += _devicecontroltimer_EControlFinalOff;

            _devicecontroltimer.EControlOff += _devicecontroltimer_EControlOff;
        }
        #endregion

        #region PUBLIC_METHODS
        public int ScenarioTrigger( bool edge )
        {
            _devicescenariocontrol.WatchForInputValueChange( edge );
            ControlAutomaticOff( );
            int ActualScenarioNumber = _devicescenariocontrol.GetActualScenarioNumber();
            return ( ActualScenarioNumber );
        }
        public int ScenarioTriggerPersitent( bool edge, int number )
        {
            _devicescenariocontrol.Turn( edge, number );
            int ActualScenarioNumber = _devicescenariocontrol.GetActualScenarioNumber( );
            return ( ActualScenarioNumber );
        }
        #endregion

        #region PRIVATE_METHODS
        #endregion

        #region EVENTHANDLERS
        // any update when f.e. automatic off is desired
        private void ExtendedLightCommander_EUpdate( object sender, UpdateEventArgs e )
        {
            ExtUpdate?.Invoke( this, e );
        }

        private void _devicescenariocontrol_EScenario( object sender, ScenarioEventArgs e )
        {
            UpdateArgs.Index = e.Index;
            UpdateArgs.Value = e.Value;
            ExtUpdate?.Invoke( this, UpdateArgs );
        }

        private void _devicescenariocontrol_EScenarioControlInfo( object sender, ScenarioControlEventArgs e )
        {
            switch( e.Control.State )
            {
                case ScenarioState.Next:
                case ScenarioState.Toggle:
                case ScenarioState.PostIdle:
                case ScenarioState.Idle:
                     if( e.Value )
                     {
                         if( _inhibitpresencedetection )
                         {
                             _inhibitpresencedetection = false; 
                         }
                     }
                     break;
            }
        }

        private void _devicecontroltimer_EControlOff( object sender, ElapsedEventArgs e )
        {
            // there is no reset demand for remaining on devices
            if( _configuration.DeviceRemainOnAfterAutomaticOff.Count == 0 )
            {
                _devicescenariocontrol.Reset( );
            }
            else
            {
                int actualSelectedScenario = _devicescenariocontrol.GetActualScenarioNumber();
                if( DoesAcutalScenarioContainDevicesToKeepTurnedOn( actualSelectedScenario ) )
                {

                }else
                {
                    _devicescenariocontrol.PresetDeviceOff( );
                }
            }
        }

        bool DoesAcutalScenarioContainDevicesToKeepTurnedOn( int actualscenario )
        {
           bool Contain = false;
           if( actualscenario >= 0 && _AvailableScenarios.Count > 0)
            {
                int index = 0;
                foreach( var element in _config.DeviceRemainOnAfterAutomaticOff )
                {
                    if( _AvailableScenarios[actualscenario].Contains( _config.DeviceRemainOnAfterAutomaticOff[index] ) )
                    {
                        index++;
                        Contain = true;
                        continue;
                    }
                    else
                    {
                        Contain = false;
                    }
                }
            }
            return Contain;
        }

        private void _devicecontroltimer_EControlFinalOff( object sender, ElapsedEventArgs e )
        {
            _devicescenariocontrol.Reset( );
        }
        #endregion

        #region PROPERTIES
        List<List<int>>    _AvailableScenarios = new List<List<int>>();  // contains indices for different scenarios

        public List<List<int>> AvailableScenarios
        {
            set
            {
                _AvailableScenarios = value;
            }
        }

        #endregion
    }
}
