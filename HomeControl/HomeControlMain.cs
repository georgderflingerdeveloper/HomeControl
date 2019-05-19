using System;
using System.Diagnostics;
using HomeControl.BASIC_CONSTANTS;
using HomeControl.ADVANCED_COMPONENTS;
using System.Reflection;
using SystemServices;
using HomeControl.ROOMS;
using BASIC_COMPONENTS;
using HomeControl.BASIC_COMPONENTS;
using System.Threading;
using LibUdp;
using LibUdp.BASIC;

namespace HomeControl
{
    class HomeControlMain
    {
        static string                        _SelectedRoomMode;
        static AnteBathWashRoomController    _AnteBathWashRoomController;
        static AnteBathWashRoomConfiguration _AnteBathWashRoomConfiguration;
        static object                        _RoomConfiguration;
        static int                           IndexDigitalOutputReserverdForHeartBeat = 15;
        static double                        IntervallTimeHeartBeat                  = 500;
        static DataHandler                   _Datahandler = new DataHandler();
        static ControllerConfiguration       _Controller = new ControllerConfiguration();
        static object                        _ControllerData;
        static DeviceBlinker                 _HeartBeat = new DeviceBlinker( new BlinkerConfiguration( IndexDigitalOutputReserverdForHeartBeat, StartBlinker.eWithOnPeriode ), new Timer_( IntervallTimeHeartBeat ) );

        static void WaitUntilKeyPressed()
        {
           Console.WriteLine( "Press enter for terminate application" );
           while (Console.ReadKey( true ).Key != ConsoleKey.Enter)
           { Thread.Sleep( 1000 ); };
        }

        static void Main( string[] args )
        {
            InitConsoleOutput( );
            SystemInformation( );

            LoadConfigurationData_Controller( );

            _SelectedRoomMode = _Controller?.SelectedRoom;

            switch( _SelectedRoomMode?.ToUpper() )  // .ToUpper() is only for TEXT formatting reasons, because I like ROOM NAMES in UPPER CASE ....
            {
                   case InfoOperationMode.ANTEROOM:
                        AnteRoom( );
                        break;
             }

            Finish( );
        }

        #region PRIVATE_METHODS

        static void InitConsoleOutput( )
        {
            Debug.Listeners.Add( new TextWriterTraceListener( Console.Out ) );
            Debug.AutoFlush = true;
            Debug.Indent( );
        }

        static void SystemInformation( )
        {
            Console.WriteLine( basicstringconstants.ApplicationTitle );
            Console.WriteLine( );
            Console.WriteLine( basicstringconstants.InfoStartingTime + DateTime.Now );
            Console.WriteLine( );
            string Version_ = Assembly.GetExecutingAssembly( ).GetName( ).Version.ToString( );
            string CompleteVersion = basicstringconstants.InfoVersion + Version_ + Seperators.Spaceholder + basicstringconstants.InfoLastBuild + ApplicationInformation.BuildDate.ToString( );
            Console.WriteLine( basicstringconstants.InfoVersioninformation + Version_ );
            Console.WriteLine( );
            Console.WriteLine( basicstringconstants.InfoLastBuild + ApplicationInformation.BuildDate.ToString( ) );
            Console.WriteLine( );
        }

        static void LoadConfigurationData_Controller( )
        {
            Console.WriteLine( Messages.LoadingFile + Messages.WhiteSpace + FileNames.Controller + Messages.WhiteSpace + Messages.DottedSpace );
            Console.WriteLine( );
            _ControllerData = _Datahandler.Load( _Controller, FileNames.Controller );
            _Controller = _ControllerData as ControllerConfiguration;
        }

        static object LoadConfigurationData_Room( string room, object configuration )
        {
            Console.WriteLine( Messages.LoadingFile + Messages.WhiteSpace + room + FileNames.format + Messages.WhiteSpace + Messages.DottedSpace );
            Console.WriteLine( );
            return ( _Datahandler.Load( configuration, room + FileNames.format ) );
        }

        static void ConfigurationInformation( )
        {
        }

        static void Finish( )
        {
            Console.ReadLine( );
            Environment.Exit( 0 );
        }

        static void AnteRoom( )
        {
            _AnteBathWashRoomConfiguration = new AnteBathWashRoomConfiguration( );
            _RoomConfiguration = LoadConfigurationData_Room( _SelectedRoomMode.ToLower( ), _AnteBathWashRoomConfiguration );
            if ( _RoomConfiguration != null )
            {
                _AnteBathWashRoomConfiguration = (_RoomConfiguration as AnteBathWashRoomConfiguration);
            }
            LibUdp.UdpBasicSenderReceiver SenderReceiver
                = new UdpBasicSenderReceiver(
                                              _AnteBathWashRoomConfiguration.CommunicationConfig.PortFromWhereDataIsReceived,
                                              _AnteBathWashRoomConfiguration.CommunicationConfig.IpAdressWhereDataIsSent,
                                              _AnteBathWashRoomConfiguration.CommunicationConfig.PortWhereDataIsSentTo
                                             );

            IOHandler IOHandler_ = new IOHandler( HandlerMode.eHardware );

            if( IOHandler_.Attached )
            {
                IOHandler_.EDigitalInputChanged  += IOHandler__EDigitalInputChanged;
                IOHandler_.EDigitalOutputChanged += IOHandler__EDigitalOutputChanged;
                IOHandler_.SetAllOutputs( false );
                Console.WriteLine( );
                Console.WriteLine( basicstringconstants.OperationMode + _SelectedRoomMode );
                Console.WriteLine( );
                _AnteBathWashRoomController = new AnteBathWashRoomController( _AnteBathWashRoomConfiguration, _HeartBeat, IOHandler_, SenderReceiver );
                WaitUntilKeyPressed( );
                IOHandler_.SetAllOutputs( false );
                Environment.Exit( 0 );
            }
        }
        #endregion

        static void Debug_()
        {
            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.LightCommanderConfiguration.DelayTimeAllOn.ToString( ) );
            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.LightCommanderConfiguration.DelayTimeAutomaticFinalOff.ToString( ) );
            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.LightCommanderConfiguration.DelayTimeDoingNothing.ToString( ) );
            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.LightCommanderConfiguration.DelayTimeFinalOff.ToString( ) );
            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.LightCommanderConfiguration.DelayTimeOffByMissingTriggerSignal.ToString( ) );
            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.LightCommanderConfiguration.DeviceRemainOnAfterAutomaticOff.ToString( ) );

            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.LightCommanderConfiguration.IndexSelectedHardware.ToString( ) );
            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.LightCommanderConfiguration.Lastindex.ToString( ) );
            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.LightCommanderConfiguration.ModesAutomaticoff.ToString( ) );
            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.LightCommanderConfiguration.Modesdelayedon.ToString( ) );
            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.LightCommanderConfiguration.Startindex.ToString( ) );
            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.LightCommanderConfiguration.DeviceRemainOnAfterAutomaticOff.Count.ToString( ));

            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.ScenarioConfiguration.DelayTimeNextScenario.ToString( ) );
            Console.WriteLine( _AnteBathWashRoomConfiguration.AnteRoom.ScenarioConfiguration.Scenarios.Count.ToString( ) );
        }

        #region EVENTHANDLERS
        private static void IOHandler__EDigitalInputChanged( object sender, DigitalInputEventargs e )
        {
            switch( _SelectedRoomMode.ToUpper() )
            {
                case InfoOperationMode.ANTEROOM:
                     break;
            }
        }

        private static void IOHandler__EDigitalOutputChanged( object sender, DigitalOutputEventargs e )
        {
            switch( _SelectedRoomMode.ToUpper() )
            {
                case InfoOperationMode.ANTEROOM:
                     break;
            }
        }
        #endregion
    }
}
