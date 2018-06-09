using System;
using System.Collections.Generic;

namespace HomeControl.ROOMS.CONFIGURATION
{
    [Serializable]
    public class HardwareConfiguration
    {
        List<int> _IOPrimerIds  = new List<int>();
        List<string> _IOPrimerNames  = new List<string>();

        public List<int> IOPrimerIds
        {
            get
            {
                return _IOPrimerIds;
            }

            set
            {
                _IOPrimerIds = value;
            }
        }

        public List<string> IOPrimerNames
        {
            get
            {
                return _IOPrimerNames;
            }

            set
            {
                _IOPrimerNames = value;
            }
        }
  }
}
