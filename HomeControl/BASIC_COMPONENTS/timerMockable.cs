using System.Timers;

namespace BASIC_COMPONENTS
{
    public interface ITimer
    {
        void Start( );
        void Stop( );
        void SetIntervall( double intervall );
        bool IsStarted( );
        event ElapsedEventHandler Elapsed;
    }


    public class Timer_ : ITimer
    {
        bool _IsStarted = false;

        public void SetIntervall( double intervall )
        {
            if( intervall > 0 )
            {
                timer.Interval = intervall;
            }
        }

        private Timer timer = new Timer();

        public Timer_( double intervall )
        {
            if( intervall > 0 )
            {
                timer.Interval = intervall;
            }
        }

        public void Start( )
        {
            if( timer.Interval > 0 )
            {
                timer.Start( );
                _IsStarted = true;
            }
        }

        public bool IsStarted()
        {
            return _IsStarted;
        } 

        public void Stop( )
        {
            if( timer.Interval > 0 )
            {
                timer.Stop( );
                _IsStarted = false;
            }
        }

        public event ElapsedEventHandler Elapsed
        {
            add    { timer.Elapsed += value; }
            remove { timer.Elapsed -= value; }
        }
    }
}
