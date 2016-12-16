
namespace HomeControl.ADVANCED_COMPONENTS.Interfaces
{
    interface IPwmController
    {
         PwmStatus Status { set; }
         void Start( );
         void Stop( );
         event AnyStatusChanged EAnyStatusChanged;
    }
}
