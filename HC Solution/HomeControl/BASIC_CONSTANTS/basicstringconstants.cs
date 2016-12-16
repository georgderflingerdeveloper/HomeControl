using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.BASIC_CONSTANTS
{
    static class basicstringconstants
    {
        public static string ApplicationTitle = " * * * * * * * * * * HOMEAUTOMATION CONTROL SOFTWARE * * * * * * * * * * ";
        public const string InfoPhidgetException                                   =  "Phidget Exception";
        public const string InfoNoIO                                               =  "No IO primer is attached ( probably unplugged )";
        public const string AppPrefix                                              =  "Home Automation Comander: ";
        public const string AppCmdLstPrefix                                        =  "Home Automation Comado List: ";
        public const string ConfigFileName                                         =  "conf.ini";
        public const string IniSection                                             =  "SECTION";
        public const string IniSectionPhidgets                                     =  "PHIDGETS";
        public const string OperationMode                                          =  "Selected Operation Mode: ";
        public const string DeviceNotFound                                         =  "Device not found!";
        public const string FailedToEstablishClient                                =  "Failed to establish client! ";
        public const string FailedToEstablishUDPBroadcast                          =  "Failed to establish UDP invitation Broadcast! ";
        public const string FailedToEstablishUDPReceive                            =  "Failed to establisch UDP receive!";
        public const string FailedToEstablishServer                                =  "Failed to establish server! ";
        public const string ReceiveInvitationNotPossible                           =  "Receive invitation data is not possible! reason:";
        public const string GreetingToServer                                       =  "Hello Server - This is ";
        public const string MyComputernameIs                                       =  "  my computername is";
        public const string SorryServerIsNotConnectedWithClient                    =  "Sorry server is not connected with client!";
        public const string ComunicationProtocollError                             =  "Home automation comunication protocoll Error!";
        public const string SchedulerIsStartingDevice                              =  "Scheduler is Starting Device: ";
        public const string SchedulerIsStopingDevice                               =  "Scheduler is Stopping Device: ";
        public const string SchedulerIsRescheduling                                =  "Scheduler is rescheduling: ";
        public const string Asking                                                 =  "Is asking ...";
        public const string Scheduler                                              =  "Scheduler";
        public const string StatusOf                                               =  "Status of ";
        public const string Is                                                     =  "is:";
        public const string FailedToRecoverSchedulerData                           =  "Failed to recover scheduler data!";
        public const string LocationOutside                                        =  "Outside";
        public const string InfoConnectionFailed                                   =  "Sorry - connection failed - press enter for reconnect";
        public const string InfoTryToReconnect                                     =  "Try to reconnect ...";
        public const string InfoStartingTime                                       =  "Starting time: ";
        public const string InfoVersion                                            =  "Version: ";
        public const string InfoLastBuild                                          =  "Last build: ";
        public const string InfoVersioninformation                                 =  "Versioninformation: ";
        public const string InfoLoadingConfiguration                               =  "Loading configuration ...";
        public const string InfoExpectingPhidget                                   =  "Expecting Phidget ";
        public const string InfoNoConfiguredPhidgetIDused                          =  "No configured Phidget ID´s used...";
        public const string InfoLoadingConfigurationSucessfull                     =  "Loading configuration successfull";
        public const string InfoIniDidNotFindProperConfiguration                   =  "Did not find any proper configuration! - check content of ini file";
        public const string FailedToLoadConfiguration                              =  "Failed to load configuration!";
        public const string InfoTypeExit                                           =  "press (E) or (enter) ... for leave";
        public const string Exit                                                   =  "EXIT";
        public const string StartTimerForRecoverScheduler                          =  "Start timer for recover scheduler ...";
        public const string RemainingTime                                          =  "Remaining time ... ";
        public const string ON                                                     =  "ON";
        public const string OFF                                                    =  "OFF";
        public const string PUSH                                                   =  "PUSH";
        public const string PULL                                                   =  "PULL";
    }

    static class datahandlstringconst
    {
        public const string DataStoringFailed = "Failed to store data";
        public const string DataLoadingFailed = "Failed to load data";
    }

    static class Seperators
    {
        public static char[] delimiterChars                   =  { ' ', ',', '.', ':', '\t', '#' };
        public static char[] delimiterCharsExtended           =  { ' ', ',', '.', ':', '\t', '#', '_' };
        public static char[] delimiterCharsSchedulerReserved  =  { ' ', '_' };
        public static string Spaceholder  = " ";
        public const string InfoSeperator = "_";
    }

    static class InfoOperationMode
    {
        public const string SLEEPING_ROOM                                           =  "SLEEPINGROOM";
        public const string CENTER_KITCHEN_AND_LIVING_ROOM                          =  "CENTERKITCHENLIVINGROOM";
        public const string ANTEROOM                                                =  "ANTEROOM";
        public const string LIVING_ROOM_EAST                                        =  "LIVINGROOMEAST";
        public const string LIVING_ROOM_WEST                                        =  "LIVINGROOMWEST";
        public const string OUTSIDE                                                 =  "OUTSIDE";
        public const string POWERCENTER                                             =  "POWERCENTER";
    }

    public static class HardwareDevices
    {
        public const string Boiler                                      = "Boiler";
        public const string PumpWarmwater                               = "PumpWarmwater";           // Zirkulationspumpe für Warmwasser ( Küche, Bad, Dusche )
        public const string PumpCirculation                             = "PumpCirculation";         // Zirkulationspumpe Heizkreis
        public const string DoorEntryAnteRoom                           = "DoorEntryAnteRoom";       // Eingangstür Vorhaus
        public const string HeaterLivingRoomEast                        = "HeaterLivingRoomEast";    // Heizkörper Wohnzimmer Ost seitig
        public const string HeaterLivingRoomWest                        = "HeaterLivingRoomWest";    // Heizkörper Wohnzimmer West seitig
        public const string HeaterAnteRoom                              = "HeaterAnteRoom";
        public const string HeaterBathRoom                              = "HeaterBathRoom";
        public const string HeaterDryerBathRoom                         = "DryerBathRoom";
        public const string HeaterSleepingRoom                          = "HeaterSleepingRoom";
        public const string HeaterNursery                               = "HeaterNursery"; // kinderzimmer = nursery
    }

    public static class FileNames
    {
        public const string format           = ".xml";
        public const string Controller       = "controller" + format;
        public const string Anteroom         = "anteroom" + format;
    }

    public static class Messages
    {
        public const string LoadingFile = "Loading File";
        public const string DottedSpace = "...";
        public const string WhiteSpace = " ";
    }
 }


