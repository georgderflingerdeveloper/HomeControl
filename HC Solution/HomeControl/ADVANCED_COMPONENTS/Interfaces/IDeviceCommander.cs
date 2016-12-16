namespace HomeControl.ADVANCED_COMPONENTS
{
    interface IDeviceCommander
    {
        void MainTrigger( bool edge );
        void PresenceTrigger( bool edge );
        event Update EUpdate;
        void Reset( );
    }
}
