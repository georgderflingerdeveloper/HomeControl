using BASIC_COMPONENTS;
using HomeControl.BASIC_CONSTANTS;
using System;
using System.Timers;
using System.Collections.Generic;
using SystemServices;

namespace HomeControl.ADVANCED_COMPONENTS
{
    public class ScenarioEventArgs : EventArgs
    {
        int _number;
        int  _index;
        bool _value;

        public int Number
        {
            get
            {
                return _number;
            }

            set
            {
                _number = value;
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
    }

    public class ScenarioControlEventArgs : EventArgs
    {
        ScenarioCntrl _control;
        bool _value;

        public ScenarioCntrl Control
        {
            get
            {
                return _control;
            }

            set
            {
                _control = value;
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
    }

    public enum ScenarioProperty
    {
        None,
        SingleStep,
        AutoStep,
        RandomStep
    }

    public enum ScenarioState
    {
        Toggle,
        Idle,
        PostIdle,
        Next,
        DevicesON,
        DevicesOFF,
        None = 99
    }

    public class ScenarioCntrl
    {
        ScenarioProperty _Property;
        ScenarioState    _State;

        public ScenarioProperty Property
        {
            get
            {
                return _Property;
            }

            set
            {
                _Property = value;
            }
        }

        public ScenarioState State
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
    }

    [Serializable]
    public class ScenarioConfiguration
    {
        double         _DelayTimeNextScenario;
        List<List<int>>    _Scenarios = new List<List<int>>();  // contains indices for different scenarios

        public List<List<int>> Scenarios
        {
            get
            {
                return _Scenarios;
            }

            set
            {
                _Scenarios = value;
            }
        }

        public double DelayTimeNextScenario
        {
            get
            {
                return _DelayTimeNextScenario;
            }

            set
            {
                _DelayTimeNextScenario = value;
            }
        }
    }

    public delegate void Scenario( object sender, ScenarioEventArgs e );
    public delegate void ScenarioControlInfo( object sender, ScenarioControlEventArgs e );

    class DeviceScenarioControl : IDeviceScenarioControl
    {
        #region DECLARATION
        ITimer   _NextScenario;
 //       ITimer   _AutoScenario;
        ITimer   _IdleScenario;
        ScenarioCntrl _ScenarioCntrl = new ADVANCED_COMPONENTS.ScenarioCntrl();
        ScenarioEventArgs ScenarioStep = new ScenarioEventArgs();
        ScenarioControlEventArgs ScenarioArgs = new ScenarioControlEventArgs();
        int    _beginswithindex;
        int    _endswithindex;
        int    _ActualScenarioNumber;
        int    _LastSelectedSecenarioNumber;
        bool   _UsedRemoteControl;
        bool   _value;
        bool   _idlevalue; 
        List<List<int>> _scenarios = new List<List<int>>();

        public event Scenario EScenario;
        public event ScenarioControlInfo EScenarioControlInfo;
        #endregion

        #region CONSTRUCTOR
        public DeviceScenarioControl( int beginswithindex, int endswithindex, ITimer NextScenario, ITimer AutoScenario, ITimer IdleScenario )
        {
            _NextScenario           = NextScenario;
            _beginswithindex        = beginswithindex;
            _endswithindex          = endswithindex;
            _IdleScenario           = IdleScenario;
            _NextScenario.Elapsed  += _NextScenario_Elapsed;
            _IdleScenario.Elapsed  += _IdleScenario_Elapsed;
            _ScenarioCntrl.State    = ScenarioState.Toggle;
            _ScenarioCntrl.Property = ScenarioProperty.SingleStep;
            _value = false;
        }
       #endregion

        #region PRIVATE_METHODS
        void Watcher( bool trigger )
        {
            switch( trigger )
            {
                case Trigger.RISING:
                     _NextScenario.Start( );
                     _IdleScenario.Start( );
                     break;

                case Trigger.FALLING:
                     _NextScenario.Stop( );
                     _IdleScenario.Stop( );
                     ScenarioStateMachine( ref _ScenarioCntrl );
                     break;
            }
        }

        void ScenarioStateMachine( ref ScenarioCntrl scenario )
        {
            switch( scenario.State )
            {
                case ScenarioState.Idle:
                     scenario.State = ScenarioState.PostIdle;
                     break;

                case ScenarioState.PostIdle:
                     UpdateScenario( _idlevalue );
                     scenario.State = ScenarioState.Toggle;
                     _value = !_value;
                     break;

                case ScenarioState.Next:
                     PrepareNextScenario( );
                     scenario.State = ScenarioState.Toggle;
                     break;

                case ScenarioState.Toggle:
                     _value = !_value;
                     UpdateScenario( _value );
                     break;

                case ScenarioState.DevicesOFF:
                     break;

                case ScenarioState.DevicesON:
                     break;

                case ScenarioState.None:
                     break;

                default:
                     break;
            }
            ScenarioArgs.Control = scenario;
            ScenarioArgs.Value   = _value;
            EScenarioControlInfo?.Invoke( this, ScenarioArgs );
        }

        void ClearPreviousScenario( )
        {
            if( _ActualScenarioNumber > 0 )
            {
                int PreviousScenarioIndex = _ActualScenarioNumber - 1;
                List<int> Devices = _scenarios[PreviousScenarioIndex];
                int index = 0;
                foreach( var element in Devices )
                {
                        ScenarioStep.Index = _scenarios[PreviousScenarioIndex][index++];
                        ScenarioStep.Value = false;
                        if( ( ScenarioStep.Index >= _beginswithindex ) &&
                            ( ScenarioStep.Index <= _endswithindex ) )
                        {
                            EScenario?.Invoke( this, ScenarioStep );
                        }
                 }
             }
        }

        void UpdateScenario( bool updatevalue )
        {
            if( ( _ActualScenarioNumber < _scenarios.Count ) && ( _ActualScenarioNumber >= 0 ) )
            {
                List<int> DevicesPerScenario = _scenarios[_ActualScenarioNumber];
                int index = 0;
                foreach( var element in DevicesPerScenario )
                {
                    ScenarioStep.Index  = _scenarios[_ActualScenarioNumber][index++];
                    ScenarioStep.Number = _ActualScenarioNumber;
                    ScenarioStep.Value  = updatevalue;

                    if( ( ScenarioStep.Index >= _beginswithindex )    && 
                        ( ScenarioStep.Index <= _endswithindex   ) )
                    {
                        EScenario?.Invoke( this, ScenarioStep );
                    }
                }
            }
        }

        void NextScenario()
        {
            _ActualScenarioNumber++;
            if( _ActualScenarioNumber >= _scenarios.Count  )
            { 
                _ActualScenarioNumber = 0;
                Turn( PowerState.OFF, _ActualScenarioNumber );
            }
        }

        void PrepareNextScenario()
        {
            NextScenario( );
            ClearPreviousScenario( );
            UpdateScenario( true );
            _value = true;
        }

        void _Turn( bool command, int number )
        {
            for( int i = _beginswithindex; i <= _endswithindex; i++ )
            {
                ScenarioStep.Value = command;
                ScenarioStep.Index = i;
                ScenarioStep.Number = number;
                EScenario?.Invoke( this, ScenarioStep );
            }
        }

        void _TurnScenario( bool command, int number )
        {
            UpdateScenario( false );
            if( !_UsedRemoteControl )
            {
                _LastSelectedSecenarioNumber = _ActualScenarioNumber;
                _UsedRemoteControl = true;
            }
            _ActualScenarioNumber = number;
            UpdateScenario( command );
        }

        #endregion

        #region PUBLIC_METHODS
        public void Turn( bool command, int number )
        {
            _Turn(  command,  number );
        }

        public void TurnScenario( bool command, int number )
        {
            _TurnScenario( command, number );
        }

        public void WatchForInputValueChange( bool trigger )
        {
            if (_UsedRemoteControl)
            {
                 _ActualScenarioNumber = _LastSelectedSecenarioNumber;
                if (trigger)
                {
                    UpdateScenario( true );
                }
                else
                {
                    _UsedRemoteControl = false;
                    return;
                }
            }
            else
            {
                Watcher( trigger );
            }
        }

        public void ProceedCommandos( ScenarioCntrl scenario, int scenarionumber )
        {
            switch( scenario.State )
            {
                case ScenarioState.DevicesON:
                     Turn( PowerState.ON, scenarionumber );
                     break;

                case ScenarioState.DevicesOFF:
                     Turn( PowerState.OFF, scenarionumber );
                     break;

                case ScenarioState.Next:
                     PrepareNextScenario( );
                     break;

                default:
                     break;
            }
        }

        public void Reset( )
        {
           _NextScenario.Stop( );
            switch( _ScenarioCntrl.State )
            {
                case ScenarioState.PostIdle:
                     _idlevalue = true;
                     break;
                case ScenarioState.Toggle:
                     _value = false;
                     return;
            }
            _value = false;
       }

        public void PresetDeviceOff( )
        {
            _value = false;
        }

        public int GetActualScenarioNumber( )
        {
            return ( _ActualScenarioNumber );
        }

        public void SetSecenarioNumber( int desiredscenario )
        {
           _ActualScenarioNumber = desiredscenario;
        }
        #endregion

        #region PROPERTIES
        public int Number
        {
            get
            {
                return _ActualScenarioNumber;
            }

            set
            {
                _ActualScenarioNumber = value;
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

        public List<List<int>> Scenarios
        {
            get
            {
                return _scenarios;
            }

            set
            {
                try
                {
                    _scenarios = value;
                }catch( Exception ex )
                {
                    Services.TraceMessage_( ex.Message );
                }
            }
        }

        public ScenarioCntrl ScenarioCntrl_
        {
            get
            {
                return _ScenarioCntrl;
            }

            set
            {
                _ScenarioCntrl = value;
            }
        }
        #endregion

        #region EVENTHANDLERS
        private void _NextScenario_Elapsed( object sender, ElapsedEventArgs e )
        {
            _ScenarioCntrl.State = ScenarioState.Next;
            _NextScenario.Stop( );
        }

        private void _IdleScenario_Elapsed( object sender, ElapsedEventArgs e )
        {
            _idlevalue = !_value;
            _ScenarioCntrl.State = ScenarioState.Idle;
            _IdleScenario.Stop( );
        }
        #endregion
    }
}

