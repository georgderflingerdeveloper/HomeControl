using HomeControl.BASIC_CONSTANTS;
using BASIC_COMPONENTS;
using System.Timers;
using System;

namespace HomeControl.ADVANCED_COMPONENTS
{
    public enum SequencerModi
    {
        Single,
        MultipleCount,
        MultipleDuration,
    }

    public enum SequencerProperty
    {
        StartWithHighPulse,
    }

    public enum SequencerState
    {
        Inactive,
        Running,
        WaitForNextSequence,
        Finished
    }

    public class SeqUpdateEventArgs : UpdateEventArgs
    {
        SequencerState _State;
        decimal _ActualCountSingleSignals; 
        decimal _ActualCountSequences;     

        public SequencerState State
        {
            get
            {
                return _State;
            }

            set
            {
                _State = value;
            }
        }

        public decimal ActualCountSingleONSignals
        {
            get
            {
                return _ActualCountSingleSignals;
            }

            set
            {
                _ActualCountSingleSignals = value;
            }
        }  // number of 0 means endless

        public decimal ActualCountSequences
        {
            get
            {
                return _ActualCountSequences;
            }

            set
            {
                _ActualCountSequences = value;
            }
        }  // number of 0 means endless
    }

    [Serializable]
    public class DeviceSignalSequencerConfiguration
    {
        SequencerModi      _SequencerMode;
        SequencerProperty  _SequencerProperty;

        int           _index;
        decimal       _CountSequencePeriodes;
        decimal       _CountSequenceSignals;
        double        _DurationONBetweenSingleSignal;
        double        _DurationOFFBetweenSingleSignal;
        double        _DurationBetweenSequence;

        public DeviceSignalSequencerConfiguration( int index, SequencerModi mode )
        {
            _index         = index;
            SequencerMode  = mode;
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

        public SequencerModi SequencerMode
        {
            get
            {
                return _SequencerMode;
            }

            set
            {
                _SequencerMode = value;
            }
        }

        public decimal CountSequencePeriodes
        {
            get
            {
                return _CountSequencePeriodes;
            }

            set
            {
                _CountSequencePeriodes = value;
            }
        }

        public decimal CountSequenceSignals
        {
            get
            {
                return _CountSequenceSignals;
            }

            set
            {
                _CountSequenceSignals = value;
            }
        }

        public double DurationONBetweenSingleSignal
        {
            get
            {
                return _DurationONBetweenSingleSignal;
            }

            set
            {
                _DurationONBetweenSingleSignal = value;
            }
        }

        public double DurationOFFBetweenSingleSignal
        {
            get
            {
                return _DurationOFFBetweenSingleSignal;
            }

            set
            {
                _DurationOFFBetweenSingleSignal = value;
            }
        }

        public double DurationBetweenSequence
        {
            get
            {
                return _DurationBetweenSequence;
            }

            set
            {
                _DurationBetweenSequence = value;
            }
        }

        public SequencerProperty SequencerProperty
        {
            get
            {
                return _SequencerProperty;
            }

            set
            {
                _SequencerProperty = value;
            }
        }
    }

    public delegate void SeqUpdate( object sender, SeqUpdateEventArgs e );

    public class DeviceSignalSequencer : IDeviceSequencer
    {
        bool[] _ToggleOutput               = new bool[CommanderConfiguration.NumberOfOutputs];
        public event SeqUpdate             EUpdate;
        SeqUpdateEventArgs                 _UpdateEventArgs = new SeqUpdateEventArgs();
        DeviceSignalSequencerConfiguration _config;
        decimal StartValueSignalCount   = 0;
        decimal StartValueSequenceCount = 1;
        decimal SignalCounter   = 0;
        decimal SequenceCounter = 0;
        ITimer _PulseOnTimer;
        ITimer _PulseOffTimer;
        ITimer _NextSequenceTimer;
        ITimer _SequencerFinishedTimer;


        #region CONSTRUCTOR
        // diverse constructors are used for different modi
        public DeviceSignalSequencer( DeviceSignalSequencerConfiguration config,
                                      ITimer PulseOnTimer,
                                      ITimer PulseOffTimer
                                    )
        {
            _config = config;
            _PulseOnTimer            = PulseOnTimer;
            _PulseOffTimer           = PulseOffTimer;
            _PulseOnTimer.Elapsed   += _PulseOnTimer_Elapsed;
            _PulseOffTimer.Elapsed  += _PulseOffTimer_Elapsed;
            Constructor( );
        }

        public DeviceSignalSequencer( DeviceSignalSequencerConfiguration config,
                                      ITimer PulseOffTimer,
                                      ITimer PulseOnTimer,
                                      ITimer NextSequenceTimer  )
        {
             _config                          = config;
             _PulseOnTimer                    = PulseOnTimer;
             _PulseOffTimer                   = PulseOffTimer;
             _NextSequenceTimer               = NextSequenceTimer;
             _PulseOnTimer.Elapsed           += _PulseOnTimer_Elapsed;
             _PulseOffTimer.Elapsed          += _PulseOffTimer_Elapsed;
             _NextSequenceTimer.Elapsed      += _NextSequenceTimer_Elapsed;
             Constructor( );
        }

        public DeviceSignalSequencer( DeviceSignalSequencerConfiguration config,
                                      ITimer PulseOffTimer,
                                      ITimer PulseOnTimer,
                                      ITimer NextSequenceTimer,
                                      ITimer SequencerFinishedTimer )
        {
            _config                          = config;
            _PulseOnTimer                    = PulseOnTimer;
            _PulseOffTimer                   = PulseOffTimer;
            _NextSequenceTimer               = NextSequenceTimer;
            _SequencerFinishedTimer          = SequencerFinishedTimer;
            _PulseOnTimer.Elapsed           += _PulseOnTimer_Elapsed;
            _PulseOffTimer.Elapsed          += _PulseOffTimer_Elapsed;
            _NextSequenceTimer.Elapsed      += _NextSequenceTimer_Elapsed;
            _SequencerFinishedTimer.Elapsed += _SequencerFinishedTimer_Elapsed;
            Constructor( );
        }
        #endregion

        #region INTERFACE_IMPLEMENTATION
        public void Start( )
        {
            bool StartSignal = false;
            SignalCounter++;
            SequenceCounter = StartValueSequenceCount;
            _UpdateEventArgs.ActualCountSingleONSignals = SignalCounter;
            _UpdateEventArgs.State = SequencerState.Running;
            Update_( );

            switch( _config.SequencerProperty )
            {
                case SequencerProperty.StartWithHighPulse:
                     StartSignal = true;
                     break;
            }

            switch( _config.SequencerMode )
            {
                case SequencerModi.MultipleCount:
                case SequencerModi.Single:
                     TurnSingleDevice( _config.Index, StartSignal );
                     break;
            }
            _PulseOnTimer.Start( );
        }

        public void Stop( )
        {
            if( _UpdateEventArgs.State == SequencerState.Finished )
            {
                return;
            }

            TurnSingleDevice( _config.Index, false );

            _NextSequenceTimer?.Stop( );
            _PulseOnTimer.Stop( );
            _PulseOffTimer.Stop( );

            SequenceCounter = StartValueSequenceCount;
            _UpdateEventArgs.ActualCountSequences = SequenceCounter;

            SignalCounter = StartValueSignalCount;
            _UpdateEventArgs.ActualCountSingleONSignals = SignalCounter;

            _UpdateEventArgs.State = SequencerState.Finished;

            Update_( );
        }

        public void ReconfigIntervalls( double on, double off )
        {
            _PulseOnTimer.SetIntervall( on );
            _PulseOffTimer.SetIntervall( off );
        }

        public void ReconfigIntervalls( double on, double off, double sequenceintervall )
        {
            _PulseOnTimer.SetIntervall( on );
            _PulseOffTimer.SetIntervall( off );
            _NextSequenceTimer.SetIntervall( sequenceintervall );
        }

        public void Update()
        {
            Update_( );
        }
        #endregion

        #region PRIVATEMETHODS
        void Update_()
        {
             EUpdate?.Invoke( this, _UpdateEventArgs );
        }
        void Constructor()
        {
            _UpdateEventArgs.State = SequencerState.Inactive;
            _UpdateEventArgs.ActualCountSequences       = StartValueSequenceCount;
            SequenceCounter = _UpdateEventArgs.ActualCountSequences;
            _UpdateEventArgs.ActualCountSingleONSignals = StartValueSignalCount;
        }

        void TurnSingleDevice( int index, bool value )
        {
            if( ( index < CommanderConfiguration.NumberOfOutputs ) && ( index >= 0 ) )
            {
                _UpdateEventArgs.Index = index;
                _UpdateEventArgs.Value = value;
                _UpdateEventArgs.State = SequencerState.Running;
                EUpdate?.Invoke( this, _UpdateEventArgs );
            }
        }

        void StopPulseTimers( )
        {
            _PulseOffTimer.Stop( );
            _PulseOnTimer.Start( );
        }
        #endregion

        #region PROTECTED_METHODS
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
        private void _SequencerFinishedTimer_Elapsed( object sender, ElapsedEventArgs e )
        {
        }

        private void _NextSequenceTimer_Elapsed( object sender, ElapsedEventArgs e )
        {
            _NextSequenceTimer.Stop( );
            _PulseOnTimer.Start( );
            _UpdateEventArgs.Value = TurnDevice.ON;
            _UpdateEventArgs.State = SequencerState.Running;
            _UpdateEventArgs.ActualCountSequences = ++SequenceCounter;
            SignalCounter = StartValueSignalCount;
            SignalCounter++;
            _UpdateEventArgs.ActualCountSingleONSignals = SignalCounter;
            EUpdate?.Invoke( this, _UpdateEventArgs );
        }

        private void _PulseOnTimer_Elapsed( object sender, ElapsedEventArgs e )
        {
            _PulseOnTimer.Stop( );
            _UpdateEventArgs.Value = TurnDevice.OFF;

            if( (_UpdateEventArgs.ActualCountSingleONSignals < _config.CountSequenceSignals) || (_config.CountSequenceSignals == 0) )
            {
                _PulseOffTimer.Start( );
            }
            else if( _UpdateEventArgs.ActualCountSingleONSignals >= _config.CountSequenceSignals )
            {
                _PulseOffTimer.Stop( );
                switch( _config.SequencerMode )
                {
                    case SequencerModi.MultipleCount:
                         SignalCounter = StartValueSignalCount;
                         if( ( _UpdateEventArgs.ActualCountSequences < _config.CountSequencePeriodes ) || ( _config.CountSequencePeriodes == 0 ) )
                         {
                              _UpdateEventArgs.State = SequencerState.WaitForNextSequence;
                              _NextSequenceTimer.Start( );
                         }
                         else
                         {
                             _UpdateEventArgs.ActualCountSequences = StartValueSequenceCount;
                             _NextSequenceTimer.Stop( );
                             _UpdateEventArgs.State = SequencerState.Finished;
                         }
                         break;

                   case SequencerModi.Single:
                        SignalCounter = StartValueSignalCount;
                        _UpdateEventArgs.State = SequencerState.Finished;
                        break;
                }
           }
           EUpdate?.Invoke( this, _UpdateEventArgs );
        }

        private void _PulseOffTimer_Elapsed( object sender, ElapsedEventArgs e )
        {
            _UpdateEventArgs.Value = TurnDevice.ON;
            SignalCounter++;
            _UpdateEventArgs.ActualCountSingleONSignals = SignalCounter;
            EUpdate?.Invoke( this, _UpdateEventArgs );
            _PulseOffTimer.Stop( );
            _PulseOnTimer.Start( );
        }
        #endregion
    }
}
