using BASIC_COMPONENTS;
using HomeControl.BASIC_CONSTANTS;
using System.Timers;

namespace BASIC_CONTROL_LOGIC
{
    class devicestepcontrol 
    {
        #region DECLARATION
        uint    _numberofdevices;
        ITimer  _TimerNextDevice;
        uint    _actualindex;
        bool    _value;
        uint    _startindex;

        public delegate void Step( uint number, bool value );
        public event Step EStep;
        #endregion

        #region CONSTRUCTOR
        public devicestepcontrol( uint startindex, uint numberofdevices, ITimer TimerNextDevice_ )
        {
            _numberofdevices          = numberofdevices;
            _TimerNextDevice          = TimerNextDevice_;
            _startindex               = startindex;
            _actualindex              = _startindex;
            _TimerNextDevice.Elapsed += _TimerNextDevice_Elapsed;
        }
        #endregion

        #region PRIVATE_METHODS
        void Watcher( bool trigger )
        {
            switch( trigger )
            {
                case Trigger.RISING:
                     _TimerNextDevice.Start( );
                     break;

                case Trigger.FALLING:
                     _TimerNextDevice.Stop( );
                     _value = !_value;
                     EStep?.Invoke( _actualindex, _value );
                     break;

                default:
                     break;
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void WatchForInputValueChange( bool trigger )
        {
            Watcher( trigger );
        }

        public void Reset()
        {
            _actualindex = 0;
            _value = false;
            _TimerNextDevice.Stop( );
        }
        #endregion

        #region PROPERTIES
        public uint Number
        {
            get
            {
                return _actualindex;
            }

            set
            {
                _actualindex = value;
            }
        }

        public bool Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }
        #endregion

        #region EVENTHANDLERS
        private void _TimerNextDevice_Elapsed( object sender, ElapsedEventArgs e )
        {
            _value = false;
            EStep?.Invoke( _actualindex, _value );
            if( _actualindex < _numberofdevices  )
            {
                _actualindex++;
            }else
            {
                _actualindex = _startindex; 
            }
            EStep?.Invoke( _actualindex, _value );
            _TimerNextDevice.Stop( );
        }
        #endregion
    }
}
