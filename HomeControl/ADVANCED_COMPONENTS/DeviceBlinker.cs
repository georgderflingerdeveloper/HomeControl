using HomeControl.ADVANCED_COMPONENTS.Interfaces;
using HomeControl.BASIC_CONSTANTS;
using BASIC_COMPONENTS;
using System.Timers;

namespace HomeControl.ADVANCED_COMPONENTS
{
    public enum StartBlinker
    {
        eWithOffPeriode,
        eWithOnPeriode
    }

    public class BlinkerConfiguration
    {

        int _index;
        StartBlinker _StartMode;

        public BlinkerConfiguration( int index, StartBlinker mode)
        {
            _index     = index;
            _StartMode = mode;
        }

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

        public StartBlinker StartMode
        {
            get
            {
                return _StartMode;
            }

            set
            {
                _StartMode = value;
            }
        }
    }

    public class DeviceBlinker : IDeviceBlinker
    {
        public event Update          EUpdate;
        bool[] _ToggleOutput  = new bool[CommanderConfiguration.NumberOfOutputs];
        UpdateEventArgs        _UpdateEventArgs = new UpdateEventArgs();
        ITimer _devicecontroltimer;
        BlinkerConfiguration _config;

        #region CONSTRUCTOR
        public DeviceBlinker( BlinkerConfiguration config, ITimer devicecontroltimer )
        {
            _config = config;
            _devicecontroltimer = devicecontroltimer;
            _devicecontroltimer.Elapsed += _devicecontroltimer_Elapsed;
        }
        #endregion

        #region INTERFACE_IMPLEMENTATION
        public void Start( )
        {
            switch( _config.StartMode )
            {
                case StartBlinker.eWithOffPeriode:
                     PresetToggling( _config.Index, false );
                     TurnSingleDevice( _config.Index, false );
                     break;

                case StartBlinker.eWithOnPeriode:
                     PresetToggling( _config.Index, true );
                     TurnSingleDevice( _config.Index, true );
                     break;
            }
            _devicecontroltimer.Start( );
        }

        public void Stop( )
        {
            TurnSingleDevice( _config.Index, false );
            _devicecontroltimer.Stop( );
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
 
        #region EVENTHANDLERS
        private void _devicecontroltimer_Elapsed( object sender, ElapsedEventArgs e )
        {
            ToggleSingleDevice( _config.Index );
            _devicecontroltimer.Stop( );
            _devicecontroltimer.Start( );
        }
        #endregion
    }
}
