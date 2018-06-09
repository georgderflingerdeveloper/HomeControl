using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.ROOMS.CONFIGURATION
{
    [Serializable]
    public class CommunicationConfiguration
    {
        string _IpAdressWhereDataIsSent ="127.0.0.1";
        public string IpAdressWhereDataIsSent
        { get => _IpAdressWhereDataIsSent; set => _IpAdressWhereDataIsSent = value; }
        public int PortFromWhereDataIsReceived { get; set; }
        public int PortWhereDataIsSentTo { get; set; }
    }
}
