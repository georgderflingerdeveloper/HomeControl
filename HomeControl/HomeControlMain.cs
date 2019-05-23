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
using HomeControl.ROOMS.CONFIGURATION;
using HomeControl.ROOMS.SLEEPING_ROOM;
using HomeControl.ADVANCED_COMPONENTS.Interfaces;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.ROOMS.SLEEPING_ROOM.INTERFACE;

namespace HomeControl
{
    class HomeControlMain
    {
        static string                        _SelectedRoomMode;

        static AnteBathWashRoomController    _AnteBathWashRoomController;
        static AnteBathWashRoomConfiguration _AnteBathWashRoomConfiguration;

        static SleepingRoomController        _SleepingRoomController;
        static SleepingRoomConfiguration     _SleepingRoomConfiguration;

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

                   case InfoOperationMode.SLEEPING_ROOM:
                        SleepingRoom();
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

        static void InfoMode()
        {
            Console.WriteLine();
            Console.WriteLine(basicstringconstants.OperationMode + _SelectedRoomMode);
            Console.WriteLine();
        }

        static void InfoWait()
        {
            Console.WriteLine("Waiting for attaching IO primer ....");
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
            InfoWait();

            IOHandler IOHandler_ = new IOHandler( HandlerMode.eHardware );


            if ( IOHandler_.Attached )
            {
                IOHandler_.SetAllOutputs( false );
                _AnteBathWashRoomController = new AnteBathWashRoomController( _AnteBathWashRoomConfiguration, 
                                                                              _HeartBeat, 
                                                                              IOHandler_, 
                                                                              SenderReceiver );
                InfoMode();
                WaitUntilKeyPressed( );
                IOHandler_.SetAllOutputs( false );
                Environment.Exit( 0 );
            }
        }

        static void SleepingRoom()
        {
            _SleepingRoomConfiguration = new SleepingRoomConfiguration();
            _RoomConfiguration = LoadConfigurationData_Room(_SelectedRoomMode.ToLower(), _SleepingRoomConfiguration);
            if (_RoomConfiguration != null)
            {
                _SleepingRoomConfiguration = (_RoomConfiguration as SleepingRoomConfiguration);
            }

            int PortFromWhereDataIsReceived = _SleepingRoomConfiguration.CommunicationConfig.PortFromWhereDataIsReceived;
            string IpAdressWhereDataIsSent  = _SleepingRoomConfiguration.CommunicationConfig.IpAdressWhereDataIsSent;
            int PortToWhereDataIsSent       = _SleepingRoomConfiguration.CommunicationConfig.PortWhereDataIsSentTo;

            UdpBasicSenderReceiver SenderReceiver
              = new UdpBasicSenderReceiver( PortFromWhereDataIsReceived,
                                            IpAdressWhereDataIsSent,
                                            PortToWhereDataIsSent
                                           );

            double TimeAllOn        
                = _SleepingRoomConfiguration.RoomConfig.LightCommanderConfiguration.DelayTimeAllOn;
            double TimeNextScenario 
                = _SleepingRoomConfiguration.RoomConfig.ScenarioConfiguration.DelayTimeNextScenario;

            CommanderConfiguration 
                CommanderConfig = _SleepingRoomConfiguration.RoomConfig.LightCommanderConfiguration;

            IDeviceControlTimer
                ControlTimer = new DeviceControlTimer(new Timer_(TimeAllOn));

            int DeviceStartIndex = _SleepingRoomConfiguration.RoomConfig.LightCommanderConfiguration.Startindex;
            int DeviceFinalIndex = _SleepingRoomConfiguration.RoomConfig.LightCommanderConfiguration.Lastindex;
            HeaterCommanderConfiguration HeaterConfig = _SleepingRoomConfiguration.HeaterConfig;

            IDeviceScenarioControl
                ScenarioControl = new DeviceScenarioControl(
                                                            DeviceStartIndex, 
                                                            DeviceFinalIndex,
                                                            new Timer_(TimeNextScenario),
                                                            new Timer_(0),
                                                            new Timer_(0) 
                                                            );

            IExtendedLightCommander ExtendedLightCommander = new ExtendedLightCommander(CommanderConfig,
                                                                ControlTimer,
                                                                ScenarioControl);

            IHeaterCommander HeaterCommander = new HeaterCommander(HeaterConfig, ControlTimer);

            InfoWait();

            IOHandler IOHandler_ = new IOHandler(HandlerMode.eHardware);

            if (IOHandler_.Attached)
            {
                IOHandler_.SetAllOutputs(false);
                ISleepingRoomController SleepingRoom =   new SleepingRoomController(
                                                                 _SleepingRoomConfiguration,
                                                                 IOHandler_, 
                                                                 SenderReceiver,
                                                                 ExtendedLightCommander, 
                                                                 HeaterCommander );
                InfoMode();
                WaitUntilKeyPressed();
                IOHandler_.SetAllOutputs(false);
                Environment.Exit(0);
            }
         }
        #endregion

        static void DebugAnteRoomConfiguration()
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
  
        #endregion
    }
}
