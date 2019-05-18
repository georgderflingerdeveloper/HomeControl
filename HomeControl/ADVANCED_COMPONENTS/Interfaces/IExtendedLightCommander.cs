
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.ROOMS.CONFIGURATION;
namespace HomeControl.ADVANCED_COMPONENTS.Interfaces
{
    public interface IExtendedLightCommander :ILightCommander
    {
        int   ScenarioTrigger( bool trigger );
        new void  PresenceTrigger( bool edge );

        event ExtUpdate  ExtUpdate;
        void UpdateConfig(BaseConfiguration config);

        int ScenarioTriggerPersitent(bool edge, int number);

        UpdateEventArgs TurnSingleDevice(bool value, int index);
    }
}
