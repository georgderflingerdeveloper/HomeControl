namespace BASIC_COMPONENTS
{
    using System.Timers;

    #region Interfaces

    /// <summary>
    /// Defines the <see cref="ITimer" />
    /// </summary>
    public interface ITimer
    {
        #region Events

        /// <summary>
        /// Defines the Elapsed
        /// </summary>
        event ElapsedEventHandler Elapsed;

        #endregion

        #region Methods

        /// <summary>
        /// The IsStarted
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        bool IsStarted();

        /// <summary>
        /// The SetIntervall
        /// </summary>
        /// <param name="intervall">The <see cref="double"/></param>
        void SetIntervall( double intervall );

        /// <summary>
        /// The Start
        /// </summary>
        void Start();

        /// <summary>
        /// The Stop
        /// </summary>
        void Stop();

        #endregion
    }

    #endregion


    public class Timer_ : ITimer
    {

        bool _IsStarted = false;

        public void SetIntervall( double intervall )
        {
            if (intervall > 0)
            {
                timer.Interval = intervall;
            }
        }

        private Timer timer = new Timer( );

        public Timer_( double intervall )
        {
            if (intervall > 0)
            {
                timer.Interval = intervall;
            }
        }

        public void Start()
        {
            if (timer.Interval > 0)
            {
                timer.Start( );
                _IsStarted = true;
            }
        }

        public bool IsStarted()
        {
            return _IsStarted;
        }

        public void Stop()
        {
            if (timer.Interval > 0)
            {
                timer.Stop( );
                _IsStarted = false;
            }
        }

        public event ElapsedEventHandler Elapsed
        {
            add { timer.Elapsed += value; }
            remove { timer.Elapsed -= value; }
        }
    }
}
