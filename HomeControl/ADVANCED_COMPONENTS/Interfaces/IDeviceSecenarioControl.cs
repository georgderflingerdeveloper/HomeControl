using System.Collections.Generic;

namespace HomeControl.ADVANCED_COMPONENTS
{
    interface IDeviceScenarioControl
    {
        void  WatchForInputValueChange( bool trigger );
        void  Turn( bool command, int number );
        void  TurnScenario( bool command, int number );
        void  ProceedCommandos( ScenarioCntrl scenario, int scenarionumber );
        void  Reset( );
        void  PresetDeviceOff( );
        event Scenario EScenario;
        event ScenarioControlInfo EScenarioControlInfo;
        int   GetActualScenarioNumber( );
        void SetSecenarioNumber( int desiredscenario );
    }
}
