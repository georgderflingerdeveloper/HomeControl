
namespace HomeControl.ADVANCED_COMPONENTS
{
    public interface IDeviceBlinker
    {
        void Start( );
        void Stop();
        event Update EUpdate;
    }
}
