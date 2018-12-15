using System;
using System.Collections.Generic;
using SystemServices;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.ROOMS.CONFIGURATION;

namespace HomeControl.ROOMS
{
 
    static class IOAssignmentControllerAnteBathWashRoom
    {
        #region INPUTS

        #region DIGITAL_INPUTS_ANTEROOM
         public const int indDigitalInputAnteRoomMainButton                         = 0;
         public const int indDigitalInputAnteRoomPresenceDetector                   = 3;
        #endregion

        #region DIGITAL_INPUTS_WASHROOM
         public const int indDigitalInputWashRoomMainButton                         = 1;
        #endregion

        #region DIGITAL_INPUTS_BATHROOM
         public const int indDigitalInputBathRoomMainButton                         = 2;
         public const int indDigitalInputWindow                                     = 4;
        #endregion

        #endregion

        #region OUTPUTS

        #region DIGITAL_OUTPUTS_ANTEROOM
         public const int indDigitalOutputAnteRoomMainLight                         = 0;
         public const int indDigitalOutputAnteRoomBackSide                          = 1;
         public const int indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle1 = 2;
         public const int indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle2 = 3;
         public const int indDigitalOutputAnteRoomNightLight                        = 4;
        #endregion

        #region DIGITAL_OUTPUTS_BATHROOM
         public const int indDigitalOutputBathRoomCenterLight                       = 7;  // BAD LICHT MITTE
         public const int indDigitalOutputBathRoomCornerLeftLight                   = 8;  // BAD LICHT ECKE LINKS
         public const int indDigitalOutputBathRoomShowerLight                       = 9;  // BAD LICHT DUSCHE
         public const int indDigitalOutputBathRoomWindowLight                       = 10; // BAD LICHT FENSTER
         public const int indDigitalOutputBathRoomRBGPanelOverBathTub               = 11; // BAD RGB LED PANEL
         public const int indDigitalOutputBathRoomHeater                            = 12;
        #endregion

        #region DIGITAL_OUTPUTS_WASHROOM
         public const int indDigitalOutputWashRoomMainLight                         = 5;
         public const int indDigitalOutputWashRoomDimLight                          = 6;
        #endregion

        public static Dictionary<int, string> DigitalInputDictionary = new Dictionary<int, string>
        {
                {indDigitalInputAnteRoomMainButton,                         nameof(indDigitalInputAnteRoomMainButton)                                   },
                {indDigitalInputAnteRoomPresenceDetector,                   nameof(indDigitalInputAnteRoomPresenceDetector)                             },
                {indDigitalInputWashRoomMainButton,                         nameof(indDigitalInputWashRoomMainButton)                                   },
                {indDigitalInputBathRoomMainButton,                         nameof(indDigitalInputBathRoomMainButton)                                   },
                {indDigitalInputWindow,                                     nameof(indDigitalInputWindow)                                               },
        };

        public static Dictionary<int, string> DigitalOutputDictionary = new Dictionary<int, string>
        {
                {indDigitalOutputAnteRoomMainLight,                         nameof(indDigitalOutputAnteRoomMainLight)                                   },
                {indDigitalOutputAnteRoomBackSide,                          nameof(indDigitalOutputAnteRoomBackSide)                                    },
                {indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle1, nameof(indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle1)           },
                {indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle2, nameof(indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle2)           },
                {indDigitalOutputAnteRoomNightLight,                        nameof(indDigitalOutputAnteRoomNightLight)                                  },
                {indDigitalOutputBathRoomCenterLight,                       nameof(indDigitalOutputBathRoomCenterLight)                                 },
                {indDigitalOutputBathRoomCornerLeftLight,                   nameof(indDigitalOutputBathRoomCornerLeftLight)                             },
                {indDigitalOutputBathRoomShowerLight,                       nameof(indDigitalOutputBathRoomShowerLight)                                 },
                {indDigitalOutputBathRoomWindowLight,                       nameof(indDigitalOutputBathRoomWindowLight)                                 },
                {indDigitalOutputBathRoomRBGPanelOverBathTub,               nameof(indDigitalOutputBathRoomRBGPanelOverBathTub)                         },
                {indDigitalOutputBathRoomHeater,                            nameof(indDigitalOutputBathRoomHeater)                                      },
                {indDigitalOutputWashRoomMainLight,                         nameof(indDigitalOutputWashRoomMainLight)                                   },
                {indDigitalOutputWashRoomDimLight,                          nameof(indDigitalOutputWashRoomDimLight)                                    },
        };

        static string GetDeviceName( Dictionary<int, string> IoDictionary, int key )
        {
             IoDictionary.TryGetValue( key, out string devicename );
             return ( devicename );
        }

        public static string GetInputDeviceName( int key )
        {
            return ( GetDeviceName( DigitalInputDictionary, key ) );
        }

        public static string GetOutputDeviceName( int key )
        {
            return ( GetDeviceName( DigitalOutputDictionary, key ) );
        }

        #endregion
    }

    class DefaultSettingsLightControlAnteRoom : DefaultSettingsLightControl
    {
        static public double DelayOffOnMissingPresenceSignal = TimeConverter.ToMiliseconds( 2, 30 );
        new static readonly public double DelayTimeFinalOff  = TimeConverter.ToMiliseconds( 1, 30, 0 );
    }

    class DefaultSettingsLightControlBathRoom : DefaultSettingsLightControl
    {
        new static readonly public double DelayTimeFinalOff  = TimeConverter.ToMiliseconds( 60, 0 );
    }

    class DefaultSettingsHeaterControlBathRoom : DefaultSettingsHeaterControl
    {
    }

    class DefaultSettingsLightControlWashRoom : DefaultSettingsLightControl
    {
        new static readonly public double DelayTimeFinalOff  = TimeConverter.ToMiliseconds( 20, 0 );
    }

    static class RangeIndexAnteRoom
    {
        public const int indAFirstLight    = 0;
        public const int indALastLight     = 3;
    }

    static class RangeIndexWashRoom
    {
        public const int indWFirstLight = 5;
        public const int indWLastLight  = 6;
    }

    static class RangeIndexBathRoom
    {
        public const int indBFirstLight = 7;
        public const int indBLastLight  = 11;
    }

    static class ScenarioConstantsAnteRoom
    {
        public const int ScenarioMainLightOnly = 3;
    }

    [Serializable]
    public class BaseConfiguration
    {
        public CommunicationConfiguration CommunicationConfig { get; set; }
    }

    [Serializable]
    public class AnteBathWashRoomConfiguration : BaseConfiguration
    {
        public AnteBathWashRoomConfiguration() : base()
        {
            base.CommunicationConfig = new CommunicationConfiguration( ) {};
        }

        #region COMMUNICATION_CONFIGURATION

        #endregion

        #region HARDWARECONFIGURATION 
        HardwareConfiguration _HardwareConfiguration = new HardwareConfiguration()
        {
            IOPrimerIds = new List<int>()
            {
                 123456,
                 789102
            },
        };
        #endregion

        #region ANTEROOM_CONFIGURATION
        RoomConfiguration _AnteRoom = new RoomConfiguration()
        { 
            // default settings
            LightCommanderConfiguration = new CommanderConfiguration()
            {
                Startindex                          = RangeIndexAnteRoom.indAFirstLight,
                Lastindex                           = RangeIndexAnteRoom.indALastLight,
                DelayTimeAllOn                      = DefaultSettingsLightControlAnteRoom.DelayTimeAllOn,
                DelayTimeOffByMissingTriggerSignal  = DefaultSettingsLightControlAnteRoom.DelayTimeAutomaticOffViaPresenceDetector,
                DelayTimeFinalOff                   = DefaultSettingsLightControlAnteRoom.DelayTimeFinalOff,
                DelayTimeDoingNothing               = DefaultSettingsLightControlAnteRoom.DelayTimeDoingNothing,
                Modes                               = DeviceCommandos.ScenarioLight,
                ModesAutomaticoff                   = DeviceCommanderAutomaticOff.WithPresenceTrigger,
                DeviceRemainOnAfterAutomaticOff     = new List<int> 
                {
                   IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle1,
                   IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle2
                }
            },

            ScenarioConfiguration = new ScenarioConfiguration()
            {
                DelayTimeNextScenario               = DefaultSettingsLightControlAnteRoom.DelayTimeNextScenario,
                Scenarios = new List<List<int>>
                {
                    // scenario 0
                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomMainLight,
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomBackSide
                              },

                    // scenario 1
                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomMainLight,
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomBackSide,
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle1,
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle2
                              },

                    // scenario 2
                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle1,
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomRoofBackSideFloorSpotGroupMiddle2
                              },

                    // scenario 3
                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomMainLight,
                              },

                }

            }
        };
        #endregion
 
        #region BATHROOM_CONFIGURATION
        RoomConfiguration _BathRoom = new RoomConfiguration()
        { 
            // default settings
            LightCommanderConfiguration = new CommanderConfiguration()
            {
                Startindex                          = RangeIndexBathRoom.indBFirstLight,
                Lastindex                           = RangeIndexBathRoom.indBLastLight,
                DelayTimeAllOn                      = DefaultSettingsLightControlBathRoom.DelayTimeAllOn,
                DelayTimeOffByMissingTriggerSignal = DefaultSettingsLightControlBathRoom.DelayTimeAutomaticOffViaPresenceDetector,
                DelayTimeFinalOff                   = DefaultSettingsLightControlBathRoom.DelayTimeFinalOff,
                DelayTimeDoingNothing               = DefaultSettingsLightControlBathRoom.DelayTimeDoingNothing,
                Modes                               = DeviceCommandos.ScenarioLight,
                ModesAutomaticoff                   = DeviceCommanderAutomaticOff.WithMainTrigger,
            },

            ScenarioConfiguration = new ScenarioConfiguration()
            {
                DelayTimeNextScenario               = DefaultSettingsLightControlBathRoom.DelayTimeNextScenario,
                Scenarios = new List<List<int>>
                {
                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputBathRoomCenterLight,
                              },

                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputBathRoomRBGPanelOverBathTub
                              },

                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputBathRoomCenterLight,
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputBathRoomRBGPanelOverBathTub
                              },

                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputBathRoomCornerLeftLight,
                              },

                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputBathRoomShowerLight,
                              },

                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputBathRoomWindowLight,
                              },
                }

            }
        };


        #endregion

        #region WASHROOM_CONFIGURATION
        RoomConfiguration _WashRoom = new RoomConfiguration()
        { 
            // default settings
            LightCommanderConfiguration = new CommanderConfiguration()
            {
                Startindex                          = RangeIndexWashRoom.indWFirstLight,
                Lastindex                           = RangeIndexWashRoom.indWLastLight,
                DelayTimeAllOn                      = DefaultSettingsLightControlWashRoom.DelayTimeAllOn,
                DelayTimeOffByMissingTriggerSignal =  DefaultSettingsLightControlWashRoom.DelayTimeFinalOff,
                DelayTimeFinalOff                   = DefaultSettingsLightControlWashRoom.DelayTimeFinalOff,
                DelayTimeDoingNothing               = DefaultSettingsLightControlWashRoom.DelayTimeDoingNothing,
                Modes                               = DeviceCommandos.ScenarioLight,
                ModesAutomaticoff                   = DeviceCommanderAutomaticOff.WithMainTrigger,
            },

            ScenarioConfiguration = new ScenarioConfiguration()
            {
                DelayTimeNextScenario               = DefaultSettingsLightControlWashRoom.DelayTimeNextScenario,
                Scenarios = new List<List<int>>
                {

                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputWashRoomMainLight,
                              },

                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputWashRoomDimLight,
                              },

                    new List<int> {
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputWashRoomMainLight,
                                IOAssignmentControllerAnteBathWashRoom.indDigitalOutputWashRoomDimLight,
                              },
                }

            }
        };
        #endregion

        #region PRESENCE_LIGHT
        CommanderConfiguration _PresenceLightConfiguration = new CommanderConfiguration()
        {
            Startindex = IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomNightLight,
            Lastindex  = IOAssignmentControllerAnteBathWashRoom.indDigitalOutputAnteRoomNightLight,
            DelayTimeOffByMissingTriggerSignal = DefaultSettingsLightControlAnteRoom.DelayOffOnMissingPresenceSignal,
            Modes = DeviceCommandos.OnWithDelayedOffByFallingEdge
        };
        #endregion

        #region BATHROOM_HEATER_CONFIGURATION
        HeaterCommanderConfiguration _HeaterBathRoom = new HeaterCommanderConfiguration()
        {
            Startindex        = IOAssignmentControllerAnteBathWashRoom.indDigitalOutputBathRoomHeater,
            Lastindex         = IOAssignmentControllerAnteBathWashRoom.indDigitalOutputBathRoomHeater,
            Modes             = DeviceCommandos.SingleDeviceToggleRisingEdge,
            Modesdelayedon    = DeviceCommanderDelayedOn.WithMainTrigger,
            ModesAutomaticoff = DeviceCommanderAutomaticOff.WithMainTrigger,
            DelayTimeAllOn    = DefaultSettingsHeaterControlBathRoom.DelayTimeAllOn,
            DelayTimeFinalOff = DefaultSettingsHeaterControlBathRoom.DelayTimeFinalOff
        };

 
        #endregion

        #region PROPERTIES
        public RoomConfiguration AnteRoom
        {
            get
            {
                return _AnteRoom;
            }

            set
            {
                _AnteRoom = value;
            }
        }

        public RoomConfiguration BathRoom
        {
            get
            {
                return _BathRoom;
            }

            set
            {
                _BathRoom = value;
            }
        }

        public RoomConfiguration WashRoom
        {
            get
            {
                return _WashRoom;
            }

            set
            {
                _WashRoom = value;
            }
        }

        public CommanderConfiguration PresenceLightConfiguration
        {
            get
            {
                return _PresenceLightConfiguration;
            }

            set
            {
                _PresenceLightConfiguration = value;
            }
        }

        public HeaterCommanderConfiguration HeaterBathRoom
        {
            get
            {
                return _HeaterBathRoom;
            }

            set
            {
                _HeaterBathRoom = value;
            }
        }

        public HardwareConfiguration HardwareConfiguration
        {
            get
            {
                return _HardwareConfiguration;
            }

            set
            {
                _HardwareConfiguration = value;
            }
        }
        #endregion
    }
}
