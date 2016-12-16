using System;

namespace HomeControl.ADVANCED_COMPONENTS
{
    public enum HOptions
    {
        EventSwitchInactive,
        EventSwitchActive
    }

    public enum HOptionProperties
    {
        SwitchOffEvent,
        SwitchOnEvent
    }

    public class PwmIntervall
    {
        TimeSpan DurationOn;
        TimeSpan DurationOff;
    }

    [Serializable]
    public class PwmParameters
    {
        PwmIntervall     _PwmStep1;
        PwmIntervall     _PwmStep2;
        PwmIntervall     _PwmStep3;

        TimeSpan         _DurationForActivationPwmStep1;
        TimeSpan         _DurationForActivationPwmStep2;
        TimeSpan         _DurationForActivationPwmStep3;

        public PwmIntervall PwmStep1
        {
            get
            {
                return _PwmStep1;
            }

            set
            {
                _PwmStep1 = value;
            }
        }

        public PwmIntervall PwmStep2
        {
            get
            {
                return _PwmStep2;
            }

            set
            {
                _PwmStep2 = value;
            }
        }

        public PwmIntervall PwmStep3
        {
            get
            {
                return _PwmStep3;
            }

            set
            {
                _PwmStep3 = value;
            }
        }

        public TimeSpan DurationForActivationPwmStep1
        {
            get
            {
                return _DurationForActivationPwmStep1;
            }

            set
            {
                _DurationForActivationPwmStep1 = value;
            }
        }

        public TimeSpan DurationForActivationPwmStep2
        {
            get
            {
                return _DurationForActivationPwmStep2;
            }

            set
            {
                _DurationForActivationPwmStep2 = value;
            }
        }

        public TimeSpan DurationForActivationPwmStep3
        {
            get
            {
                return _DurationForActivationPwmStep3;
            }

            set
            {
                _DurationForActivationPwmStep3 = value;
            }
        }
    }


    [Serializable]
    public class HeaterCommanderConfiguration : CommanderConfiguration
    {
        HOptions            _Options;
        HOptionProperties   _OptionProperties;
        PwmParameters       _PwmParameters;

        public HOptions Options
        {
            get
            {
                return _Options;
            }

            set
            {
                _Options = value;
            }
        }

        public HOptionProperties OptionProperties
        {
            get
            {
                return _OptionProperties;
            }

            set
            {
                _OptionProperties = value;
            }
        }

        public PwmParameters PwmParameters
        {
            get
            {
                return _PwmParameters;
            }

            set
            {
                _PwmParameters = value;
            }
        }
    }
}
