using System;
using System.Collections.Generic;
using SystemServices;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.ROOMS.CONFIGURATION;

namespace HomeControl.ROOMS.CONFIGURATION
{
    static class IOAssignmentControllerSleepingRoom
    {
        #region INPUTS
        public const int indDigitalInputMainButton              = 0;
        public const int indDigitalInputWindowWest              = 2;  // Fenster hinten Richtung Westen
        public const int indDigitalInputMansardWindowNorthLeft  = 3;  // Velux Fenster links
        public const int indDigitalInputMansardWindowNorthRight = 4;  // Velux Fenster rechts
        public const int indDigitalInputFireAlert               = 5;  // fire alert - GIRA Rauchmelder
        #endregion

        #region OUTPUTS
        public const int indDigitalOutputLightMansardRightEnd        = 0;  // Leuchte Mansarden ganz rechts
        public const int indDigitalOutputLightBarMansardWindowRight  = 1;  // Led Balken Mansarden Fenster rechter Rand 
        public const int indDigitalOutputLightBarMansardWindowMiddle = 2;  // Led Balken zischen linken und rechten Mansarden Fenster 
        public const int indDigitalOutputLightBarMansardWindowLeft   = 3;  // Led Balken Mansarden Fenster rechter Rand 
        public const int indDigitalOutputLightCeiling                = 4;  // Leuchte an der Decke
        public const int indDigitalOutputHeater                      = 5;  // Heizkörper
        #endregion

        public static Dictionary<int, string> DigitalInputDictionary = new Dictionary<int, string>
        {
                {indDigitalInputMainButton,                         nameof(indDigitalInputMainButton)                                   },
                {indDigitalInputWindowWest,                         nameof(indDigitalInputWindowWest)                                   },
                {indDigitalInputMansardWindowNorthLeft,             nameof(indDigitalInputMansardWindowNorthLeft)                                   },
                {indDigitalInputMansardWindowNorthRight,            nameof(indDigitalInputMansardWindowNorthRight)                                   },
                {indDigitalInputFireAlert,                          nameof(indDigitalInputFireAlert)                                   },

        };

        public static Dictionary<int, string> DigitalOutputDictionary = new Dictionary<int, string>
        {
                {indDigitalOutputLightMansardRightEnd,              nameof(indDigitalOutputLightMansardRightEnd)                                   },
                {indDigitalOutputLightBarMansardWindowRight,        nameof(indDigitalOutputLightBarMansardWindowRight)                                   },
                {indDigitalOutputLightBarMansardWindowMiddle,       nameof(indDigitalOutputLightBarMansardWindowMiddle)                                   },
                {indDigitalOutputLightBarMansardWindowLeft,         nameof(indDigitalOutputLightBarMansardWindowLeft)                                   },
                {indDigitalOutputLightCeiling,                      nameof(indDigitalOutputLightCeiling)                                   },
                {indDigitalOutputHeater,                            nameof(indDigitalOutputHeater)                                   },
        };

        static string GetDeviceName(Dictionary<int, string> IoDictionary, int key)
        {
            IoDictionary.TryGetValue(key, out string devicename);
            return (devicename);
        }

        public static string GetInputDeviceName(int key)
        {
            return (GetDeviceName(DigitalInputDictionary, key));
        }

        public static string GetOutputDeviceName(int key)
        {
            return (GetDeviceName(DigitalOutputDictionary, key));
        }

    }

    [Serializable]
    class DefaultSettingsLightControlSleepingRoom : DefaultSettingsLightControl
    {
    }

    static class RangeIndexSleepingRoom
    {
        public const int indAFirstLight = IOAssignmentControllerSleepingRoom.indDigitalOutputLightMansardRightEnd;
        public const int indALastLight  = IOAssignmentControllerSleepingRoom.indDigitalOutputLightCeiling;
    }

    [Serializable]
    public class SleepingRoomConfiguration : BaseConfiguration
    {
        public SleepingRoomConfiguration() : base()
        {
            base.CommunicationConfig = new CommunicationConfiguration() { };
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

        #region SLEEPINGROOM_CONFIGURATION
        RoomConfiguration _SleepingRoom = new RoomConfiguration()
        {
            // default settings
            LightCommanderConfiguration = new CommanderConfiguration()
            {
                Startindex = RangeIndexSleepingRoom.indAFirstLight,
                Lastindex = RangeIndexSleepingRoom.indALastLight,
                DelayTimeAllOn = DefaultSettingsLightControlSleepingRoom.DelayTimeAllOn,
                DelayTimeOffByMissingTriggerSignal = DefaultSettingsLightControlSleepingRoom.DelayTimeAutomaticOffViaPresenceDetector,
                DelayTimeFinalOff = DefaultSettingsLightControlSleepingRoom.DelayTimeFinalOff,
                DelayTimeDoingNothing = DefaultSettingsLightControlSleepingRoom.DelayTimeDoingNothing,
                Modes = DeviceCommandos.ScenarioLight,
                ModesAutomaticoff = DeviceCommanderAutomaticOff.WithMainTrigger,
                DeviceRemainOnAfterAutomaticOff = new List<int>
                {
                }
            },

            ScenarioConfiguration = new ScenarioConfiguration()
            {
                DelayTimeNextScenario = DefaultSettingsLightControlAnteRoom.DelayTimeNextScenario,
                Scenarios = new List<List<int>>
                {
                    // scenario 0
                    new List<int>
                        {
                           IOAssignmentControllerSleepingRoom.indDigitalOutputLightBarMansardWindowLeft,
                           IOAssignmentControllerSleepingRoom.indDigitalOutputLightBarMansardWindowMiddle,
                           IOAssignmentControllerSleepingRoom.indDigitalOutputLightBarMansardWindowRight
                        },

                    // scenario 1
                    new List<int>
                        {

                        },

                    // scenario 2
                    new List<int>
                        {
                        },

                    // scenario 3
                    new List<int>
                        {
                        },

                }

            }
        };
        #endregion



    }
}
