
namespace HomeControl.ADVANCED_COMPONENTS
{
    interface IDeviceBlinker
    {
        void Start( );
        void Stop();
        event Update EUpdate;
    }
}
