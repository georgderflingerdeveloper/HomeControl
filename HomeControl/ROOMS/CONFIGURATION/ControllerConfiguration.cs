using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.ROOMS
{
    public class ControllerConfiguration
    {
        string _IpAdressServer;
        uint _PortServer;
        string _SelectedRoom;

        public string IpAdressServer
        {
            get
            {
                return _IpAdressServer;
            }

            set
            {
                _IpAdressServer = value;
            }
        }

        public uint PortServer
        {
            get
            {
                return _PortServer;
            }

            set
            {
                _PortServer = value;
            }
        }

        public string SelectedRoom
        {
            get
            {
                return _SelectedRoom;
            }

            set
            {
                _SelectedRoom = value;
            }
        }
    }
}
