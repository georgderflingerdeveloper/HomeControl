using System;
using System.Timers;
using HomeControl.ADVANCED_COMPONENTS.Interfaces;
using BASIC_COMPONENTS;


namespace HomeControl.ADVANCED_COMPONENTS
{
    public enum PwmStatus
    {
        eInactive,
        eActive
    };

    public enum SwitchStatus
    {
        eOff,
        eOn
    };

    public class PwmControllerEventArgs : EventArgs
    {
        PwmStatus      _Status;
        SwitchStatus   _SwitchStatus;
        double         _DurationOn;
        double         _DurationOff;

        public PwmStatus Status
        {
            get
            {
                return _Status;
            }

            set
            {
                _Status = value;
            }
        }

        public SwitchStatus SwitchStatus
        {
            get
            {
                return _SwitchStatus;
            }

            set
            {
                _SwitchStatus = value;
            }
        }

        public double DurationOn
        {
            get
            {
                return _DurationOn;
            }

            set
            {
                _DurationOn = value;
            }
        }

        public double DurationOff
        {
            get
            {
                return _DurationOff;
            }

            set
            {
                _DurationOff = value;
            }
        }
    }

    public delegate void AnyStatusChanged( object sender, PwmControllerEventArgs e );

    class PwmController : IPwmController
    {
        #region DECLARATION
        PwmStatus               _Status;
        double                  _DurationOn;
        double                  _DurationOff;
        ITimer                  _TimerOn;
        ITimer                  _TimerOff;
        PwmControllerEventArgs  _PwmControllerEventArgs;
        bool                    _TimerOnIsStarted;
        bool                    _TimerOffIsStarted;
        #endregion

        #region CONSTRUCTOR
        public PwmController( ITimer TimerOn, ITimer TimerOff, double DurationOn, double DurationOff )
        {
            _PwmControllerEventArgs             = new PwmControllerEventArgs( );
            _DurationOn                         = DurationOn;
            _DurationOff                        = DurationOff;
            _PwmControllerEventArgs.DurationOn  = _DurationOn;
            _PwmControllerEventArgs.DurationOff = _DurationOff;
            _TimerOn = TimerOn;
            _TimerOn.Elapsed += _TimerOn_Elapsed;
        }


        #endregion

        #region PROPERTIES
        public PwmStatus Status
        {
            get
            {
                return _Status;
            }

            set
            {
                _Status = value;
            }
        }

        public bool TimerOnIsStarted
        {
            get
            {
                return _TimerOnIsStarted;
            }
       }

        public bool TimerOffIsStarted
        {
            get
            {
                return _TimerOffIsStarted;
            }
        }
        #endregion

        #region PRIVATE
        #endregion

        #region PUBLIC
        public event AnyStatusChanged EAnyStatusChanged;
        public void Start( )
        {
            _PwmControllerEventArgs.Status       = PwmStatus.eActive;
            _PwmControllerEventArgs.SwitchStatus = SwitchStatus.eOn;
            EAnyStatusChanged?.Invoke( this, _PwmControllerEventArgs );
            _TimerOn.Start( );
            _TimerOnIsStarted = _TimerOn.IsStarted( );
        }

        public void Stop( )
        {

        }
        #endregion

        #region EVENTHANDLERS
        private void _TimerOn_Elapsed( object sender, ElapsedEventArgs e )
        {
            _PwmControllerEventArgs.SwitchStatus = SwitchStatus.eOff;
            EAnyStatusChanged?.Invoke( this, _PwmControllerEventArgs );
            _TimerOn.Stop( );
            _TimerOnIsStarted = _TimerOn.IsStarted( );
        }
        #endregion
    }
}
