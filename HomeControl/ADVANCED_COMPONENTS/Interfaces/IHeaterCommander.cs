
namespace HomeControl.ADVANCED_COMPONENTS
{
    public interface IHeaterCommander : IDeviceCommander
    {
        void EventSwitch( bool command );
    }
}
