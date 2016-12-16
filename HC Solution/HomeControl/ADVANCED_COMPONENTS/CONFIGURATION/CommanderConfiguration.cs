using System.Collections.Generic;
using HomeControl.HARDCONFIG;
using System;

namespace HomeControl.ADVANCED_COMPONENTS
{
    public enum DeviceCommandos
    {
        None,
        OnOff,
        OnWithDelayedOffByRisingEdge,
        OnWithDelayedOffByFallingEdge,
        AllOn,
        AllOff,
        SingleDeviceOnOffFallingEdge,
        SingleDeviceToggleRisingEdge,
        StepDevice,
        SingleLight,
        ScenarioLight,
    }

    public enum DeviceCommanderDelayedOn
    {
        None,
        WithMainTrigger,
        WithPresenceTrigger,
        WithMainAndPresenceTrigger
    }

    public enum DeviceCommanderAutomaticOff
    {
        None,
        WithMainTrigger,
        WithPresenceTrigger,
        WithMainAndPresenceTrigger
    }

    [Serializable]
    public class CommanderConfiguration
    {
        static  int    _numberofoutputs = basichardconfig.NumberOfDigitalOutputsPrimer16;
        int            _startindex;
        int            _lastindex;
        int            _IndexSelectedHardware;
        double         _DelayTimeAllOn;
        double         _DelayTimeFinalOff;
        double         _DelayTimeOffByAbsentPresenceSignal;
        double         _DelayTimeAutomaticOff;
        double         _DelayTimeDoingNothing;
        List<int>      _DeviceRemainOnAfterAutomaticOff = new List<int>();

        DeviceCommandos              _modes;
        DeviceCommanderAutomaticOff  _modesautomaticoff;
        DeviceCommanderDelayedOn     _modesdelayedon;

        public int Startindex
        {
            get
            {
                return _startindex;
            }

            set
            {
                _startindex = value;
            }
        }

        public int Lastindex
        {
            get
            {
                return _lastindex;
            }

            set
            {
                _lastindex = value;
            }
        }

        public double DelayTimeAllOn
        {
            get
            {
                return _DelayTimeAllOn;
            }

            set
            {
                _DelayTimeAllOn = value;
            }
        }

        public double DelayTimeAutomaticFinalOff
        {
            get
            {
                return _DelayTimeFinalOff;
            }

            set
            {
                _DelayTimeFinalOff = value;
            }
        }

        public double DelayTimeOffByMissingTriggerSignal
        {
            get
            {
                return _DelayTimeOffByAbsentPresenceSignal;
            }

            set
            {
                _DelayTimeOffByAbsentPresenceSignal = value;
            }
        }

        public DeviceCommandos Modes
        {
            get
            {
                return _modes;
            }

            set
            {
                _modes = value;
            }
        }

        public static int NumberOfOutputs
        {
            get
            {
                return _numberofoutputs;
            }
        }

        public DeviceCommanderAutomaticOff ModesAutomaticoff
        {
            get
            {
                return _modesautomaticoff;
            }

            set
            {
                _modesautomaticoff = value;
            }
        }

        public List<int> DeviceRemainOnAfterAutomaticOff
        {
            get
            {
                return _DeviceRemainOnAfterAutomaticOff;
            }

            set
            {
                _DeviceRemainOnAfterAutomaticOff = value;
            }
        }

        public DeviceCommanderDelayedOn Modesdelayedon
        {
            get
            {
                return _modesdelayedon;
            }

            set
            {
                _modesdelayedon = value;
            }
        }

        public double DelayTimeDoingNothing
        {
            get
            {
                return _DelayTimeDoingNothing;
            }

            set
            {
                _DelayTimeDoingNothing = value;
            }
        }

        public double DelayTimeFinalOff
        {
            get
            {
                return _DelayTimeAutomaticOff;
            }

            set
            {
                _DelayTimeAutomaticOff = value;
            }
        }

        public int IndexSelectedHardware
        {
            get
            {
                return _IndexSelectedHardware;
            }

            set
            {
                _IndexSelectedHardware = value;
            }
        }
    }
}
