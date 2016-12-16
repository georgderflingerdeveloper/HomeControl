

namespace HomeControl.ADVANCED_COMPONENTS.Interfaces
{
    interface IExtendedLightCommander
    {
        int  ScenarioTrigger( bool trigger );
        void  PresenceTrigger( bool edge );
        event ExtUpdate  ExtUpdate;
    }
}
