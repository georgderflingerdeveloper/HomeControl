namespace HomeControl.BASIC_CONSTANTS
{
    public static class Trigger
    {
        public const bool FALLING = false;
        public const bool RISING  = true;
    }

    static public class Edge
    {
        public const bool Falling = false;
        public const bool Rising  = true;
    }

    public class PowerState
    {
        public const bool ON   = true;
        public const bool OFF  = false;
    }

    public class TurnDevice : PowerState
    {
    }
}

