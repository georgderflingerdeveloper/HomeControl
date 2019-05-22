namespace HomeControl.ADVANCED_COMPONENTS.Interfaces
{
    public interface IDeviceControlTimer
    {
        void StartOn( );
        void StopOn( );
        void StartOff( );
        void RestartOff( );
        void StartFinalOff( );
        void StopFinalOff( );

        event ControlOn        EControlOn;

        event ControlOff       EControlOff;

        event ControlFinalOff  EControlFinalOff;
    }
}
