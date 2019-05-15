
using HomeControl.ADVANCED_COMPONENTS;
namespace HomeControl.ADVANCED_COMPONENTS.Interfaces
{
    public interface IExtendedLightCommander :ILightCommander
    {
        int   ScenarioTrigger( bool trigger );
        new void  PresenceTrigger( bool edge );
        event ExtUpdate  ExtUpdate;
    }
}
