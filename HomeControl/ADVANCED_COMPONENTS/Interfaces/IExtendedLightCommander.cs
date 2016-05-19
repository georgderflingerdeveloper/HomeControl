

namespace HomeControl.ADVANCED_COMPONENTS.Interfaces
{
    interface IExtendedLightCommander
    {
        void  ScenarioTrigger( bool trigger );
        void  PresenceTrigger( bool edge );
        event ExtUpdate  ExtUpdate;
    }
}
