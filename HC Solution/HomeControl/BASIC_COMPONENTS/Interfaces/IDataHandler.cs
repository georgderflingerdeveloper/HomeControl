using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.BASIC_COMPONENTS.Interfaces
{
    interface IDataHandler
    {
        void Store( object data, string filename );
        object Load( object expecteddata, string filename );
    }
}
