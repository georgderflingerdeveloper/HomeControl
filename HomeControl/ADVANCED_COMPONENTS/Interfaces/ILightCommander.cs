

namespace HomeControl.ADVANCED_COMPONENTS
{
    public interface ILightCommander
    {
        void MainTrigger( bool edge );
        void PresenceTrigger( bool edge );

        event Update EUpdate;
        void Reset( );
    }
}
