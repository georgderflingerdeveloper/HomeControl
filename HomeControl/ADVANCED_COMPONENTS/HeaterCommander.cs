using HomeControl.ADVANCED_COMPONENTS.Interfaces;

namespace HomeControl.ADVANCED_COMPONENTS
{
    class HeaterCommander : DeviceCommander, IHeaterCommander
    {
        HeaterCommanderConfiguration _heaterconfig;
        #region CONSTRUCTOR
        public HeaterCommander( HeaterCommanderConfiguration config, IDeviceControlTimer devicecontroltimer ) : base( config, devicecontroltimer )
        {
            _heaterconfig = config;
        }
        #endregion

        public void EventSwitch( bool command )
        {
            EventSwitch_(  command );
        }

        void EventSwitch_(  bool command )
        {
            if( _heaterconfig.Options == HOptions.EventSwitchInactive )
            {
                return;
            }
            else if( _heaterconfig.Options == HOptions.EventSwitchActive )
            {
                switch( State )
                {
                    case DeviceState.Inactive:
                         break;

                    case DeviceState.Activated:
                         Update( _heaterconfig.Startindex, command );
                         break;
                }
            }
        }
    }
}
