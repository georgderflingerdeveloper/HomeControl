using HomeControl.ADVANCED_COMPONENTS;
using SystemServices;
using System;
using HomeControl.ROOMS.CONFIGURATION;

namespace HomeControl.ROOMS
{
    class DefaultSettingsLightControl
    {
        static readonly public double DelayTimeAllOutputsOff                           = TimeConverter.ToMiliseconds( 10 );
        static readonly public double DelayTimeAutomaticOffRoom                        = TimeConverter.ToMiliseconds( 5 );
        static readonly public double DelayTimeAllOn                                   = TimeConverter.ToMiliseconds( 1.3 );
        static readonly public double DelayTimeAutomaticOffViaPresenceDetector         = TimeConverter.ToMiliseconds( 10, 0 );
        static readonly public double DelayTimeFinalOff                                = TimeConverter.ToMiliseconds( 5 );
        static readonly public double DelayTimeDoingNothing                            = TimeConverter.ToMiliseconds( 1.5 );
        static readonly public double DelayTimeNextScenario                            = TimeConverter.ToMiliseconds( 0.5 );
    }

    class DefaultSettingsHeaterControl
    {
        static double LittleDelay = 300;
        static readonly public double DelayTimeAllOn                                   =  DefaultSettingsLightControl.DelayTimeDoingNothing + LittleDelay;
        static readonly public double DelayTimeFinalOff                                = TimeConverter.ToMiliseconds( 12, 0, 0 );
    }

    [Serializable]
    public class RoomConfiguration
    {
        CommanderConfiguration _LightCommanderConfiguration;
        ScenarioConfiguration  _ScenarioConfiguration;

        public CommanderConfiguration LightCommanderConfiguration
        {
            get
            {
                return _LightCommanderConfiguration;
            }

            set
            {
                _LightCommanderConfiguration = value;
            }
        }

        public ScenarioConfiguration ScenarioConfiguration
        {
            get
            {
                return _ScenarioConfiguration;
            }

            set
            {
                _ScenarioConfiguration = value;
            }
        }
    }
}
