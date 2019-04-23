using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.ROOMS.CONFIGURATION
{
    [Serializable]
    public class BaseConfiguration
    {
        public CommunicationConfiguration CommunicationConfig { get; set; }
    }

    class CommonConfiguration
    {
    }
}
