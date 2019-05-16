
namespace HomeControl.ADVANCED_COMPONENTS
{
    interface IHeaterCommander : IDeviceCommander
    {
        void EventSwitch( bool command );
    }
}
