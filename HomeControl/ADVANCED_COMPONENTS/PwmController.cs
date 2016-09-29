using System;
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

    enum Mode
    {
        eStartWithOff,
        eStartWithOn
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
        Mode                    _Mode;
        double                  _DurationOn;
        double                  _DurationOff;
        ITimer                  _TimerOn;
        ITimer                  _TimerOff;
        PwmControllerEventArgs  _PwmControllerEventArgs;
        #endregion

        #region CONSTRUCTOR
        public PwmController( ITimer TimerOn, ITimer TimerOff, double DurationOn, double DurationOff, Mode Mode_ )
        {
            _PwmControllerEventArgs = new PwmControllerEventArgs( );
            _DurationOn  = DurationOn;
            _DurationOff = DurationOff;
            _PwmControllerEventArgs.DurationOn  = _DurationOn;
            _PwmControllerEventArgs.DurationOff = _DurationOff;
            _Mode = Mode_;
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
        #endregion

        #region PUBLIC
        public event AnyStatusChanged EAnyStatusChanged;
        public void Start( )
        {
            _PwmControllerEventArgs.Status = PwmStatus.eActive;

            switch( _Mode )
            {
                case Mode.eStartWithOn:
                     _PwmControllerEventArgs.SwitchStatus = SwitchStatus.eOn;
                     EAnyStatusChanged?.Invoke( this, _PwmControllerEventArgs );
                     break;

                case Mode.eStartWithOff:
                     break;
            }
        }

        public void Stop( )
        {

        }
        #endregion


    }
}
