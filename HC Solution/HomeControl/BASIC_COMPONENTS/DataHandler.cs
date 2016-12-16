using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Xml.Serialization;
using SystemServices;
using HomeControl.BASIC_CONSTANTS;
using HomeControl.BASIC_COMPONENTS.Interfaces;

namespace HomeControl.BASIC_COMPONENTS
{
    public class DataHandler : IDataHandler
    {
        string _filename;

        void Store_( object data )
        {
            try
            {
                Directory.GetCurrentDirectory( );
                XmlSerializer ser = new XmlSerializer( data?.GetType() );
                FileStream str = new FileStream( _filename, FileMode.Create );
                ser.Serialize( str, data );
                str.Close( );
            }
            catch( Exception ex )
            {
                Services.TraceMessage_( ex.Message, datahandlstringconst.DataStoringFailed );
            }
        }

        public void Store( object data, string filename )
        {
            _filename = filename;
            Store_( data );
        }

        public object Load( object expecteddata, string filename )
        {
            object readdata;
            try
            {
                Directory.GetCurrentDirectory( );
                XmlSerializer ser        = new XmlSerializer( expecteddata?.GetType() );
                StreamReader   sr        = new StreamReader( filename );
                readdata =  ser.Deserialize( sr );
                sr.Close( );
                return ( readdata );
            }
            catch( Exception ex )
            {
                Services.TraceMessage_( ex.Message, datahandlstringconst.DataLoadingFailed );
            }
            return ( null );
        }
    }
}
