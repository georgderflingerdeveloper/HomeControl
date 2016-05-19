using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
